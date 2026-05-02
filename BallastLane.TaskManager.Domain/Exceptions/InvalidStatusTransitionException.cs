namespace BallastLane.TaskManager.Domain.Exceptions;

public sealed class InvalidStatusTransitionException : Exception
{
    public InvalidStatusTransitionException(Tasks.TaskStatus from, Tasks.TaskStatus to)
        : base($"Cannot transition task from {from} to {to}.")
    {
        From = from;
        To = to;
    }

    public Tasks.TaskStatus From { get; }
    public Tasks.TaskStatus To { get; }
}
