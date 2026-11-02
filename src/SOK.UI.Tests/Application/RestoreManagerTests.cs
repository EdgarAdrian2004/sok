using System.Text.Json;
using Moq;
using SOK.Application.Abstractions;
using SOK.Application.Common;
using SOK.Application.DTOs;
using SOK.Application.Services;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;
using Xunit;

namespace SOK.UI.Tests.Application;

public sealed class RestoreManagerTests
{
    [Fact]
    public async Task GetBackupsAsync_VerificaExistenciaYHash()
    {
        var directory = CreateTempDirectory();

        try
        {
            var validPath = Path.Combine(directory, "valido.json");
            var invalidPath = Path.Combine(directory, "alterado.json");
            var missingPath = Path.Combine(directory, "inexistente.json");

            await File.WriteAllTextAsync(validPath, "{\"ok\":true}");
            await File.WriteAllTextAsync(invalidPath, "{\"ok\":true}");

            var validHash = await Sha256Helper.ComputeFileHashAsync(validPath);

            var backups = new Mock<IBackupRepository>();
            backups
                .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Backup>
                {
                    new() { Id = 1, Name = "Válido", BackupType = BackupType.Completo, FilePath = validPath, HashSha256 = validHash },
                    new() { Id = 2, Name = "Alterado", BackupType = BackupType.Registro, FilePath = invalidPath, HashSha256 = new string('0', 64) },
                    new() { Id = 3, Name = "Inexistente", BackupType = BackupType.Servicios, FilePath = missingPath, HashSha256 = new string('0', 64) }
                });

            var manager = CreateManager(backups: backups);
            var statuses = await manager.GetBackupsAsync();

            Assert.Equal(3, statuses.Count);
            Assert.True(statuses[0].CanRestore);
            Assert.False(statuses[1].CanRestore);
            Assert.False(statuses[2].CanRestore);
            Assert.Contains("Íntegro", statuses[0].IntegrityText);
            Assert.Contains("Hash alterado", statuses[1].IntegrityText);
            Assert.Contains("no encontrado", statuses[2].IntegrityText);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task RestoreAsync_RespaldoInexistente_DevuelveFalloYRegistraLaBitacora()
    {
        var backups = new Mock<IBackupRepository>();
        backups
            .Setup(repository => repository.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Backup?)null);

        var logs = new Mock<ILogService>();
        var manager = CreateManager(backups: backups, logs: logs);

        var result = await manager.RestoreAsync(99);

        Assert.False(result.Success);
        Assert.Contains("No existe el respaldo", result.Message);

        logs.Verify(
            service => service.LogAsync(
                LogModule.Respaldos,
                "Restaurar sistema",
                null,
                ActionResult.Fallido,
                It.Is<string>(detail => detail.Contains("No existe")),
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RestoreAsync_HashAlterado_NoRestauraNada()
    {
        var directory = CreateTempDirectory();

        try
        {
            var path = Path.Combine(directory, "backup-manifest.json");
            await File.WriteAllTextAsync(path, "{\"Registry\":\"x\",\"Services\":\"y\"}");

            var backups = new Mock<IBackupRepository>();
            backups
                .Setup(repository => repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Backup
                {
                    Id = 1,
                    Name = "Alterado",
                    BackupType = BackupType.Completo,
                    FilePath = path,
                    HashSha256 = new string('0', 64)
                });

            var registry = new Mock<IRegistryRestoreService>();
            var manager = CreateManager(backups: backups, registry: registry);

            var result = await manager.RestoreAsync(1);

            Assert.False(result.Success);
            Assert.Contains("SHA-256", result.Message);

            registry.Verify(
                service => service.RestoreFromManifestAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task RestoreAsync_ManifiestoValido_RestauraRegistroYServicios()
    {
        var directory = CreateTempDirectory();

        try
        {
            var registryManifest = Path.Combine(directory, "registry-manifest.json");
            var servicesBackup = Path.Combine(directory, "services-backup.json");
            var manifest = Path.Combine(directory, "backup-manifest.json");

            await File.WriteAllTextAsync(registryManifest, "[]");
            await File.WriteAllTextAsync(servicesBackup, """
                [
                  { "Name": "DiagTrack", "DisplayName": "Telemetría", "StartMode": "Auto", "State": "Running" },
                  { "Name": "Fax", "DisplayName": "Fax", "StartMode": "Manual", "State": "Stopped" }
                ]
                """);
            await File.WriteAllTextAsync(manifest, JsonSerializer.Serialize(new
            {
                CreatedAt = DateTime.Now,
                Description = "Respaldo de prueba",
                Registry = registryManifest,
                Services = servicesBackup
            }));

            var hash = await Sha256Helper.ComputeFileHashAsync(manifest);

            var backups = new Mock<IBackupRepository>();
            backups
                .Setup(repository => repository.GetByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Backup
                {
                    Id = 5,
                    Name = "Respaldo de prueba",
                    BackupType = BackupType.Completo,
                    FilePath = manifest,
                    HashSha256 = hash
                });

            var registry = new Mock<IRegistryRestoreService>();
            registry
                .Setup(service => service.RestoreFromManifestAsync(
                    registryManifest,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RegistryRestoreOutcome(3, 1, Array.Empty<string>()));

            var serviceControl = new Mock<IServiceControlService>();
            serviceControl
                .Setup(service => service.SetStartModeAsync(
                    It.IsAny<string>(),
                    It.IsAny<ServiceStartMode>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            serviceControl
                .Setup(service => service.GetAsync("DiagTrack", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceSnapshot("DiagTrack", "Telemetría", ServiceStartMode.Automatico, ServiceState.Detenido));
            serviceControl
                .Setup(service => service.GetAsync("Fax", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceSnapshot("Fax", "Fax", ServiceStartMode.Manual, ServiceState.Detenido));

            var logs = new Mock<ILogService>();
            var manager = CreateManager(
                backups: backups,
                registry: registry,
                serviceControl: serviceControl,
                logs: logs);

            var result = await manager.RestoreAsync(5);

            Assert.True(result.Success);
            Assert.Equal(3, result.RegistryKeysRestored);
            Assert.Equal(2, result.ServicesRestored);
            Assert.Empty(result.Failures);

            logs.Verify(
                service => service.LogAsync(
                    LogModule.Respaldos,
                    "Restaurar sistema",
                    "Respaldo de prueba",
                    ActionResult.Revertido,
                    It.Is<string>(detail => detail.Contains("3 claves") && detail.Contains("2 servicios")),
                    5,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task RestoreAsync_FalloParcial_DevuelveFalloSinLanzarExcepcion()
    {
        var directory = CreateTempDirectory();

        try
        {
            var manifest = Path.Combine(directory, "backup-manifest.json");
            await File.WriteAllTextAsync(manifest, JsonSerializer.Serialize(new
            {
                CreatedAt = DateTime.Now,
                Description = "Parcial",
                Registry = Path.Combine(directory, "registry-manifest.json"),
                Services = (string?)null
            }));

            var hash = await Sha256Helper.ComputeFileHashAsync(manifest);

            var backups = new Mock<IBackupRepository>();
            backups
                .Setup(repository => repository.GetByIdAsync(6, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Backup
                {
                    Id = 6,
                    Name = "Parcial",
                    BackupType = BackupType.Completo,
                    FilePath = manifest,
                    HashSha256 = hash
                });

            var registry = new Mock<IRegistryRestoreService>();
            registry
                .Setup(service => service.RestoreFromManifestAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RegistryRestoreOutcome(
                    1,
                    0,
                    new[] { "HKCU\\Software\\X: el hash no coincide." }));

            var manager = CreateManager(backups: backups, registry: registry);
            var result = await manager.RestoreAsync(6);

            Assert.False(result.Success);
            Assert.Single(result.Failures);
            Assert.Contains("hash no coincide", result.Failures[0]);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static RestoreManager CreateManager(
        Mock<IBackupRepository>? backups = null,
        Mock<IRegistryRestoreService>? registry = null,
        Mock<IServiceControlService>? serviceControl = null,
        Mock<ILogService>? logs = null) =>
        new(
            (backups ?? new Mock<IBackupRepository>()).Object,
            (registry ?? new Mock<IRegistryRestoreService>()).Object,
            (serviceControl ?? new Mock<IServiceControlService>()).Object,
            (logs ?? new Mock<ILogService>()).Object);

    private static string CreateTempDirectory()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            $"sok-restore-{Guid.NewGuid():N}");

        Directory.CreateDirectory(directory);
        return directory;
    }
}
