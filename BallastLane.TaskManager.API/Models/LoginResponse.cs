namespace BallastLane.TaskManager.Models;

public sealed record LoginResponse(string Token, DateTimeOffset ExpiresAt);
