using Microsoft.Data.Sqlite;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio ADO.NET de los ajustes que componen un perfil.
/// </summary>
public sealed class ProfileSettingRepository : IProfileSettingRepository
{
    private readonly SokDbContext _context;

    public ProfileSettingRepository(SokDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IReadOnlyList<ProfileSetting>> GetByProfileIdAsync(int profileId, CancellationToken cancellationToken = default)
    {
        var settings = new List<ProfileSetting>();

        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, ProfileId, Module, TargetKey, Action, Value
            FROM ProfileSettings
            WHERE ProfileId = @ProfileId
            ORDER BY Module, TargetKey;
            """;
        command.Parameters.AddWithValue("@ProfileId", profileId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            settings.Add(Map(reader));
        }

        return settings;
    }

    public async Task<int> AddAsync(ProfileSetting setting, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(setting);
        ArgumentException.ThrowIfNullOrWhiteSpace(setting.TargetKey);

        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO ProfileSettings (ProfileId, Module, TargetKey, Action, Value)
            VALUES (@ProfileId, @Module, @TargetKey, @Action, @Value);
            SELECT last_insert_rowid();
            """;

        command.Parameters.AddWithValue("@ProfileId", setting.ProfileId);
        command.Parameters.AddWithValue("@Module", setting.Module.ToString());
        command.Parameters.AddWithValue("@TargetKey", setting.TargetKey.Trim());
        command.Parameters.AddWithValue("@Action", setting.Action.ToString());
        command.Parameters.AddWithValue("@Value", (object?)SqliteMapping.NullableString(setting.Value) ?? DBNull.Value);

        var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToInt32(result);
    }

    public async Task DeleteByProfileIdAsync(int profileId, CancellationToken cancellationToken = default)
    {
        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM ProfileSettings WHERE ProfileId = @ProfileId;";
        command.Parameters.AddWithValue("@ProfileId", profileId);

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static ProfileSetting Map(SqliteDataReader reader) =>
        new()
        {
            Id = reader.GetInt32(0),
            ProfileId = reader.GetInt32(1),
            Module = Enum.Parse<ProfileModule>(reader.GetString(2)),
            TargetKey = reader.GetString(3),
            Action = Enum.Parse<ProfileAction>(reader.GetString(4)),
            Value = reader.IsDBNull(5) ? null : reader.GetString(5)
        };
}
