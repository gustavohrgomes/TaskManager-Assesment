using BallastLane.TaskManager.Common;
using BallastLane.TaskManager.Exceptions;
using BallastLane.TaskManager.Models;
using BallastLane.TaskManager.Tasks.CreateTask;
using BallastLane.TaskManager.Tasks.DeleteTask;
using BallastLane.TaskManager.Tasks.GetTasks;
using BallastLane.TaskManager.Tasks.UpdateTask;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using DomainTaskStatus = BallastLane.TaskManager.Tasks.TaskStatus;

namespace BallastLane.TaskManager.Controllers;

[ApiController]
[Route("tasks")]
[Authorize]
public sealed class TasksController(
    CreateTaskHandler createHandler,
    GetTaskHandler getHandler,
    GetTasksHandler listHandler,
    UpdateTaskHandler updateHandler,
    DeleteTaskHandler deleteHandler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskRequest request, [FromServices] IValidator<CreateTaskRequest> createValidator, CancellationToken cancellationToken)
    {
        ValidateRequest(createValidator, request);

        var result = await createHandler.Handle(
            new CreateTaskCommand(request.Title, request.Description, request.DueDate), cancellationToken);

        var response = TaskResponse.From(result);
        return CreatedAtAction(nameof(GetById), new { id = response.TaskId }, response);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] DateTimeOffset? dueBefore = null,
        [FromQuery] string? sort = null,
        [FromQuery] string? order = null,
        CancellationToken cancellationToken = default)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        page = Math.Max(page, 1);

        var parsedStatus = ParseStatus(status);
        var parsedSort = ParseSortField(sort);
        var parsedOrder = ParseSortOrder(order);

        var query = new TaskListQuery(Guid.Empty, page, pageSize, parsedStatus, dueBefore, parsedSort, parsedOrder);
        var result = await listHandler.Handle(query, cancellationToken);

        var response = new TaskListResponse(
            [.. result.Items.Select(TaskResponse.From)],
            result.TotalCount,
            result.Page,
            result.PageSize);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await getHandler.Handle(new GetTaskQuery(id), cancellationToken);
        return Ok(TaskResponse.From(result));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskRequest request, [FromServices] IValidator<UpdateTaskRequest> updateValidator, CancellationToken cancellationToken)
    {
        ValidateRequest(updateValidator, request);

        var parsedStatus = ParseStatusRequired(request.Status);
        var result = await updateHandler.Handle(
            new UpdateTaskCommand(id, request.Title, request.Description, request.DueDate, parsedStatus), cancellationToken);

        return Ok(TaskResponse.From(result));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await deleteHandler.Handle(new DeleteTaskCommand(id), cancellationToken);
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
        => !string.IsNullOrWhiteSpace(value) 
        ? ParseStatusRequired(value)
        : null;

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
