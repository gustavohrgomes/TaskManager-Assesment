namespace BallastLane.TaskManager.Common;

/// <summary>
/// Fields a task list can be sorted by.
/// </summary>
public enum TaskSortField
{
    /// <summary>Sort by the task's creation timestamp.</summary>
    CreatedAt = 0,
    /// <summary>Sort by the task's due date (tasks without a due date sort last in ascending order).</summary>
    DueDate = 1,
    /// <summary>Sort alphabetically by title.</summary>
    Title = 2,
    /// <summary>Sort by lifecycle status.</summary>
    Status = 3,
}
