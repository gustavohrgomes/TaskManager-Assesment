namespace BallastLane.TaskManager.Common;

/// <summary>
/// Read-side query parameters for listing tasks: scoping, filtering, sorting, and paging.
/// </summary>
/// <param name="OwnerId">Identifier of the user whose tasks should be returned.</param>
/// <param name="Page">1-based page number to retrieve.</param>
/// <param name="PageSize">Maximum number of items per page.</param>
/// <param name="Status">Optional status filter; when null, tasks of any status are returned.</param>
/// <param name="DueBefore">Optional UTC cutoff; when set, only tasks with a due date earlier than this are returned.</param>
/// <param name="Sort">Field to sort by.</param>
/// <param name="Order">Direction of the sort.</param>
public sealed record TaskListQuery(
    Guid OwnerId,
    int Page,
    int PageSize,
    Tasks.TaskStatus? Status,
    DateTimeOffset? DueBefore,
    TaskSortField Sort,
    SortOrder Order);
