using BallastLane.TaskManager.Domain.Tasks;
using DomainTaskStatus = BallastLane.TaskManager.Domain.Tasks.TaskStatus;

namespace BallastLane.TaskManager.Application.Tasks;

public sealed record TaskResult(
    Guid Id,
    Guid OwnerId,
    string Title,
    string? Description,
    DomainTaskStatus Status,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public static TaskResult From(TaskItem task) =>
        new(task.Id, task.OwnerId, task.Title, task.Description, task.Status, task.DueDate, task.CreatedAt, task.UpdatedAt);
}
