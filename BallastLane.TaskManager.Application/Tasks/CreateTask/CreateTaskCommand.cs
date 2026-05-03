namespace BallastLane.TaskManager.Tasks.CreateTask;

/// <summary>
/// Input for creating a new task on behalf of the current user.
/// </summary>
/// <param name="Title">Required short title for the task.</param>
/// <param name="Description">Optional long-form description.</param>
/// <param name="DueDate">Optional UTC due date; must be in the future when supplied.</param>
public sealed record CreateTaskCommand(
    string Title,
    string? Description,
    DateTimeOffset? DueDate);
