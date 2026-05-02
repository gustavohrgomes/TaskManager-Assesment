using BallastLane.TaskManager.Application.Abstractions;

namespace BallastLane.TaskManager.Application.Tasks;

public sealed class DeleteTaskHandler
{
    public DeleteTaskHandler(
        ITaskRepository tasks,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
    }

    public Task Handle(DeleteTaskCommand command, CancellationToken ct) =>
        throw new NotImplementedException("See Phase 3.");
}
