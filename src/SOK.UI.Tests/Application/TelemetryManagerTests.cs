using Moq;
using SOK.Application.Abstractions;
using SOK.Application.Common;
using SOK.Application.Services;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;
using Xunit;

namespace SOK.UI.Tests.Application;

public sealed class TelemetryManagerTests : IDisposable
{
    private readonly string _tempRoot = Path.Combine(
        Path.GetTempPath(),
        "sok-telemetry-tests-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task SetDisabledAsync_Exitoso_CreaRespaldoYRegistraBitacora()
    {
        var key = GetKey("advertising-id-user");
        var state = new RegistryState();
        state.Set(key.RegistryPath, key.ValueName, new TelemetryValue(key.ValueKind, key.EnabledValue));

        var registry = CreateRegistryMock(state);
        var backups = CreateBackupServiceMock();
        var backupRepository = CreateBackupRepositoryMock();
        var logs = new Mock<ILogService>();
        var manager = CreateManager(registry, backups, backupRepository, logs);

        var result = await manager.SetDisabledAsync(key.Id, disabled: true);

        Assert.True(result.Success);
        Assert.Equal(1, result.AppliedCount);
        Assert.Equal(0, result.FailedCount);
        Assert.NotNull(result.BackupFilePath);

        backups.Verify(
            b => b.CreateBackupForKeysAsync(
                It.Is<IReadOnlyCollection<string>>(keys => keys.Contains(key.RegistryPath)),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        registry.Verify(
            r => r.WriteValueAsync(
                key.RegistryPath,
                key.ValueName,
                key.DisabledValue,
                key.ValueKind,
                It.IsAny<CancellationToken>()),
            Times.Once);

        logs.Verify(
            l => l.LogAsync(
                LogModule.Telemetria,
                "Deshabilitar telemetría",
                key.TargetKey,
                ActionResult.Exitoso,
                It.IsAny<string>(),
                7,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetDisabledAsync_SinCambio_NoCreaRespaldoNiEscribe()
    {
        var key = GetKey("advertising-id-user");
        var state = new RegistryState();
        state.Set(key.RegistryPath, key.ValueName, new TelemetryValue(key.ValueKind, key.DisabledValue));

        var registry = CreateRegistryMock(state);
        var backups = CreateBackupServiceMock();
        var backupRepository = CreateBackupRepositoryMock();
        var logs = new Mock<ILogService>();
        var manager = CreateManager(registry, backups, backupRepository, logs);

        var result = await manager.SetDisabledAsync(key.Id, disabled: true);

        Assert.True(result.Success);
        Assert.Equal(0, result.AppliedCount);
        Assert.Null(result.BackupFilePath);
        backups.VerifyNoOtherCalls();
        registry.Verify(
            r => r.WriteValueAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<TelemetryValueKind>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        logs.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SetDisabledAsync_FalloDeEscritura_RegistraResultadoFallido()
    {
        var key = GetKey("advertising-id-user");
        var state = new RegistryState();
        state.Set(key.RegistryPath, key.ValueName, new TelemetryValue(key.ValueKind, key.EnabledValue));

        var registry = new Mock<ITelemetryRegistryService>();
        registry
            .Setup(r => r.ReadValueAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string path, string name, CancellationToken _) => state.Get(path, name));
        registry
            .Setup(r => r.WriteValueAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<TelemetryValueKind>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Acceso denegado."));

        var backups = CreateBackupServiceMock();
        var backupRepository = CreateBackupRepositoryMock();
        var logs = new Mock<ILogService>();
        var manager = CreateManager(registry, backups, backupRepository, logs);

        var result = await manager.SetDisabledAsync(key.Id, disabled: true);

        Assert.False(result.Success);
        Assert.Equal(0, result.AppliedCount);
        Assert.Equal(1, result.FailedCount);
        Assert.Contains(result.Failures, failure => failure.Contains("Acceso denegado"));

        backups.Verify(
            b => b.CreateBackupForKeysAsync(
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        logs.Verify(
            l => l.LogAsync(
                LogModule.Telemetria,
                "Deshabilitar telemetría",
                key.TargetKey,
                ActionResult.Fallido,
                "Acceso denegado.",
                7,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ApplyMaxPrivacyAsync_CreaUnSoloRespaldoYAplicaTodasLasPendientes()
    {
        var catalog = TelemetryCatalog.CreateDefinitions();
        var state = new RegistryState();
        foreach (var key in catalog)
        {
            var value = key.ValueKind == TelemetryValueKind.Dword ? "7" : "Otro";
            state.Set(key.RegistryPath, key.ValueName, new TelemetryValue(key.ValueKind, value));
        }

        var registry = CreateRegistryMock(state);
        var backups = CreateBackupServiceMock();
        var backupRepository = CreateBackupRepositoryMock();
        var logs = new Mock<ILogService>();
        var manager = CreateManager(registry, backups, backupRepository, logs);

        var result = await manager.ApplyMaxPrivacyAsync();

        Assert.True(result.Success);
        Assert.Equal(catalog.Count, result.AppliedCount);
        Assert.Equal(0, result.FailedCount);
        Assert.NotNull(result.BackupFilePath);

        backups.Verify(
            b => b.CreateBackupForKeysAsync(
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        registry.Verify(
            r => r.WriteValueAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<TelemetryValueKind>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(catalog.Count));

        logs.Verify(
            l => l.LogAsync(
                LogModule.Telemetria,
                "Aplicar privacidad máxima",
                null,
                ActionResult.Exitoso,
                It.Is<string>(detail => detail.Contains($"{catalog.Count} de {catalog.Count}")),
                7,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ApplyMaxPrivacyAsync_SinPendientes_NoCreaRespaldo()
    {
        var catalog = TelemetryCatalog.CreateDefinitions();
        var state = new RegistryState();
        foreach (var key in catalog)
        {
            state.Set(key.RegistryPath, key.ValueName, new TelemetryValue(key.ValueKind, key.DisabledValue));
        }

        var registry = CreateRegistryMock(state);
        var backups = CreateBackupServiceMock();
        var backupRepository = CreateBackupRepositoryMock();
        var logs = new Mock<ILogService>();
        var manager = CreateManager(registry, backups, backupRepository, logs);

        var result = await manager.ApplyMaxPrivacyAsync();

        Assert.True(result.Success);
        Assert.Equal(0, result.AppliedCount);
        Assert.Null(result.BackupFilePath);
        backups.VerifyNoOtherCalls();
        registry.Verify(
            r => r.WriteValueAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<TelemetryValueKind>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static TelemetryKey GetKey(string id) =>
        TelemetryCatalog.CreateDefinitions().Single(key => key.Id == id);

    private Mock<IRegistryBackupService> CreateBackupServiceMock()
    {
        Directory.CreateDirectory(_tempRoot);
        var manifestPath = Path.Combine(_tempRoot, "registry-manifest.json");
        File.WriteAllText(manifestPath, "{}");

        var backups = new Mock<IRegistryBackupService>();
        backups
            .Setup(b => b.CreateBackupForKeysAsync(
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(manifestPath);
        return backups;
    }

    private static Mock<IBackupRepository> CreateBackupRepositoryMock()
    {
        var repository = new Mock<IBackupRepository>();
        repository
            .Setup(r => r.AddAsync(It.IsAny<Backup>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(7);
        return repository;
    }

    private TelemetryManager CreateManager(
        Mock<ITelemetryRegistryService> registry,
        Mock<IRegistryBackupService> backups,
        Mock<IBackupRepository> backupRepository,
        Mock<ILogService> logs)
    {
        var paths = new Mock<IAppPathProvider>();
        paths.SetupGet(p => p.BackupDirectory).Returns(Path.Combine(_tempRoot, "Backups"));

        return new TelemetryManager(
            registry.Object,
            backups.Object,
            backupRepository.Object,
            logs.Object,
            paths.Object);
    }

    private static Mock<ITelemetryRegistryService> CreateRegistryMock(RegistryState state)
    {
        var registry = new Mock<ITelemetryRegistryService>();

        registry
            .Setup(r => r.ReadValueAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns((string path, string name, CancellationToken _) => Task.FromResult(state.Get(path, name)));

        registry
            .Setup(r => r.WriteValueAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<TelemetryValueKind>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, string, string, TelemetryValueKind, CancellationToken>(
                (path, name, value, kind, _) => state.Set(path, name, new TelemetryValue(kind, value)))
            .Returns(Task.CompletedTask);

        return registry;
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }

    private sealed class RegistryState
    {
        private readonly Dictionary<string, TelemetryValue> _values =
            new(StringComparer.OrdinalIgnoreCase);

        public void Set(string path, string name, TelemetryValue value) =>
            _values[BuildKey(path, name)] = value;

        public TelemetryValue? Get(string path, string name) =>
            _values.TryGetValue(BuildKey(path, name), out var value) ? value : null;

        private static string BuildKey(string path, string name) => $@"{path}\{name}";
    }
}
