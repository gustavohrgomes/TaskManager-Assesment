using BallastLane.TaskManager.Application.Abstractions;
using BallastLane.TaskManager.Application.Common;

namespace BallastLane.TaskManager.Application.Tasks;

public sealed class ListTasksHandler
{
    public ListTasksHandler(ITaskRepository tasks, IUserContext userContext)
    {
    }

    public Task<PagedResult<TaskResult>> Handle(TaskListQuery query, CancellationToken ct) =>
        throw new NotImplementedException("See Phase 3.");
}
