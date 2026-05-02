using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Common;
using FluentValidation;

namespace BallastLane.TaskManager.Auth;

/// <summary>
/// Application-layer handler for the login flow: validates the command, looks up the user by email,
/// verifies the password, and issues a JWT.
/// </summary>
public sealed class LoginHandler
{
    public LoginHandler(
        IValidator<LoginCommand> validator,
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IJwtTokenIssuer tokenIssuer)
    {
    }

    /// <summary>
    /// Authenticates the supplied credentials and produces a signed JWT.
    /// </summary>
    /// <param name="command">Email/password pair submitted by the caller.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>The issued token together with its expiration.</returns>
    public Task<IssuedToken> Handle(LoginCommand command, CancellationToken ct) =>
        throw new NotImplementedException("See Phase 3.");
}
