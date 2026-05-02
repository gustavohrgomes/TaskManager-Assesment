using BallastLane.TaskManager.Application.Common;
using BallastLane.TaskManager.Domain.Tasks;

namespace BallastLane.TaskManager.Application.Abstractions;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task, CancellationToken ct);
    Task<TaskItem?> GetByIdAsync(Guid id, Guid ownerId, CancellationToken ct);
    Task<PagedResult<TaskItem>> ListAsync(TaskListQuery query, CancellationToken ct);
    Task UpdateAsync(TaskItem task, CancellationToken ct);
    Task DeleteAsync(Guid id, Guid ownerId, CancellationToken ct);
}
