using Npgsql;

namespace BallastLane.TaskManager.Persistence;

internal sealed class NpgsqlDbContext : IDbContext
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlDbContext(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken ct)
    {
        return await _dataSource.OpenConnectionAsync(ct);
    }
}
