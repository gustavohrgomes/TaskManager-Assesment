namespace BallastLane.TaskManager.Exceptions;

/// <summary>
/// Thrown when a user attempts to access or mutate a task that belongs to a different owner.
/// The base message is intentionally generic so the API layer can map it to a 404 without leaking task existence.
/// </summary>
/// <param name="taskId">Identifier of the task whose ownership check failed.</param>
/// <param name="userId">Identifier of the caller that attempted the access.</param>
public sealed class TaskNotOwnedByUserException(Guid taskId, Guid userId) : Exception("Task not found.")
{
    /// <summary>Identifier of the task whose ownership check failed.</summary>
    public Guid TaskId { get; } = taskId;
    /// <summary>Identifier of the caller that attempted the access.</summary>
    public Guid UserId { get; } = userId;
}
