using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Common;

namespace BallastLane.TaskManager.Tasks.GetTasks;

/// <summary>
/// Application-layer handler that returns a paged, filtered, sorted view of the current user's tasks.
/// </summary>
public sealed class GetTasksHandler
{
    private readonly ITaskRepository _tasks;
    private readonly IUserContext _userContext;

    public GetTasksHandler(ITaskRepository tasks, IUserContext userContext)
    {
        _tasks = tasks;
        _userContext = userContext;
    }

    /// <summary>
    /// Retrieves a page of the current user's tasks matching the supplied query.
    /// </summary>
    /// <param name="query">Filtering, paging, and sorting parameters.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A page of task projections together with paging metadata.</returns>
    public async Task<PagedResult<TaskResult>> Handle(TaskListQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query, nameof(query));

        var scoped = query with { OwnerId = _userContext.UserId };
        var result = await _tasks.ListAsync(scoped, cancellationToken);

        return new PagedResult<TaskResult>(
            [.. result.Items.Select(TaskResult.From)],
            result.TotalCount,
            result.Page,
            result.PageSize);
    }
}
