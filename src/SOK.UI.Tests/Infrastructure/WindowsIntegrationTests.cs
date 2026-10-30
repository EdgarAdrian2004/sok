using Microsoft.Data.Sqlite;
using SOK.Application.Abstractions;
using SOK.Application.Services;
using SOK.Domain.Enums;
using SOK.Infrastructure.Persistence;
using SOK.Infrastructure.Persistence.Repositories;
using SOK.Infrastructure.Windows;
using Xunit;

namespace SOK.UI.Tests.Infrastructure;

/// <summary>
/// Pruebas de integración con Windows. No requieren elevación porque
/// solo leen información del sistema.
/// </summary>
public sealed class WindowsIntegrationTests : IDisposable
{
    private readonly string _tempDirectory = Path.Combine(
        Path.GetTempPath(),
        "sok-windows-tests-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task SystemInfoProvider_DevuelveDatosDelEquipo()
    {
        var provider = new SystemInfoProvider();

        var snapshot = await provider.GetSnapshotAsync();

        Assert.False(string.IsNullOrWhiteSpace(snapshot.OsName));
        Assert.False(string.IsNullOrWhiteSpace(snapshot.MachineName));
        Assert.False(string.IsNullOrWhiteSpace(snapshot.ProcessorName));
        Assert.True(snapshot.RamTotalBytes >= 0);
        Assert.InRange(snapshot.CpuUsagePercent, 0, 100);
    }

    [Fact]
    public async Task ServicesBackupService_GeneraUnJsonConServicios()
    {
        var service = new ServicesBackupService();

        var filePath = await service.CreateBackupAsync(_tempDirectory);

        Assert.True(File.Exists(filePath));
        var json = await File.ReadAllTextAsync(filePath);
        Assert.Contains("[", json);
        Assert.Contains("Name", json);
    }

    [Fact]
    public async Task RegistryBackupService_ExportaAlMenosUnaClaveDelRegistro()
    {
        var service = new RegistryBackupService();

        var manifestPath = await service.CreateBackupAsync(_tempDirectory);

        Assert.True(File.Exists(manifestPath));
        var json = await File.ReadAllTextAsync(manifestPath);
        Assert.Contains("HKCU", json);
    }

    [Fact]
    public async Task BackupManager_IntegracionReal_GeneraRespaldoCompletoYLoRegistra()
    {
        var databasePath = Path.Combine(_tempDirectory, "sok.db");
        var context = new SokDbContext(databasePath);
        await context.InitializeAsync();

        var backupRepository = new BackupRepository(context);
        var logRepository = new LogRepository(context);
        var manager = new BackupManager(
            new RegistryBackupService(),
            new ServicesBackupService(),
            backupRepository,
            new LogService(logRepository),
            new TestPathProvider(_tempDirectory));

        var backup = await manager.CreateFullBackupAsync("prueba de integracion");

        Assert.True(File.Exists(backup.FilePath));
        Assert.Equal(64, backup.HashSha256.Length);

        var stored = await backupRepository.GetByIdAsync(backup.Id);
        Assert.NotNull(stored);
        Assert.Equal(backup.HashSha256, stored!.HashSha256);

        var logs = await logRepository.GetByModuleAsync(LogModule.Respaldos);
        Assert.Contains(logs, entry => entry.Action == "Crear respaldo completo");
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();

        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }

    private sealed class TestPathProvider : IAppPathProvider
    {
        public TestPathProvider(string root)
        {
            AppDataDirectory = root;
            DatabasePath = Path.Combine(root, "sok.db");
            BackupDirectory = Path.Combine(root, "Backups");
        }

        public string AppDataDirectory { get; }

        public string DatabasePath { get; }

        public string BackupDirectory { get; }
    }
}
