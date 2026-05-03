using BallastLane.TaskManager.Tasks;

namespace BallastLane.TaskManager.API.Models;

public sealed record TaskResponse(
    Guid TaskId,
    string Title,
    string? Description,
    string Status,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public static TaskResponse From(TaskResult result) =>
        new(result.TaskId, result.Title, result.Description,
            FormatStatus(result.Status),
            result.DueDate, result.CreatedAt, result.UpdatedAt);

    private static string FormatStatus(Tasks.TaskStatus status) => status switch
    {
        Tasks.TaskStatus.Pending => "pending",
        Tasks.TaskStatus.InProgress => "inProgress",
        Tasks.TaskStatus.Completed => "completed",
        _ => status.ToString()
    };
}
