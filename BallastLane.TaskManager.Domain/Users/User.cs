using BallastLane.TaskManager.Domain.Exceptions;

namespace BallastLane.TaskManager.Domain.Users;

public sealed class User
{
    public Guid Id { get; }
    public EmailAddress Email { get; }
    public string PasswordHash { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    private User(Guid id, EmailAddress email, string passwordHash, DateTimeOffset createdAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public static User Register(EmailAddress email, string passwordHash, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainValidationException(nameof(PasswordHash), "Password hash is required.");

        return new User(
            id: Guid.NewGuid(),
            email: email,
            passwordHash: passwordHash,
            createdAt: now);
    }
}
