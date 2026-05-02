namespace BallastLane.TaskManager.Exceptions;

/// <summary>
/// Thrown when a task is asked to move between two statuses that the domain state machine does not allow.
/// </summary>
/// <param name="from">Current status of the task at the time of the attempted transition.</param>
/// <param name="to">Status the caller attempted to transition the task into.</param>
public sealed class InvalidStatusTransitionException(Tasks.TaskStatus from, Tasks.TaskStatus to) : Exception($"Cannot transition task from {from} to {to}.")
{
    /// <summary>Status the task was in when the invalid transition was requested.</summary>
    public Tasks.TaskStatus From { get; } = from;
    /// <summary>Status the caller attempted to transition the task into.</summary>
    public Tasks.TaskStatus To { get; } = to;
}
