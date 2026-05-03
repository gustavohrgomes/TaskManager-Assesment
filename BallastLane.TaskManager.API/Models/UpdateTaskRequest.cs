namespace BallastLane.TaskManager.API.Models;

public sealed record UpdateTaskRequest(string Title, string? Description, DateTimeOffset? DueDate, string Status);
