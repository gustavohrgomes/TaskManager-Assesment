namespace BallastLane.TaskManager.API.Models;

public sealed record TaskListResponse(
    IReadOnlyList<TaskResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
