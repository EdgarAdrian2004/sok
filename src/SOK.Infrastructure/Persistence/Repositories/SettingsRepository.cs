using Microsoft.Data.Sqlite;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;

namespace SOK.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio ADO.NET de configuración clave/valor.
/// </summary>
public sealed class SettingsRepository : ISettingsRepository
{
    private readonly SokDbContext _context;

    public SettingsRepository(SokDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Setting?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT SettingKey, SettingValue, UpdatedAt
            FROM Settings
            WHERE SettingKey = @Key;
            """;
        command.Parameters.AddWithValue("@Key", key.Trim());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        return await reader.ReadAsync(cancellationToken).ConfigureAwait(false) ? Map(reader) : null;
    }

    public async Task<IReadOnlyList<Setting>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var settings = new List<Setting>();

        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT SettingKey, SettingValue, UpdatedAt FROM Settings ORDER BY SettingKey;";

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            settings.Add(Map(reader));
        }

        return settings;
    }

    public async Task SetAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Settings (SettingKey, SettingValue, UpdatedAt)
            VALUES (@Key, @Value, @UpdatedAt)
            ON CONFLICT (SettingKey) DO UPDATE SET
                SettingValue = excluded.SettingValue,
                UpdatedAt = excluded.UpdatedAt;
            """;

        command.Parameters.AddWithValue("@Key", key.Trim());
        command.Parameters.AddWithValue("@Value", value);
        command.Parameters.AddWithValue("@UpdatedAt", SqliteMapping.ToDb(DateTime.Now));

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Settings WHERE SettingKey = @Key;";
        command.Parameters.AddWithValue("@Key", key.Trim());

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Setting Map(SqliteDataReader reader) =>
        new()
        {
            Key = reader.GetString(0),
            Value = reader.GetString(1),
            UpdatedAt = SqliteMapping.FromDb(reader.GetString(2))
        };
}
