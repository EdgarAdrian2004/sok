using Microsoft.Data.Sqlite;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio ADO.NET de perfiles de optimización.
/// </summary>
public sealed class ProfileRepository : IProfileRepository
{
    private const string SelectColumns =
        "Id, Name, Type, Description, IsDefault, CreatedAt, UpdatedAt";

    private readonly SokDbContext _context;

    public ProfileRepository(SokDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IReadOnlyList<Profile>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var profiles = new List<Profile>();

        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = $"SELECT {SelectColumns} FROM Profiles ORDER BY Name;";

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            profiles.Add(Map(reader));
        }

        return profiles;
    }

    public async Task<Profile?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = $"SELECT {SelectColumns} FROM Profiles WHERE Id = @Id;";
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        return await reader.ReadAsync(cancellationToken).ConfigureAwait(false) ? Map(reader) : null;
    }

    public async Task<Profile?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = $"SELECT {SelectColumns} FROM Profiles WHERE Name = @Name;";
        command.Parameters.AddWithValue("@Name", name.Trim());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        return await reader.ReadAsync(cancellationToken).ConfigureAwait(false) ? Map(reader) : null;
    }

    public async Task<int> AddAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentException.ThrowIfNullOrWhiteSpace(profile.Name);

        var now = DateTime.Now;
        profile.CreatedAt = profile.CreatedAt == default ? now : profile.CreatedAt;
        profile.UpdatedAt = now;

        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Profiles (Name, Type, Description, IsDefault, CreatedAt, UpdatedAt)
            VALUES (@Name, @Type, @Description, @IsDefault, @CreatedAt, @UpdatedAt);
            SELECT last_insert_rowid();
            """;

        command.Parameters.AddWithValue("@Name", profile.Name.Trim());
        command.Parameters.AddWithValue("@Type", profile.Type.ToString());
        command.Parameters.AddWithValue("@Description", (object?)SqliteMapping.NullableString(profile.Description) ?? DBNull.Value);
        command.Parameters.AddWithValue("@IsDefault", profile.IsDefault ? 1 : 0);
        command.Parameters.AddWithValue("@CreatedAt", SqliteMapping.ToDb(profile.CreatedAt));
        command.Parameters.AddWithValue("@UpdatedAt", SqliteMapping.ToDb(profile.UpdatedAt));

        var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToInt32(result);
    }

    public async Task UpdateAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(profile);

        profile.UpdatedAt = DateTime.Now;

        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Profiles
            SET Name = @Name,
                Type = @Type,
                Description = @Description,
                IsDefault = @IsDefault,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id;
            """;

        command.Parameters.AddWithValue("@Id", profile.Id);
        command.Parameters.AddWithValue("@Name", profile.Name.Trim());
        command.Parameters.AddWithValue("@Type", profile.Type.ToString());
        command.Parameters.AddWithValue("@Description", (object?)SqliteMapping.NullableString(profile.Description) ?? DBNull.Value);
        command.Parameters.AddWithValue("@IsDefault", profile.IsDefault ? 1 : 0);
        command.Parameters.AddWithValue("@UpdatedAt", SqliteMapping.ToDb(profile.UpdatedAt));

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = _context.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Profiles WHERE Id = @Id;";
        command.Parameters.AddWithValue("@Id", id);

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Profile Map(SqliteDataReader reader) =>
        new()
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Type = Enum.Parse<ProfileType>(reader.GetString(2)),
            Description = reader.IsDBNull(3) ? null : reader.GetString(3),
            IsDefault = reader.GetInt32(4) == 1,
            CreatedAt = SqliteMapping.FromDb(reader.GetString(5)),
            UpdatedAt = SqliteMapping.FromDb(reader.GetString(6))
        };
}
