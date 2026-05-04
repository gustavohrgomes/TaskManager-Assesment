using BallastLane.TaskManager.Exceptions;
using FluentValidation;

namespace BallastLane.TaskManager.Common;

public static class ValidatorExtensions
{
    public static void ValidateOrThrow<T>(this IValidator<T> validator, T instance)
    {
        var result = validator.Validate(instance);
        if (result.IsValid)
            return;

        var errors = result.Errors
            .Select(f => new ValidationError(f.PropertyName, f.ErrorMessage))
            .ToList();

        throw new DomainValidationException(errors);
    }
}
