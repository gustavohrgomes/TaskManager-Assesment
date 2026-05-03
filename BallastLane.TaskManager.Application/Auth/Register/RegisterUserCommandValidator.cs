using FluentValidation;

namespace BallastLane.TaskManager.Auth.Register;

/// <summary>
/// FluentValidation rules for <see cref="RegisterUserCommand"/>: requires a syntactically valid email
/// and a password of at least 8 characters.
/// </summary>
public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}
