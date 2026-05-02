using System.Text;
using Npgsql;
using NpgsqlTypes;
using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Common;
using BallastLane.TaskManager.Tasks;

using DomainTaskStatus = BallastLane.TaskManager.Tasks.TaskStatus;

namespace BallastLane.TaskManager.Persistence;

internal sealed class NpgsqlTaskRepository : ITaskRepository
{
    private readonly IDbContext _db;

    public NpgsqlTaskRepository(IDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(TaskItem task, CancellationToken ct)
    {
        await using var conn = await _db.CreateConnectionAsync(ct);
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO tasks (id, owner_id, title, description, status, due_date, created_at, updated_at) VALUES ($1, $2, $3, $4, $5, $6, $7, $8)", conn);
        cmd.Parameters.Add(new NpgsqlParameter<Guid> { TypedValue = task.TaskItemId });
        cmd.Parameters.Add(new NpgsqlParameter<Guid> { TypedValue = task.OwnerId });
        cmd.Parameters.Add(new NpgsqlParameter<string> { TypedValue = task.Title });
        cmd.Parameters.Add(new NpgsqlParameter { Value = (object?)task.Description ?? DBNull.Value, NpgsqlDbType = NpgsqlDbType.Text });
        cmd.Parameters.Add(new NpgsqlParameter<short> { TypedValue = (short)task.Status });
        cmd.Parameters.Add(new NpgsqlParameter { Value = task.DueDate.HasValue ? (object)task.DueDate.Value : DBNull.Value, NpgsqlDbType = NpgsqlDbType.TimestampTz });
        cmd.Parameters.Add(new NpgsqlParameter<DateTimeOffset> { TypedValue = task.CreatedAt });
        cmd.Parameters.Add(new NpgsqlParameter<DateTimeOffset> { TypedValue = task.UpdatedAt });
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, Guid ownerId, CancellationToken ct)
    {
        await using var conn = await _db.CreateConnectionAsync(ct);
        await using var cmd = new NpgsqlCommand(
            "SELECT id, owner_id, title, description, status, due_date, created_at, updated_at FROM tasks WHERE id = $1 AND owner_id = $2", conn);
        cmd.Parameters.Add(new NpgsqlParameter<Guid> { TypedValue = id });
        cmd.Parameters.Add(new NpgsqlParameter<Guid> { TypedValue = ownerId });
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;
        return ReadTask(reader);
    }

    public async Task<PagedResult<TaskItem>> ListAsync(TaskListQuery query, CancellationToken ct)
    {
        await using var conn = await _db.CreateConnectionAsync(ct);

        var whereClauses = new StringBuilder("WHERE owner_id = $1");
        var parameters = new List<NpgsqlParameter>
        {
            new NpgsqlParameter<Guid> { TypedValue = query.OwnerId }
        };
        var paramIndex = 2;

        if (query.Status.HasValue)
        {
            whereClauses.Append($" AND status = ${paramIndex}");
            parameters.Add(new NpgsqlParameter<short> { TypedValue = (short)query.Status.Value });
            paramIndex++;
        }

        if (query.DueBefore.HasValue)
        {
            whereClauses.Append($" AND due_date < ${paramIndex}");
            parameters.Add(new NpgsqlParameter<DateTimeOffset> { TypedValue = query.DueBefore.Value });
            paramIndex++;
        }

        var countSql = $"SELECT COUNT(*) FROM tasks {whereClauses}";
        await using var countCmd = new NpgsqlCommand(countSql, conn);
        foreach (var p in parameters)
            countCmd.Parameters.Add(CloneParameter(p));
        var totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync(ct));

        var sortColumn = ToColumnName(query.Sort);
        var sortDirection = ToDirection(query.Order);
        var dataSql = $"SELECT id, owner_id, title, description, status, due_date, created_at, updated_at FROM tasks {whereClauses} ORDER BY {sortColumn} {sortDirection} LIMIT ${paramIndex} OFFSET ${paramIndex + 1}";

        await using var dataCmd = new NpgsqlCommand(dataSql, conn);
        foreach (var p in parameters)
            dataCmd.Parameters.Add(CloneParameter(p));
        dataCmd.Parameters.Add(new NpgsqlParameter<int> { TypedValue = query.PageSize });
        dataCmd.Parameters.Add(new NpgsqlParameter<int> { TypedValue = (query.Page - 1) * query.PageSize });

        var items = new List<TaskItem>();
        await using var reader = await dataCmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
            items.Add(ReadTask(reader));

        return new PagedResult<TaskItem>(items, totalCount, query.Page, query.PageSize);
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken ct)
    {
        await using var conn = await _db.CreateConnectionAsync(ct);
        await using var cmd = new NpgsqlCommand(
            "UPDATE tasks SET title = $1, description = $2, status = $3, due_date = $4, updated_at = $5 WHERE id = $6 AND owner_id = $7", conn);
        cmd.Parameters.Add(new NpgsqlParameter<string> { TypedValue = task.Title });
        cmd.Parameters.Add(new NpgsqlParameter { Value = (object?)task.Description ?? DBNull.Value, NpgsqlDbType = NpgsqlDbType.Text });
        cmd.Parameters.Add(new NpgsqlParameter<short> { TypedValue = (short)task.Status });
        cmd.Parameters.Add(new NpgsqlParameter { Value = task.DueDate.HasValue ? (object)task.DueDate.Value : DBNull.Value, NpgsqlDbType = NpgsqlDbType.TimestampTz });
        cmd.Parameters.Add(new NpgsqlParameter<DateTimeOffset> { TypedValue = task.UpdatedAt });
        cmd.Parameters.Add(new NpgsqlParameter<Guid> { TypedValue = task.TaskItemId });
        cmd.Parameters.Add(new NpgsqlParameter<Guid> { TypedValue = task.OwnerId });
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task DeleteAsync(Guid id, Guid ownerId, CancellationToken ct)
    {
        await using var conn = await _db.CreateConnectionAsync(ct);
        await using var cmd = new NpgsqlCommand(
            "DELETE FROM tasks WHERE id = $1 AND owner_id = $2", conn);
        cmd.Parameters.Add(new NpgsqlParameter<Guid> { TypedValue = id });
        cmd.Parameters.Add(new NpgsqlParameter<Guid> { TypedValue = ownerId });
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static string ToColumnName(TaskSortField field) => field switch
    {
        TaskSortField.CreatedAt => "created_at",
        TaskSortField.DueDate => "due_date",
        TaskSortField.Title => "title",
        TaskSortField.Status => "status",
        _ => "created_at"
    };

    private static string ToDirection(SortOrder order) => order switch
    {
        SortOrder.Desc => "DESC",
        _ => "ASC"
    };

    private static TaskItem ReadTask(NpgsqlDataReader reader)
    {
        return new TaskItem(
            taskItemId: reader.GetGuid(0),
            ownerId: reader.GetGuid(1),
            title: reader.GetString(2),
            description: reader.IsDBNull(3) ? null : reader.GetString(3),
            status: (DomainTaskStatus)reader.GetInt16(4),
            dueDate: reader.IsDBNull(5) ? null : reader.GetFieldValue<DateTimeOffset>(5),
            createdAt: reader.GetFieldValue<DateTimeOffset>(6),
            updatedAt: reader.GetFieldValue<DateTimeOffset>(7));
    }

    private static NpgsqlParameter CloneParameter(NpgsqlParameter source)
    {
        return new NpgsqlParameter
        {
            Value = source.Value ?? DBNull.Value,
            NpgsqlDbType = source.NpgsqlDbType,
        };
    }
}
