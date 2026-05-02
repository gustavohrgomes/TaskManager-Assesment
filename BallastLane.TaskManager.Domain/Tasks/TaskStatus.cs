namespace BallastLane.TaskManager.Tasks;

/// <summary>
/// Lifecycle state of a task: not yet started, actively being worked on, or finished.
/// </summary>
public enum TaskStatus
{
    /// <summary>Task has been created but no work has begun.</summary>
    Pending = 0,
    /// <summary>Task has been started and is actively being worked on.</summary>
    InProgress = 1,
    /// <summary>Task has been finished and requires no further work.</summary>
    Completed = 2,
}
