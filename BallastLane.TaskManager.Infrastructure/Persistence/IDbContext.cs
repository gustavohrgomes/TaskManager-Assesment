using Npgsql;

namespace BallastLane.TaskManager.Persistence;

internal interface IDbContext
{
    Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken ct);
}
