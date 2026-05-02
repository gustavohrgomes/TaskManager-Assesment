namespace BallastLane.TaskManager.Common;

/// <summary>
/// A single page of items together with the metadata needed to render paging controls.
/// </summary>
/// <typeparam name="T">Element type of the page.</typeparam>
/// <param name="Items">Items belonging to the current page.</param>
/// <param name="TotalCount">Total number of items matching the underlying query, across all pages.</param>
/// <param name="Page">1-based page number this result represents.</param>
/// <param name="PageSize">Maximum number of items per page that was used to compute this slice.</param>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize);
