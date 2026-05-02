using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Common;

namespace BallastLane.TaskManager.Tasks;

/// <summary>
/// Application-layer handler that returns a paged, filtered, sorted view of the current user's tasks.
/// </summary>
public sealed class ListTasksHandler
{
    public ListTasksHandler(ITaskRepository tasks, IUserContext userContext)
    {
    }

    /// <summary>
    /// Retrieves a page of the current user's tasks matching the supplied query.
    /// </summary>
    /// <param name="query">Filtering, paging, and sorting parameters.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>A page of task projections together with paging metadata.</returns>
    public Task<PagedResult<TaskResult>> Handle(TaskListQuery query, CancellationToken ct) =>
        throw new NotImplementedException("See Phase 3.");
}
