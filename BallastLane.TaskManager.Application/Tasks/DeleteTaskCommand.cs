namespace BallastLane.TaskManager.Tasks;

/// <summary>
/// Input for deleting a task owned by the current user.
/// </summary>
/// <param name="TaskId">Identifier of the task to delete.</param>
public sealed record DeleteTaskCommand(Guid TaskId);
