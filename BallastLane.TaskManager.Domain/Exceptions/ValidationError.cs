namespace BallastLane.TaskManager.Exceptions;

/// <summary>
/// Single field-level validation failure, identifying the offending property and the human-readable reason.
/// </summary>
/// <param name="PropertyName">Name of the property that failed validation.</param>
/// <param name="ErrorMessage">Description of why the value was rejected.</param>
public sealed record ValidationError(string PropertyName, string ErrorMessage);
