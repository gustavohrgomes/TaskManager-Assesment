namespace BallastLane.TaskManager.Tasks;

/// <summary>
/// Input for fetching a single task by its identifier.
/// </summary>
/// <param name="TaskId">Identifier of the task to retrieve.</param>
public sealed record GetTaskQuery(Guid TaskId);
