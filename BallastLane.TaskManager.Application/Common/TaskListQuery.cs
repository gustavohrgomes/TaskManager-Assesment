namespace BallastLane.TaskManager.Application.Common;

public sealed record TaskListQuery(
    Guid OwnerId,
    int Page,
    int PageSize,
    BallastLane.TaskManager.Domain.Tasks.TaskStatus? Status,
    DateTimeOffset? DueBefore,
    TaskSortField Sort,
    SortOrder Order);
