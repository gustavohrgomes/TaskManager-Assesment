using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Common;
using BallastLane.TaskManager.Exceptions;
using BallastLane.TaskManager.Users;
using FluentValidation;

namespace BallastLane.TaskManager.Auth.Register;

/// <summary>
/// Application-layer handler for new-user registration: validates the command, hashes the password,
/// persists the user, and returns a summary of the created account.
/// </summary>
public sealed class RegisterUserHandler
{
    private readonly IValidator<RegisterUserCommand> _validator;
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly TimeProvider _timeProvider;

    public RegisterUserHandler(
        IValidator<RegisterUserCommand> validator,
        IUserRepository users,
        IPasswordHasher passwordHasher,
        TimeProvider timeProvider)
    {
        _validator = validator;
        _users = users;
        _passwordHasher = passwordHasher;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Registers a new user with the supplied credentials.
    /// </summary>
    /// <param name="command">Email and plaintext password for the new account.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The new user's identifier and email.</returns>
    public async Task<RegisterUserResult> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));

        _validator.ValidateOrThrow(command);

        var email = EmailAddress.From(command.Email);
        var existing = await _users.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
            throw new DomainValidationException("Email", "A user with this email already exists.");

        var hash = _passwordHasher.Hash(command.Password);
        var user = User.Register(email, hash, _timeProvider.GetUtcNow());
        await _users.AddAsync(user, cancellationToken);

        return new RegisterUserResult(user.Id, user.Email.Value);
    }
}
