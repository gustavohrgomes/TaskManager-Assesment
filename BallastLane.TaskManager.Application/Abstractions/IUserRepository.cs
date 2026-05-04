using BallastLane.TaskManager.Users;

namespace BallastLane.TaskManager.Abstractions;

/// <summary>
/// Persistence boundary for <see cref="User"/> aggregates, used by registration and login flows.
/// </summary>
public interface IUserRepository
{
    /// <summary>Inserts a new user into the underlying store.</summary>
    /// <param name="user">User aggregate to persist.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task AddAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// Looks up a user by their normalized email address, used during login and duplicate-registration checks.
    /// </summary>
    /// <param name="email">Normalized email value object to look up.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching user if found; otherwise <c>null</c>.</returns>
    Task<User?> GetByEmailAsync(EmailAddress email, CancellationToken cancellationToken);

    /// <summary>Looks up a user by their identifier.</summary>
    /// <param name="id">Identifier of the user to fetch.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching user if found; otherwise <c>null</c>.</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
