using BallastLane.TaskManager.API.Models;
using BallastLane.TaskManager.Common;
using BallastLane.TaskManager.Exceptions;
using BallastLane.TaskManager.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using DomainTaskStatus = BallastLane.TaskManager.Tasks.TaskStatus;

namespace BallastLane.TaskManager.API.Controllers;

[ApiController]
[Route("tasks")]
[Authorize]
public sealed class TasksController(
    CreateTaskHandler createHandler,
    GetTaskHandler getHandler,
    ListTasksHandler listHandler,
    UpdateTaskHandler updateHandler,
    DeleteTaskHandler deleteHandler,
    IValidator<CreateTaskRequest> createValidator,
    IValidator<UpdateTaskRequest> updateValidator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskRequest request, CancellationToken ct)
    {
        ValidateRequest(createValidator, request);

        var result = await createHandler.Handle(
            new CreateTaskCommand(request.Title, request.Description, request.DueDate), ct);

        var response = TaskResponse.From(result);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] DateTimeOffset? dueBefore = null,
        [FromQuery] string? sort = null,
        [FromQuery] string? order = null,
        CancellationToken ct = default)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        page = Math.Max(page, 1);

        var parsedStatus = ParseStatus(status);
        var parsedSort = ParseSortField(sort);
        var parsedOrder = ParseSortOrder(order);

        var query = new TaskListQuery(Guid.Empty, page, pageSize, parsedStatus, dueBefore, parsedSort, parsedOrder);
        var result = await listHandler.Handle(query, ct);

        var response = new TaskListResponse(
            result.Items.Select(TaskResponse.From).ToList(),
            result.TotalCount,
            result.Page,
            result.PageSize);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await getHandler.Handle(new GetTaskQuery(id), ct);
        return Ok(TaskResponse.From(result));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskRequest request, CancellationToken ct)
    {
        ValidateRequest(updateValidator, request);

        var parsedStatus = ParseStatusRequired(request.Status);
        var result = await updateHandler.Handle(
            new UpdateTaskCommand(id, request.Title, request.Description, request.DueDate, parsedStatus), ct);

        return Ok(TaskResponse.From(result));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await deleteHandler.Handle(new DeleteTaskCommand(id), ct);
        return NoContent();
    }

    private static void ValidateRequest<T>(IValidator<T> validator, T request)
    {
        var validation = validator.Validate(request);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .Select(f => new ValidationError(f.PropertyName, f.ErrorMessage))
                .ToList();
            throw new DomainValidationException(errors);
        }
    }

    private static DomainTaskStatus? ParseStatus(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return ParseStatusRequired(value);
    }

    private static DomainTaskStatus ParseStatusRequired(string value) => value.ToLowerInvariant() switch
    {
        "pending" => DomainTaskStatus.Pending,
        "inprogress" => DomainTaskStatus.InProgress,
        "completed" => DomainTaskStatus.Completed,
        _ => throw new DomainValidationException("Status", "Status must be one of: pending, inProgress, completed.")
    };

    private static TaskSortField ParseSortField(string? value) => value?.ToLowerInvariant() switch
    {
        null or "" or "createdat" => TaskSortField.CreatedAt,
        "duedate" => TaskSortField.DueDate,
        "title" => TaskSortField.Title,
        "status" => TaskSortField.Status,
        _ => throw new DomainValidationException("Sort", "Sort must be one of: createdAt, dueDate, title, status.")
    };

    private static SortOrder ParseSortOrder(string? value) => value?.ToLowerInvariant() switch
    {
        null or "" or "asc" => SortOrder.Asc,
        "desc" => SortOrder.Desc,
        _ => throw new DomainValidationException("Order", "Order must be one of: asc, desc.")
    };
}
