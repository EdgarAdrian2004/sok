using Microsoft.Data.Sqlite;

namespace SOK.Infrastructure.Persistence;

/// <summary>
/// Contexto de SQLite basado en ADO.NET puro. Abre una conexión por
/// operación y garantiza que las claves foráneas estén activas.
/// </summary>
public sealed class SokDbContext
{
    private const string DdlResourceName = "SOK.Infrastructure.Persistence.SOK_DDL.sql";
    private readonly string _connectionString;

    public SokDbContext(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        DatabasePath = databasePath;

        var directory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        }.ToString();
    }

    public string DatabasePath { get; }

    public SqliteConnection CreateConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys = ON;";
        command.ExecuteNonQuery();

        return connection;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = ReadDdl();

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static string ReadDdl()
    {
        var assembly = typeof(SokDbContext).Assembly;
        using var stream = assembly.GetManifestResourceStream(DdlResourceName)
            ?? throw new InvalidOperationException($"No se encontró el recurso embebido '{DdlResourceName}'.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
