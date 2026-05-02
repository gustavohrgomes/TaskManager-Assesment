using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Exceptions;
using FluentValidation;

using DomainTaskStatus = BallastLane.TaskManager.Tasks.TaskStatus;

namespace BallastLane.TaskManager.Tasks;

/// <summary>
/// Application-layer handler that validates an <see cref="UpdateTaskCommand"/>, applies the changes
/// to a task owned by the current user, and persists the result.
/// </summary>
public sealed class UpdateTaskHandler
{
    private readonly IValidator<UpdateTaskCommand> _validator;
    private readonly ITaskRepository _tasks;
    private readonly IUserContext _userContext;
    private readonly TimeProvider _timeProvider;

    public UpdateTaskHandler(
        IValidator<UpdateTaskCommand> validator,
        ITaskRepository tasks,
        IUserContext userContext,
        TimeProvider timeProvider)
    {
        _validator = validator;
        _tasks = tasks;
        _userContext = userContext;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Updates the specified task with the supplied details.
    /// </summary>
    /// <param name="command">Task identifier together with the new field values.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>A projection of the updated task.</returns>
    public async Task<TaskResult> Handle(UpdateTaskCommand command, CancellationToken ct)
    {
        var validation = _validator.Validate(command);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .Select(f => new ValidationError(f.PropertyName, f.ErrorMessage))
                .ToList();
            throw new DomainValidationException(errors);
        }

        var task = await _tasks.GetByIdAsync(command.TaskId, _userContext.UserId, ct);
        if (task is null)
            throw new TaskNotOwnedByUserException(command.TaskId, _userContext.UserId);

        var now = _timeProvider.GetUtcNow();
        task.UpdateDetails(command.Title, command.Description, command.DueDate, now);

        if (command.Status != task.Status)
        {
            if (command.Status == DomainTaskStatus.InProgress)
                task.Start(now);
            else if (command.Status == DomainTaskStatus.Completed)
                task.Complete(now);
        }

        await _tasks.UpdateAsync(task, ct);
        return TaskResult.From(task);
    }
}
