namespace BallastLane.TaskManager.Tasks;

/// <summary>
/// Input for updating an existing task. All fields except <paramref name="TaskId"/> overwrite the
/// task's current values.
/// </summary>
/// <param name="TaskId">Identifier of the task to update.</param>
/// <param name="Title">New required short title.</param>
/// <param name="Description">New optional description.</param>
/// <param name="DueDate">New optional UTC due date.</param>
/// <param name="Status">New lifecycle status; must be reachable from the current state.</param>
public sealed record UpdateTaskCommand(
    Guid TaskId,
    string Title,
    string? Description,
    DateTimeOffset? DueDate,
    TaskStatus Status);
