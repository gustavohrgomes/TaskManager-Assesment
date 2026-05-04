using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Common;
using FluentValidation;

namespace BallastLane.TaskManager.Tasks.CreateTask;

/// <summary>
/// Application-layer handler that validates a <see cref="CreateTaskCommand"/>, builds the domain aggregate,
/// and persists it via the task repository on behalf of the current user.
/// </summary>
public sealed class CreateTaskHandler
{
    private readonly IValidator<CreateTaskCommand> _validator;
    private readonly ITaskRepository _tasks;
    private readonly IUserContext _userContext;
    private readonly TimeProvider _timeProvider;

    public CreateTaskHandler(
        IValidator<CreateTaskCommand> validator,
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
    /// Creates a new task owned by the authenticated user.
    /// </summary>
    /// <param name="command">Caller-supplied task fields.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A read-side projection of the newly persisted task.</returns>
    public async Task<TaskResult> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));

        _validator.ValidateOrThrow(command);

        var task = TaskItem.Create(
            ownerId: _userContext.UserId,
            title: command.Title,
            description: command.Description,
            dueDate: command.DueDate,
            now: _timeProvider.GetUtcNow());

        await _tasks.AddAsync(task, cancellationToken);
        return TaskResult.From(task);
    }
}
