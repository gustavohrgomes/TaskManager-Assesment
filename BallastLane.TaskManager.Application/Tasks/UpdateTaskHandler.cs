using BallastLane.TaskManager.Abstractions;
using FluentValidation;

namespace BallastLane.TaskManager.Tasks;

/// <summary>
/// Application-layer handler that validates an <see cref="UpdateTaskCommand"/>, applies the changes
/// to a task owned by the current user, and persists the result inside a unit-of-work transaction.
/// </summary>
public sealed class UpdateTaskHandler
{
    public UpdateTaskHandler(
        IValidator<UpdateTaskCommand> validator,
        ITaskRepository tasks,
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        TimeProvider timeProvider)
    {
    }

    /// <summary>
    /// Updates the specified task with the supplied details.
    /// </summary>
    /// <param name="command">Task identifier together with the new field values.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>A projection of the updated task.</returns>
    public Task<TaskResult> Handle(UpdateTaskCommand command, CancellationToken ct) =>
        throw new NotImplementedException("See Phase 3.");
}
