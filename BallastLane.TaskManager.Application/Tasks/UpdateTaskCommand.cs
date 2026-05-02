namespace BallastLane.TaskManager.Application.Tasks;

public sealed record UpdateTaskCommand(
    Guid TaskId,
    string Title,
    string? Description,
    DateTimeOffset? DueDate,
    BallastLane.TaskManager.Domain.Tasks.TaskStatus Status);
