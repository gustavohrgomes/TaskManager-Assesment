using FluentValidation;

namespace BallastLane.TaskManager.Auth;

/// <summary>
/// FluentValidation rules for <see cref="LoginCommand"/>: both the email and password fields must be supplied.
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
