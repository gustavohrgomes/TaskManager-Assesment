using BallastLane.TaskManager.Abstractions;

namespace BallastLane.TaskManager.Tasks.DeleteTask;

/// <summary>
/// Application-layer handler that deletes a task on behalf of the current user, enforcing ownership
/// via the repository's WHERE clause.
/// </summary>
public sealed class DeleteTaskHandler
{
    private readonly ITaskRepository _tasks;
    private readonly IUserContext _userContext;

    public DeleteTaskHandler(ITaskRepository tasks, IUserContext userContext)
    {
        _tasks = tasks;
        _userContext = userContext;
    }

    /// <summary>
    /// Deletes the task identified by the command, provided it is owned by the current user.
    /// </summary>
    /// <param name="command">Identifier of the task to delete.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    public async Task Handle(DeleteTaskCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));

        await _tasks.DeleteAsync(command.TaskId, _userContext.UserId, ct);
    }
}
