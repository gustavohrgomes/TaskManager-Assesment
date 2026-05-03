# GenAI Artifact: Task Management API

This project was built using Claude Code with structured planning and execution workflows (GSD). Rather than a single prompt-and-paste approach, the development involved iterative AI-assisted work across five phases with TDD discipline, architecture tests, and plan-check-verify gates. The artifact below demonstrates what happens when you ask an AI to generate a task management API in a single prompt, and why critical evaluation of the output matters.

## 1. Prompt

**System context:**

```
You are a senior .NET developer. Generate production-ready code following Clean Architecture principles.
Use ASP.NET Core Web API with controllers. Use PostgreSQL for persistence. Include JWT authentication.
```

**User prompt:**

```
Generate a REST API for a task management system with:
- CRUD operations for tasks (title, description, status, due_date)
- User authentication (register, login, JWT bearer tokens)
- Task ownership: users can only access their own tasks
- ASP.NET Core with Clean Architecture (Domain, Application, Infrastructure, API layers)
- PostgreSQL for data persistence
- Include the TasksController with all endpoints
```

## 2. Raw AI Output

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMediator _mediator;

    public TasksController(AppDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetAll()
    {
        var tasks = await _context.Tasks.ToListAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetById(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create(CreateTaskDto dto)
    {
        if (string.IsNullOrEmpty(dto.Title))
            return BadRequest("Title is required");

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = TaskStatus.Pending,
            DueDate = dto.DueDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskItem>> Update(int id, UpdateTaskDto dto)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return NotFound();

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;
        task.DueDate = dto.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(task);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

// Program.cs (partial)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
```

## 3. Critique

The following issues were identified through senior code review of the AI output:

1. **Uses Entity Framework Core (spec violation).** The interview spec explicitly bans EF Core, Dapper, and MediatR. The AI defaulted to `DbContext` because it is the most common .NET data access pattern in training data. This is the most critical issue: submitting this code would demonstrate a failure to read the spec.

2. **Uses MediatR (spec violation).** The AI included `IMediator` injection and `AddMediatR` registration. The spec bans MediatR explicitly. The AI chose it because CQRS+MediatR is the dominant Clean Architecture pattern in open-source .NET projects.

3. **Missing per-user ownership filter (security vulnerability).** `GetAll()` returns `_context.Tasks.ToListAsync()` without filtering by the authenticated user's ID. `GetById()` uses `FindAsync(id)` without an ownership check. Any authenticated user could read, update, or delete any other user's tasks.

4. **No ProblemDetails error handling.** The AI returns `BadRequest("Title is required")` as a plain string. The API should return RFC 7807 ProblemDetails with structured `type`, `title`, `status`, `detail`, and `traceId` fields. ASP.NET Core provides `AddProblemDetails()` and `UseExceptionHandler()` for this.

5. **Superficial inline validation.** A single `string.IsNullOrEmpty` check instead of FluentValidation rules with proper error aggregation. The AI skipped input validation because the prompt did not mention it, but a production API needs it.

6. **Missing JWT validation hardening.** The AI sets `ValidateIssuer = false` and `ValidateAudience = false`. This accepts tokens from any issuer, defeating the purpose of JWT authentication. Production settings must validate issuer, audience, lifetime, signing key, and restrict algorithms.

7. **No timing-safe login.** The AI's typical login implementation returns immediately on "user not found" but only after hash comparison on "wrong password". The response time difference enables user enumeration. A correct implementation always runs a dummy hash comparison.

## 4. Corrections

### Correction 1: Replace EF Core with raw Npgsql via handler injection

**Before (AI output):**
```csharp
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMediator _mediator;

    public TasksController(AppDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }
}
```

**After (actual implementation):**
```csharp
public sealed class TasksController(
    CreateTaskHandler createHandler,
    GetTaskHandler getHandler,
    ListTasksHandler listHandler,
    UpdateTaskHandler updateHandler,
    DeleteTaskHandler deleteHandler,
    IValidator<CreateTaskRequest> createValidator,
    IValidator<UpdateTaskRequest> updateValidator) : ControllerBase
```

**Rationale:** The controller depends only on Application-layer handler classes and FluentValidation validators. No EF Core, no MediatR. Handlers are plain classes registered in DI that take `ITaskRepository` (backed by raw Npgsql) as a constructor dependency. This satisfies the spec's ban on EF Core and MediatR while maintaining Clean Architecture's dependency inversion.

### Correction 2: Add per-user ownership filter

**Before (AI output):**
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<TaskItem>> GetById(int id)
{
    var task = await _context.Tasks.FindAsync(id);
    if (task == null)
        return NotFound();
    return Ok(task);
}
```

**After (actual implementation):**
```csharp
[HttpGet("{id:guid}")]
public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
{
    var result = await getHandler.Handle(new GetTaskQuery(id), ct);
    return Ok(TaskResponse.From(result));
}
```

**Rationale:** The handler internally calls `IUserContext` to get the authenticated user's ID, then the repository SQL uses `WHERE task_id = @id AND owner_id = @userId`. If the task does not belong to the user, a `TaskNotOwnedByUserException` is thrown, which the `GlobalExceptionHandler` maps to 404 (not 403, to prevent information leakage about task existence).

### Correction 3: Replace generic errors with ProblemDetails

**Before (AI output):**
```csharp
if (string.IsNullOrEmpty(dto.Title))
    return BadRequest("Title is required");
```

**After (actual implementation):**
```csharp
var validation = createValidator.Validate(request);
if (!validation.IsValid)
{
    var errors = validation.Errors
        .Select(f => new ValidationError(f.PropertyName, f.ErrorMessage))
        .ToList();
    throw new DomainValidationException(errors);
}
```

**Rationale:** FluentValidation provides rich, composable rules. The thrown `DomainValidationException` is caught by `GlobalExceptionHandler` which maps it to a 400 ProblemDetails response with structured error grouping by property name. The client receives RFC 7807-compliant JSON with `type`, `title`, `status`, `detail`, and an `errors` dictionary.

### Correction 4: Harden JWT validation

**Before (AI output):**
```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
    ValidateIssuer = false,
    ValidateAudience = false
};
```

**After (actual implementation):**
```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = jwtSettings.Issuer,
    ValidAudience = jwtSettings.Audience,
    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
    ClockSkew = TimeSpan.FromSeconds(30),
    IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(jwtSettings.Key))
};
```

**Rationale:** All five validation flags are set to true. The algorithm is restricted to HS256 only (preventing algorithm confusion attacks). ClockSkew is reduced from the 5-minute default to 30 seconds. The signing key comes from an Aspire parameter (injected at runtime, never committed to source). This ensures tokens are fully validated and cannot be forged using a different algorithm or accepted from an unauthorized issuer.
