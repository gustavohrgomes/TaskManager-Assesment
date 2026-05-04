using Npgsql;
using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Users;

namespace BallastLane.TaskManager.Persistence;

internal sealed class NpgsqlUserRepository : IUserRepository
{
    private readonly IDbContext _db;

    public NpgsqlUserRepository(IDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await using var conn = await _db.CreateConnectionAsync(cancellationToken);
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO users (id, email, password_hash, created_at) VALUES ($1, $2, $3, $4)", conn);
        cmd.Parameters.Add(new NpgsqlParameter<Guid> { TypedValue = user.Id });
        cmd.Parameters.Add(new NpgsqlParameter<string> { TypedValue = user.Email.Value });
        cmd.Parameters.Add(new NpgsqlParameter<string> { TypedValue = user.PasswordHash });
        cmd.Parameters.Add(new NpgsqlParameter<DateTimeOffset> { TypedValue = user.CreatedAt });
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(EmailAddress email, CancellationToken cancellationToken)
    {
        await using var conn = await _db.CreateConnectionAsync(cancellationToken);
        await using var cmd = new NpgsqlCommand(
            "SELECT id, email, password_hash, created_at FROM users WHERE email = $1", conn);
        cmd.Parameters.Add(new NpgsqlParameter<string> { TypedValue = email.Value });
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken)) return null;
        return ReadUser(reader);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var conn = await _db.CreateConnectionAsync(cancellationToken);
        await using var cmd = new NpgsqlCommand(
            "SELECT id, email, password_hash, created_at FROM users WHERE id = $1", conn);
        cmd.Parameters.Add(new NpgsqlParameter<Guid> { TypedValue = id });
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken)) return null;
        return ReadUser(reader);
    }

    private static User ReadUser(NpgsqlDataReader reader)
    {
        return new User(
            id: reader.GetGuid(0),
            email: EmailAddress.From(reader.GetString(1)),
            passwordHash: reader.GetString(2),
            createdAt: reader.GetFieldValue<DateTimeOffset>(3));
    }
}
