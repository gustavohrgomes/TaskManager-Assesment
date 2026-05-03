using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Exceptions;

namespace BallastLane.TaskManager.Tasks.GetTasks;

/// <summary>
/// Application-layer handler that fetches a single task by id, scoped to the current user so that
/// other users' tasks surface as "not found" rather than leaking their existence.
/// </summary>
public sealed class GetTaskHandler
{
    private readonly ITaskRepository _tasks;
    private readonly IUserContext _userContext;

    public GetTaskHandler(ITaskRepository tasks, IUserContext userContext)
    {
        _tasks = tasks;
        _userContext = userContext;
    }

    /// <summary>
    /// Retrieves a task owned by the current user.
    /// </summary>
    /// <param name="query">Identifier of the task to retrieve.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>The task projection if found and owned by the caller.</returns>
    public async Task<TaskResult> Handle(GetTaskQuery query, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(query, nameof(query));

        var task = await _tasks.GetByIdAsync(query.TaskId, _userContext.UserId, ct);

        return task is null 
            ? throw new TaskNotOwnedByUserException(query.TaskId, _userContext.UserId) 
            : TaskResult.From(task);
    }
}
