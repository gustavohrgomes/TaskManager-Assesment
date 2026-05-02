using BallastLane.TaskManager.Application.Abstractions;
using FluentValidation;

namespace BallastLane.TaskManager.Application.Auth;

public sealed class RegisterUserHandler
{
    public RegisterUserHandler(
        IValidator<RegisterUserCommand> validator,
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
    }

    public Task<RegisterUserResult> Handle(RegisterUserCommand command, CancellationToken ct) =>
        throw new NotImplementedException("See Phase 3.");
}
