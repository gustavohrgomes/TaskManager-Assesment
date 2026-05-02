using BallastLane.TaskManager.Abstractions;

namespace BallastLane.TaskManager.Tasks;

/// <summary>
/// Application-layer handler that deletes a task on behalf of the current user, enforcing ownership
/// before the delete is committed.
/// </summary>
public sealed class DeleteTaskHandler
{
    public DeleteTaskHandler(
        ITaskRepository tasks,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
    }

    /// <summary>
    /// Deletes the task identified by the command, provided it is owned by the current user.
    /// </summary>
    /// <param name="command">Identifier of the task to delete.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    public Task Handle(DeleteTaskCommand command, CancellationToken ct) =>
        throw new NotImplementedException("See Phase 3.");
}
