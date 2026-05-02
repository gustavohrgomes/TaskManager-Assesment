namespace BallastLane.TaskManager.Exceptions;

/// <summary>
/// Aggregates one or more domain-level validation failures into a single throwable error so handlers
/// can surface every offending field in one response rather than failing fast on the first.
/// </summary>
public sealed class DomainValidationException : Exception
{
    /// <summary>The set of field-level validation failures that triggered this exception.</summary>
    public IReadOnlyList<ValidationError> Errors { get; }

    public DomainValidationException(IReadOnlyList<ValidationError> errors)
        : base("One or more validation rules failed.") => Errors = errors;

    public DomainValidationException(string propertyName, string errorMessage)
        : this([new ValidationError(propertyName, errorMessage)])
    {
    }

    /// <summary>
    /// Throws a <see cref="DomainValidationException"/> with a "{property} is required" message
    /// when the supplied value is null, empty, or whitespace.
    /// </summary>
    /// <param name="value">String value to validate.</param>
    /// <param name="propertyName">Name of the property the value belongs to, used in the error message.</param>
    public static void ThrowIfNullOrWhiteSpace(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException(propertyName, $"{propertyName} is required.");
    }
}
