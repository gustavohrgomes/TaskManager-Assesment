namespace BallastLane.TaskManager.Domain.Exceptions;

public sealed class TaskNotOwnedByUserException : Exception
{
    public TaskNotOwnedByUserException(Guid taskId, Guid userId)
        : base("Task not found.")
    {
        TaskId = taskId;
        UserId = userId;
    }

    public Guid TaskId { get; }
    public Guid UserId { get; }
}
