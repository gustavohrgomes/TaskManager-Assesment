using BallastLane.TaskManager.Domain.Users;

namespace BallastLane.TaskManager.Application.Abstractions;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken ct);
    Task<User?> GetByEmailAsync(EmailAddress email, CancellationToken ct);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
}
