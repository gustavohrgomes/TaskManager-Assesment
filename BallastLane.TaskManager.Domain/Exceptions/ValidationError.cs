namespace BallastLane.TaskManager.Domain.Exceptions;

public sealed record ValidationError(string PropertyName, string ErrorMessage);
