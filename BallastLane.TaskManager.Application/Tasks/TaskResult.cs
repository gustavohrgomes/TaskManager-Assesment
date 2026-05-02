using DomainTaskStatus = BallastLane.TaskManager.Tasks.TaskStatus;

namespace BallastLane.TaskManager.Tasks;

/// <summary>
/// Read-side projection of a <see cref="TaskItem"/>.
/// </summary>
/// <param name="TaskItemId">Identifier of the task.</param>
/// <param name="OwnerId">Identifier of the owning user.</param>
/// <param name="Title">Short human-readable title.</param>
/// <param name="Description">Optional long-form description.</param>
/// <param name="Status">Current lifecycle status of the task.</param>
/// <param name="DueDate">Optional UTC due date.</param>
/// <param name="CreatedAt">UTC timestamp at which the task was created.</param>
/// <param name="UpdatedAt">UTC timestamp of the most recent mutation.</param>
public sealed record TaskResult(
    Guid TaskItemId,
    Guid OwnerId,
    string Title,
    string? Description,
    DomainTaskStatus Status,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    /// <summary>
    /// Builds a <see cref="TaskResult"/> projection from the supplied aggregate.
    /// </summary>
    /// <param name="task">Source aggregate to project.</param>
    /// <returns>A new projection mirroring <paramref name="task"/>.</returns>
    public static TaskResult From(TaskItem task) =>
        new(task.TaskItemId, task.OwnerId, task.Title, task.Description, task.Status, task.DueDate, task.CreatedAt, task.UpdatedAt);
}
