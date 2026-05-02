using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Exceptions;
using FluentValidation;

namespace BallastLane.TaskManager.Tasks;

/// <summary>
/// Application-layer handler that validates a <see cref="CreateTaskCommand"/>, builds the domain aggregate,
/// and persists it inside a single unit-of-work transaction owned by the current user.
/// </summary>
public sealed class CreateTaskHandler
{
    private readonly IValidator<CreateTaskCommand> _validator;
    private readonly ITaskRepository _tasks;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly TimeProvider _timeProvider;

    public CreateTaskHandler(
        IValidator<CreateTaskCommand> validator,
        ITaskRepository tasks,
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        TimeProvider timeProvider)
    {
        _validator = validator;
        _tasks = tasks;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Creates a new task owned by the authenticated user.
    /// </summary>
    /// <param name="command">Caller-supplied task fields.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>A read-side projection of the newly persisted task.</returns>
    public async Task<TaskResult> Handle(CreateTaskCommand command, CancellationToken ct)
    {
        var validation = _validator.Validate(command);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .Select(f => new ValidationError(f.PropertyName, f.ErrorMessage))
                .ToList();
            throw new DomainValidationException(errors);
        }

        return await _unitOfWork.ExecuteInTransactionAsync(async innerCt =>
        {
            var task = TaskItem.Create(
                ownerId: _userContext.UserId,
                title: command.Title,
                description: command.Description,
                dueDate: command.DueDate,
                now: _timeProvider.GetUtcNow());

            await _tasks.AddAsync(task, innerCt);
            return TaskResult.From(task);
        }, ct);
    }
}
