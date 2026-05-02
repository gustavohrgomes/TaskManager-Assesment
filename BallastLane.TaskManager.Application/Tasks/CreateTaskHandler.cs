using BallastLane.TaskManager.Application.Abstractions;
using BallastLane.TaskManager.Domain.Exceptions;
using BallastLane.TaskManager.Domain.Tasks;
using FluentValidation;

namespace BallastLane.TaskManager.Application.Tasks;

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
