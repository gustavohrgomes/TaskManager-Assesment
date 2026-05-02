using BallastLane.TaskManager.Abstractions;

namespace BallastLane.TaskManager.Tasks;

/// <summary>
/// Application-layer handler that fetches a single task by id, scoped to the current user so that
/// other users' tasks surface as "not found" rather than leaking their existence.
/// </summary>
public sealed class GetTaskHandler
{
    public GetTaskHandler(ITaskRepository tasks, IUserContext userContext)
    {
    }

    /// <summary>
    /// Retrieves a task owned by the current user.
    /// </summary>
    /// <param name="query">Identifier of the task to retrieve.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>The task projection if found and owned by the caller.</returns>
    public Task<TaskResult> Handle(GetTaskQuery query, CancellationToken ct) =>
        throw new NotImplementedException("See Phase 3.");
}
