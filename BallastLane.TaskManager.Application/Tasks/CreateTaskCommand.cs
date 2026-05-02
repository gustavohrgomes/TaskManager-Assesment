namespace BallastLane.TaskManager.Application.Tasks;

public sealed record CreateTaskCommand(
    string Title,
    string? Description,
    DateTimeOffset? DueDate);
