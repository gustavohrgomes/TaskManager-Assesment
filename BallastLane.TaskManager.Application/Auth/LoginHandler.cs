using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Common;
using BallastLane.TaskManager.Exceptions;
using BallastLane.TaskManager.Users;
using FluentValidation;

namespace BallastLane.TaskManager.Auth;

/// <summary>
/// Application-layer handler for the login flow: validates the command, looks up the user by email,
/// verifies the password, and issues a JWT.
/// </summary>
public sealed class LoginHandler
{
    private readonly IValidator<LoginCommand> _validator;
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenIssuer _tokenIssuer;

    public LoginHandler(
        IValidator<LoginCommand> validator,
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IJwtTokenIssuer tokenIssuer)
    {
        _validator = validator;
        _users = users;
        _passwordHasher = passwordHasher;
        _tokenIssuer = tokenIssuer;
    }

    /// <summary>
    /// Authenticates the supplied credentials and produces a signed JWT.
    /// </summary>
    /// <param name="command">Email/password pair submitted by the caller.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>The issued token together with its expiration.</returns>
    public async Task<IssuedToken> Handle(LoginCommand command, CancellationToken ct)
    {
        var validation = _validator.Validate(command);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .Select(f => new ValidationError(f.PropertyName, f.ErrorMessage))
                .ToList();
            throw new DomainValidationException(errors);
        }

        var email = EmailAddress.From(command.Email);
        var user = await _users.GetByEmailAsync(email, ct);

        if (user is null)
        {
            _passwordHasher.Verify(_passwordHasher.Hash("dummy"), command.Password);
            throw new UnauthorizedException();
        }

        if (!_passwordHasher.Verify(user.PasswordHash, command.Password))
            throw new UnauthorizedException();

        return _tokenIssuer.Issue(user);
    }
}
