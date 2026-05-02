using BallastLane.TaskManager.Common;
using BallastLane.TaskManager.Tasks;

namespace BallastLane.TaskManager.Abstractions;

/// <summary>
/// Persistence boundary for <see cref="TaskItem"/> aggregates. Read methods are scoped by owner so that
/// cross-user reads cannot leak through repository misuse.
/// </summary>
public interface ITaskRepository
{
    /// <summary>Inserts a new task into the underlying store.</summary>
    /// <param name="task">Task aggregate to persist.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    Task AddAsync(TaskItem task, CancellationToken ct);

    /// <summary>
    /// Looks up a task by its identifier, restricted to a specific owner so other users cannot retrieve it.
    /// </summary>
    /// <param name="id">Identifier of the task to fetch.</param>
    /// <param name="ownerId">Identifier of the owner the task must belong to.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>The task if found and owned by <paramref name="ownerId"/>; otherwise <c>null</c>.</returns>
    Task<TaskItem?> GetByIdAsync(Guid id, Guid ownerId, CancellationToken ct);

    /// <summary>
    /// Returns a paged slice of tasks matching the supplied query (owner, status, due-before, sort, paging).
    /// </summary>
    /// <param name="query">Filtering, paging, and sorting parameters.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>The matching items together with total count and paging metadata.</returns>
    Task<PagedResult<TaskItem>> ListAsync(TaskListQuery query, CancellationToken ct);

    /// <summary>Persists changes made to an existing task.</summary>
    /// <param name="task">Task aggregate whose updated state should be saved.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    Task UpdateAsync(TaskItem task, CancellationToken ct);

    /// <summary>
    /// Deletes a task identified by id, scoped by owner so cross-user deletes silently no-op rather than succeed.
    /// </summary>
    /// <param name="id">Identifier of the task to delete.</param>
    /// <param name="ownerId">Identifier of the owner the task must belong to.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    Task DeleteAsync(Guid id, Guid ownerId, CancellationToken ct);
}
