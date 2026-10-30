using Microsoft.Data.Sqlite;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio ADO.NET de metadatos de respaldos.
/// </summary>
public sealed class BackupRepository : IBackupRepository
{
    private const string SelectColumns =
        "Id, Name, BackupType, FilePath, HashSha256, Description, CreatedAt";

    private readonly SokDbContext _context;

    public BackupRepository(SokDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IReadOnlyList<Backup>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var backups = new List<Backup>();

        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = $"SELECT {SelectColumns} FROM Backups ORDER BY CreatedAt DESC, Id DESC;";

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            backups.Add(Map(reader));
        }

        return backups;
    }

    public async Task<Backup?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = $"SELECT {SelectColumns} FROM Backups WHERE Id = @Id;";
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        return await reader.ReadAsync(cancellationToken).ConfigureAwait(false) ? Map(reader) : null;
    }

    public async Task<int> AddAsync(Backup backup, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(backup);
        ArgumentException.ThrowIfNullOrWhiteSpace(backup.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(backup.FilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(backup.HashSha256);

        backup.CreatedAt = backup.CreatedAt == default ? DateTime.Now : backup.CreatedAt;

        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Backups (Name, BackupType, FilePath, HashSha256, Description, CreatedAt)
            VALUES (@Name, @BackupType, @FilePath, @HashSha256, @Description, @CreatedAt);
            SELECT last_insert_rowid();
            """;

        command.Parameters.AddWithValue("@Name", backup.Name.Trim());
        command.Parameters.AddWithValue("@BackupType", backup.BackupType.ToString());
        command.Parameters.AddWithValue("@FilePath", backup.FilePath.Trim());
        command.Parameters.AddWithValue("@HashSha256", backup.HashSha256.Trim());
        command.Parameters.AddWithValue("@Description", (object?)SqliteMapping.NullableString(backup.Description) ?? DBNull.Value);
        command.Parameters.AddWithValue("@CreatedAt", SqliteMapping.ToDb(backup.CreatedAt));

        var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToInt32(result);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Backups WHERE Id = @Id;";
        command.Parameters.AddWithValue("@Id", id);

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Backup Map(SqliteDataReader reader) =>
        new()
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            BackupType = Enum.Parse<BackupType>(reader.GetString(2)),
            FilePath = reader.GetString(3),
            HashSha256 = reader.GetString(4),
            Description = reader.IsDBNull(5) ? null : reader.GetString(5),
            CreatedAt = SqliteMapping.FromDb(reader.GetString(6))
        };
}
