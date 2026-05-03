namespace BallastLane.TaskManager.Models;

public sealed record CreateTaskRequest(string Title, string? Description, DateTimeOffset? DueDate);
