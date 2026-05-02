using BallastLane.TaskManager.Abstractions;
using FluentValidation;

namespace BallastLane.TaskManager.Auth;

/// <summary>
/// Application-layer handler for new-user registration: validates the command, hashes the password,
/// persists the user, and returns a summary of the created account.
/// </summary>
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

    /// <summary>
    /// Registers a new user with the supplied credentials.
    /// </summary>
    /// <param name="command">Email and plaintext password for the new account.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>The new user's identifier and email.</returns>
    public Task<RegisterUserResult> Handle(RegisterUserCommand command, CancellationToken ct) =>
        throw new NotImplementedException("See Phase 3.");
}
