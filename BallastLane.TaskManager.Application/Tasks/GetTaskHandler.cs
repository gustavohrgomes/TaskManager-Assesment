using BallastLane.TaskManager.Application.Abstractions;

namespace BallastLane.TaskManager.Application.Tasks;

public sealed class GetTaskHandler
{
    public GetTaskHandler(ITaskRepository tasks, IUserContext userContext)
    {
    }

    public Task<TaskResult> Handle(GetTaskQuery query, CancellationToken ct) =>
        throw new NotImplementedException("See Phase 3.");
}
