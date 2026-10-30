using Moq;
using SOK.Application.Abstractions;
using SOK.Application.Services;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;
using Xunit;

namespace SOK.UI.Tests.Application;

public sealed class BackupManagerTests : IDisposable
{
    private readonly string _tempRoot = Path.Combine(
        Path.GetTempPath(),
        "sok-backup-tests-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task CreateFullBackupAsync_GeneraManifiestoHashYRegistro()
    {
        Directory.CreateDirectory(_tempRoot);

        var registryFile = Path.Combine(_tempRoot, "registry-manifest.json");
        var servicesFile = Path.Combine(_tempRoot, "services-backup.json");
        await File.WriteAllTextAsync(registryFile, "{}");
        await File.WriteAllTextAsync(servicesFile, "[]");

        var registry = new Mock<IRegistryBackupService>();
        registry
            .Setup(r => r.CreateBackupAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registryFile);

        var services = new Mock<IServicesBackupService>();
        services
            .Setup(s => s.CreateBackupAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicesFile);

        var backups = new Mock<IBackupRepository>();
        Backup? saved = null;
        backups
            .Setup(b => b.AddAsync(It.IsAny<Backup>(), It.IsAny<CancellationToken>()))
            .Callback<Backup, CancellationToken>((backup, _) => saved = backup)
            .ReturnsAsync(42);

        var logs = new Mock<ILogService>();

        var paths = new Mock<IAppPathProvider>();
        paths.SetupGet(p => p.BackupDirectory).Returns(Path.Combine(_tempRoot, "Backups"));

        var manager = new BackupManager(
            registry.Object,
            services.Object,
            backups.Object,
            logs.Object,
            paths.Object);

        var result = await manager.CreateFullBackupAsync("prueba de respaldo");

        Assert.Equal(42, result.Id);
        Assert.Equal(BackupType.Completo, result.BackupType);
        Assert.True(File.Exists(result.FilePath));
        Assert.Equal(64, result.HashSha256.Length);
        Assert.NotNull(saved);
        Assert.Equal(42, saved!.Id);
        Assert.Equal("prueba de respaldo", saved.Description);

        logs.Verify(
            l => l.LogAsync(
                LogModule.Respaldos,
                "Crear respaldo completo",
                It.IsAny<string>(),
                ActionResult.Exitoso,
                "prueba de respaldo",
                42,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }
}
