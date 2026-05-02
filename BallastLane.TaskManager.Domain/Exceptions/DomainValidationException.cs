namespace BallastLane.TaskManager.Domain.Exceptions;

public sealed class DomainValidationException : Exception
{
    public IReadOnlyList<ValidationError> Errors { get; }

    public DomainValidationException(IReadOnlyList<ValidationError> errors)
        : base("One or more validation rules failed.") => Errors = errors;

    public DomainValidationException(string propertyName, string errorMessage)
        : this(new[] { new ValidationError(propertyName, errorMessage) })
    {
    }
}
