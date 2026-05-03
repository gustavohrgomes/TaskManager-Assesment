namespace BallastLane.TaskManager.Models;

public sealed record UpdateTaskRequest(string Title, string? Description, DateTimeOffset? DueDate, string Status);
