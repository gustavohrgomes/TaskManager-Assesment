namespace BallastLane.TaskManager.API.Models;

public sealed record CreateTaskRequest(string Title, string? Description, DateTimeOffset? DueDate);
