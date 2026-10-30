using Microsoft.Data.Sqlite;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio ADO.NET de la bitácora append-only.
/// </summary>
public sealed class LogRepository : ILogRepository
{
    private const string SelectColumns =
        "Id, Timestamp, Module, Action, TargetKey, Result, Detail, BackupId";

    private readonly SokDbContext _context;

    public LogRepository(SokDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<int> AddAsync(LogEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentException.ThrowIfNullOrWhiteSpace(entry.Action);

        entry.Timestamp = entry.Timestamp == default ? DateTime.Now : entry.Timestamp;

        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Logs (Timestamp, Module, Action, TargetKey, Result, Detail, BackupId)
            VALUES (@Timestamp, @Module, @Action, @TargetKey, @Result, @Detail, @BackupId);
            SELECT last_insert_rowid();
            """;

        command.Parameters.AddWithValue("@Timestamp", SqliteMapping.ToDb(entry.Timestamp));
        command.Parameters.AddWithValue("@Module", entry.Module.ToString());
        command.Parameters.AddWithValue("@Action", entry.Action.Trim());
        command.Parameters.AddWithValue("@TargetKey", (object?)SqliteMapping.NullableString(entry.TargetKey) ?? DBNull.Value);
        command.Parameters.AddWithValue("@Result", entry.Result.ToString());
        command.Parameters.AddWithValue("@Detail", (object?)SqliteMapping.NullableString(entry.Detail) ?? DBNull.Value);
        command.Parameters.AddWithValue("@BackupId", (object?)entry.BackupId ?? DBNull.Value);

        var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToInt32(result);
    }

    public Task<IReadOnlyList<LogEntry>> GetRecentAsync(int limit = 100, CancellationToken cancellationToken = default)
    {
        return QueryAsync(
            $"SELECT {SelectColumns} FROM Logs ORDER BY Timestamp DESC, Id DESC LIMIT @Limit;",
            null,
            limit,
            cancellationToken);
    }

    public Task<IReadOnlyList<LogEntry>> GetByModuleAsync(LogModule module, int limit = 100, CancellationToken cancellationToken = default)
    {
        return QueryAsync(
            $"SELECT {SelectColumns} FROM Logs WHERE Module = @Module ORDER BY Timestamp DESC, Id DESC LIMIT @Limit;",
            module.ToString(),
            limit,
            cancellationToken);
    }

    private async Task<IReadOnlyList<LogEntry>> QueryAsync(
        string sql,
        string? module,
        int limit,
        CancellationToken cancellationToken)
    {
        var entries = new List<LogEntry>();

        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@Limit", Math.Clamp(limit, 1, 10_000));

        if (module is not null)
        {
            command.Parameters.AddWithValue("@Module", module);
        }

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            entries.Add(Map(reader));
        }

        return entries;
    }

    private static LogEntry Map(SqliteDataReader reader) =>
        new()
        {
            Id = reader.GetInt32(0),
            Timestamp = SqliteMapping.FromDb(reader.GetString(1)),
            Module = Enum.Parse<LogModule>(reader.GetString(2)),
            Action = reader.GetString(3),
            TargetKey = reader.IsDBNull(4) ? null : reader.GetString(4),
            Result = Enum.Parse<ActionResult>(reader.GetString(5)),
            Detail = reader.IsDBNull(6) ? null : reader.GetString(6),
            BackupId = reader.IsDBNull(7) ? null : reader.GetInt32(7)
        };
}
