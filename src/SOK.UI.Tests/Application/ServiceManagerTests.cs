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

public sealed class ServiceManagerTests : IDisposable
{
    private readonly string _tempRoot = Path.Combine(
        Path.GetTempPath(),
        "sok-service-tests-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task GetCatalogAsync_MarcaExistenciaYEstadoObservado()
    {
        var state = CreateState();
        state.Remove("Fax");
        state["DiagTrack"] = new ServiceSnapshot(
            "DiagTrack",
            "Experiencias de usuario conectado y telemetría",
            ServiceStartMode.Deshabilitado,
            ServiceState.Detenido);

        var control = CreateControlMock(state);
        var manager = CreateManager(control, CreateBackupServiceMock(), CreateBackupRepositoryMock(), new Mock<ILogService>());

        var catalog = await manager.GetCatalogAsync();

        var diagTrack = catalog.Single(entry => entry.Id == "DiagTrack");
        Assert.True(diagTrack.Exists);
        Assert.Equal(ServiceStatus.Deshabilitado, diagTrack.Status);
        Assert.Equal(ServiceState.Detenido, diagTrack.State);

        var noInstalado = catalog.Single(entry => entry.Id == "Fax");
        Assert.False(noInstalado.Exists);
        Assert.Equal(ServiceStatus.NoAplicable, noInstalado.Status);
    }

    [Fact]
    public async Task SetDisabledAsync_Exitoso_CreaElRespaldoAntesDeEscribir()
    {
        var state = CreateState();
        var control = CreateControlMock(state);
        var backups = CreateBackupServiceMock();
        var backupRepository = CreateBackupRepositoryMock();
        var logs = new Mock<ILogService>();

        var sequence = new MockSequence();
        backups
            .InSequence(sequence)
            .Setup(b => b.CreateBackupForServicesAsync(
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateManifestFile());
        control
            .InSequence(sequence)
            .Setup(c => c.TryStopAsync("DiagTrack", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        control
            .InSequence(sequence)
            .Setup(c => c.SetStartModeAsync(
                "DiagTrack",
                ServiceStartMode.Deshabilitado,
                It.IsAny<CancellationToken>()))
            .Callback((string name, ServiceStartMode mode, CancellationToken _) =>
                state[name] = state[name] with { StartMode = mode })
            .Returns(Task.CompletedTask);

        var manager = CreateManager(control, backups, backupRepository, logs);

        var result = await manager.SetDisabledAsync("DiagTrack", disabled: true);

        Assert.True(result.Success);
        Assert.Equal(1, result.AppliedCount);
        Assert.Equal(0, result.FailedCount);
        Assert.NotNull(result.BackupFilePath);

        backups.Verify(
            b => b.CreateBackupForServicesAsync(
                It.Is<IReadOnlyCollection<string>>(names => names.Single() == "DiagTrack"),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        backupRepository.Verify(
            r => r.AddAsync(
                It.Is<Backup>(backup => backup.BackupType == BackupType.Servicios),
                It.IsAny<CancellationToken>()),
            Times.Once);

        logs.Verify(
            l => l.LogAsync(
                LogModule.Servicios,
                "Deshabilitar servicio",
                "DiagTrack",
                ActionResult.Exitoso,
                It.IsAny<string>(),
                7,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetDisabledAsync_SinCambio_NoCreaRespaldoNiEscribe()
    {
        var state = CreateState();
        state["DiagTrack"] = state["DiagTrack"] with { StartMode = ServiceStartMode.Deshabilitado };

        var control = CreateControlMock(state);
        var backups = CreateBackupServiceMock();
        var manager = CreateManager(control, backups, CreateBackupRepositoryMock(), new Mock<ILogService>());

        var result = await manager.SetDisabledAsync("DiagTrack", disabled: true);

        Assert.True(result.Success);
        Assert.Equal(0, result.AppliedCount);
        Assert.Null(result.BackupFilePath);
        backups.VerifyNoOtherCalls();
        control.Verify(
            c => c.SetStartModeAsync(
                It.IsAny<string>(),
                It.IsAny<ServiceStartMode>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SetDisabledAsync_FalloDeEscritura_RegistraResultadoFallido()
    {
        var state = CreateState();
        var control = CreateControlMock(state);
        control
            .Setup(c => c.SetStartModeAsync(
                "DiagTrack",
                ServiceStartMode.Deshabilitado,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.ComponentModel.Win32Exception("Acceso denegado."));

        var backups = CreateBackupServiceMock();
        var logs = new Mock<ILogService>();
        var manager = CreateManager(control, backups, CreateBackupRepositoryMock(), logs);

        var result = await manager.SetDisabledAsync("DiagTrack", disabled: true);

        Assert.False(result.Success);
        Assert.Equal(0, result.AppliedCount);
        Assert.Equal(1, result.FailedCount);
        Assert.Contains(result.Failures, failure => failure.Contains("Acceso denegado"));

        backups.Verify(
            b => b.CreateBackupForServicesAsync(
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        logs.Verify(
            l => l.LogAsync(
                LogModule.Servicios,
                "Deshabilitar servicio",
                "DiagTrack",
                ActionResult.Fallido,
                "Acceso denegado.",
                7,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetDisabledAsync_Habilitar_UsaElModoDeInicioPorDefecto()
    {
        var state = CreateState();
        state["DiagTrack"] = state["DiagTrack"] with { StartMode = ServiceStartMode.Deshabilitado };

        var control = CreateControlMock(state);
        var manager = CreateManager(
            control,
            CreateBackupServiceMock(),
            CreateBackupRepositoryMock(),
            new Mock<ILogService>());

        var result = await manager.SetDisabledAsync("DiagTrack", disabled: false);

        Assert.True(result.Success);
        control.Verify(
            c => c.SetStartModeAsync(
                "DiagTrack",
                ServiceStartMode.Automatico,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetDisabledAsync_ServicioInexistente_NoCreaRespaldo()
    {
        var state = CreateState();
        state.Remove("Fax");

        var control = CreateControlMock(state);
        var backups = CreateBackupServiceMock();
        var manager = CreateManager(control, backups, CreateBackupRepositoryMock(), new Mock<ILogService>());

        var result = await manager.SetDisabledAsync("Fax", disabled: true);

        Assert.False(result.Success);
        Assert.Null(result.BackupFilePath);
        Assert.Contains(result.Failures, failure => failure.Contains("no existe"));
        backups.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SetDisabledAsync_ServicioFueraDelCatalogo_LanzaArgumentException()
    {
        var manager = CreateManager(
            CreateControlMock(CreateState()),
            CreateBackupServiceMock(),
            CreateBackupRepositoryMock(),
            new Mock<ILogService>());

        await Assert.ThrowsAsync<ArgumentException>(
            () => manager.SetDisabledAsync("ServicioInventado", disabled: true));
    }

    [Fact]
    public async Task ApplyProfileAsync_CreaUnSoloRespaldoYAplicaTodosLosPendientes()
    {
        var state = CreateState();
        var control = CreateControlMock(state);
        var backups = CreateBackupServiceMock();
        var logs = new Mock<ILogService>();
        var manager = CreateManager(control, backups, CreateBackupRepositoryMock(), logs);

        var expected = ServiceCatalog.CreateDefinitions()
            .Where(entry => entry.DisabledInGamer)
            .Select(entry => entry.Id)
            .ToList();

        var result = await manager.ApplyProfileAsync(ServiceProfile.Gamer);

        Assert.True(result.Success);
        Assert.Equal(expected.Count, result.AppliedCount);
        Assert.Equal(0, result.FailedCount);

        backups.Verify(
            b => b.CreateBackupForServicesAsync(
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        control.Verify(
            c => c.SetStartModeAsync(
                It.IsAny<string>(),
                ServiceStartMode.Deshabilitado,
                It.IsAny<CancellationToken>()),
            Times.Exactly(expected.Count));

        logs.Verify(
            l => l.LogAsync(
                LogModule.Servicios,
                "Aplicar perfil Gamer",
                null,
                ActionResult.Exitoso,
                It.Is<string>(detail => detail.Contains($"{expected.Count} de {expected.Count}")),
                7,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ApplyProfileAsync_NuncaTocaLosServiciosDeAltoRiesgo()
    {
        var state = CreateState();
        var control = CreateControlMock(state);
        var manager = CreateManager(
            control,
            CreateBackupServiceMock(),
            CreateBackupRepositoryMock(),
            new Mock<ILogService>());

        await manager.ApplyProfileAsync(ServiceProfile.Gamer);
        await manager.ApplyProfileAsync(ServiceProfile.Ofimatico);

        foreach (var id in new[] { "WSearch", "SharedAccess", "bthserv", "WbioSrvc", "RemoteAccess", "Spooler" })
        {
            control.Verify(
                c => c.SetStartModeAsync(
                    id,
                    It.IsAny<ServiceStartMode>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }

    [Fact]
    public async Task ApplyProfileAsync_SinPendientes_NoCreaRespaldo()
    {
        var state = CreateState();
        foreach (var id in state.Keys.ToList())
        {
            state[id] = state[id] with { StartMode = ServiceStartMode.Deshabilitado };
        }

        var control = CreateControlMock(state);
        var backups = CreateBackupServiceMock();
        var manager = CreateManager(control, backups, CreateBackupRepositoryMock(), new Mock<ILogService>());

        var result = await manager.ApplyProfileAsync(ServiceProfile.Ofimatico);

        Assert.True(result.Success);
        Assert.Equal(0, result.AppliedCount);
        Assert.Null(result.BackupFilePath);
        backups.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ApplyProfileAsync_FalloParcial_ReportaLosFallosYRegistraBitacora()
    {
        var state = CreateState();
        var control = CreateControlMock(state);
        control
            .Setup(c => c.SetStartModeAsync(
                "DiagTrack",
                ServiceStartMode.Deshabilitado,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.ComponentModel.Win32Exception("Acceso denegado."));

        var logs = new Mock<ILogService>();
        var manager = CreateManager(
            control,
            CreateBackupServiceMock(),
            CreateBackupRepositoryMock(),
            logs);

        var result = await manager.ApplyProfileAsync(ServiceProfile.Gamer);

        Assert.False(result.Success);
        Assert.Equal(1, result.FailedCount);
        Assert.Contains(result.Failures, failure => failure.Contains("Acceso denegado"));

        logs.Verify(
            l => l.LogAsync(
                LogModule.Servicios,
                "Aplicar perfil Gamer",
                null,
                ActionResult.Fallido,
                It.Is<string>(detail => detail.Contains("Fallos:")),
                7,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private Dictionary<string, ServiceSnapshot> CreateState()
    {
        var state = new Dictionary<string, ServiceSnapshot>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in ServiceCatalog.CreateDefinitions())
        {
            state[entry.Id] = new ServiceSnapshot(
                entry.Id,
                entry.DisplayName,
                entry.DefaultStartMode,
                ServiceState.Detenido);
        }

        return state;
    }

    private string CreateManifestFile()
    {
        Directory.CreateDirectory(_tempRoot);
        var path = Path.Combine(_tempRoot, "services-backup.json");
        File.WriteAllText(path, "[]");
        return path;
    }

    private Mock<IServicesBackupService> CreateBackupServiceMock()
    {
        var backups = new Mock<IServicesBackupService>();
        backups
            .Setup(b => b.CreateBackupForServicesAsync(
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => CreateManifestFile());
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

    private ServiceManager CreateManager(
        Mock<IServiceControlService> control,
        Mock<IServicesBackupService> backups,
        Mock<IBackupRepository> backupRepository,
        Mock<ILogService> logs)
    {
        var paths = new Mock<IAppPathProvider>();
        paths.SetupGet(p => p.BackupDirectory).Returns(Path.Combine(_tempRoot, "Backups"));

        return new ServiceManager(
            control.Object,
            backups.Object,
            backupRepository.Object,
            logs.Object,
            paths.Object);
    }

    private static Mock<IServiceControlService> CreateControlMock(
        Dictionary<string, ServiceSnapshot> state)
    {
        var control = new Mock<IServiceControlService>();

        control
            .Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => state.Values.ToList());

        control
            .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string name, CancellationToken _) =>
                state.TryGetValue(name, out var snapshot) ? snapshot : null);

        control
            .Setup(c => c.TryStopAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        control
            .Setup(c => c.SetStartModeAsync(
                It.IsAny<string>(),
                It.IsAny<ServiceStartMode>(),
                It.IsAny<CancellationToken>()))
            .Callback((string name, ServiceStartMode mode, CancellationToken _) =>
            {
                if (state.TryGetValue(name, out var snapshot))
                {
                    state[name] = snapshot with { StartMode = mode };
                }
            })
            .Returns(Task.CompletedTask);

        return control;
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }
}
