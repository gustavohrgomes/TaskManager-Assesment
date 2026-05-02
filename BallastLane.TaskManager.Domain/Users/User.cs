using BallastLane.TaskManager.Exceptions;

namespace BallastLane.TaskManager.Users;

/// <summary>
/// Authentication aggregate that owns tasks. Construction is funneled through <see cref="Register"/>
/// so an instance always carries a valid email and a hashed password.
/// </summary>
public sealed class User
{
    /// <summary>Unique identifier assigned to the user at registration.</summary>
    public Guid Id { get; }
    /// <summary>The user's normalized email address, used as the login credential.</summary>
    public EmailAddress Email { get; }
    /// <summary>Hashed representation of the user's password. Plaintext passwords are never stored.</summary>
    public string PasswordHash { get; private set; }
    /// <summary>UTC timestamp at which the user was first registered.</summary>
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

    private User(Guid id, EmailAddress email, string passwordHash, DateTimeOffset createdAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Creates a brand-new <see cref="User"/> with a freshly generated identifier.
    /// </summary>
    /// <param name="email">Validated email value object that will become the login credential.</param>
    /// <param name="passwordHash">Pre-hashed password. The domain refuses to accept plaintext.</param>
    /// <param name="now">UTC timestamp to record as the registration moment.</param>
    /// <returns>A new <see cref="User"/> instance representing the registered account.</returns>
    public static User Register(EmailAddress email, string passwordHash, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(email, nameof(email));
        DomainValidationException.ThrowIfNullOrWhiteSpace(passwordHash, nameof(passwordHash));

        return new User(
            id: Guid.NewGuid(),
            email: email,
            passwordHash: passwordHash,
            createdAt: now);
    }
}
