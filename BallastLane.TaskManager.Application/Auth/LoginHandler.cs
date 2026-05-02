using BallastLane.TaskManager.Application.Abstractions;
using BallastLane.TaskManager.Application.Common;
using FluentValidation;

namespace BallastLane.TaskManager.Application.Auth;

public sealed class LoginHandler
{
    public LoginHandler(
        IValidator<LoginCommand> validator,
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IJwtTokenIssuer tokenIssuer)
    {
    }

    public Task<IssuedToken> Handle(LoginCommand command, CancellationToken ct) =>
        throw new NotImplementedException("See Phase 3.");
}
