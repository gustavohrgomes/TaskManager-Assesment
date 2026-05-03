namespace BallastLane.TaskManager.API.Models;

public sealed record LoginResponse(string Token, DateTimeOffset ExpiresAt);
