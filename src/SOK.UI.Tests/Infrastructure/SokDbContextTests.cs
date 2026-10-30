using Microsoft.Data.Sqlite;
using SOK.Domain.Entities;
using SOK.Domain.Enums;
using SOK.Infrastructure.Persistence;
using SOK.Infrastructure.Persistence.Repositories;
using Xunit;

namespace SOK.UI.Tests.Infrastructure;

public sealed class SokDbContextTests : IDisposable
{
    private readonly string _databasePath = Path.Combine(
        Path.GetTempPath(),
        "sok-db-tests-" + Guid.NewGuid().ToString("N") + ".db");

    [Fact]
    public async Task InitializeAsync_CreaElEsquemaYLosRepositoriosFuncionan()
    {
        var context = new SokDbContext(_databasePath);
        await context.InitializeAsync();

        var settings = new SettingsRepository(context);
        await settings.SetAsync("MemoryReleaseThreshold", "75");

        var setting = await settings.GetAsync("MemoryReleaseThreshold");
        Assert.NotNull(setting);
        Assert.Equal("75", setting!.Value);

        var logs = new LogRepository(context);
        var id = await logs.AddAsync(new LogEntry
        {
            Module = LogModule.Dashboard,
            Action = "Prueba de bitácora",
            Result = ActionResult.Exitoso
        });

        Assert.True(id > 0);

        var recent = await logs.GetRecentAsync();
        Assert.Single(recent);
        Assert.Equal("Prueba de bitácora", recent[0].Action);
    }

    [Fact]
    public async Task InitializeAsync_EsIdempotenteYConservaLosPerfilesIniciales()
    {
        var context = new SokDbContext(_databasePath);
        await context.InitializeAsync();
        await context.InitializeAsync();

        var profiles = new ProfileRepository(context);
        var all = await profiles.GetAllAsync();

        Assert.Contains(all, profile => profile.Name == "Ofimatico");
        Assert.Contains(all, profile => profile.Name == "Gamer");
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();

        foreach (var suffix in new[] { string.Empty, "-wal", "-shm" })
        {
            var path = _databasePath + suffix;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
