using BallastLane.TaskManager.Application.Abstractions;
using FluentValidation;

namespace BallastLane.TaskManager.Application.Tasks;

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

    public Task<TaskResult> Handle(UpdateTaskCommand command, CancellationToken ct) =>
        throw new NotImplementedException("See Phase 3.");
}
