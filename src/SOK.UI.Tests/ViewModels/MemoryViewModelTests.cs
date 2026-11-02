using Moq;
using SOK.Application.Abstractions;
using SOK.Application.DTOs;
using SOK.UI.ViewModels;
using Xunit;

namespace SOK.UI.Tests.ViewModels;

public sealed class MemoryViewModelTests
{
    [Fact]
    public async Task CargaInicial_ActualizaElMedidorYLaConfiguracion()
    {
        var snapshot = new MemorySnapshot(
            TotalBytes: 8L * 1024 * 1024 * 1024,
            AvailableBytes: 3L * 1024 * 1024 * 1024,
            UsagePercent: 62.5,
            CapturedAt: DateTimeOffset.Now);

        var manager = CreateManager(
            snapshot,
            new MemoryAutoReleaseOptions(true, 85));

        var viewModel = CreateViewModel(manager);

        await viewModel.InitializationTask;

        Assert.Equal(62.5, viewModel.RamUsagePercent);
        Assert.Contains("GB en uso", viewModel.RamUsageText);
        Assert.Contains("GB disponibles", viewModel.RamAvailableText);
        Assert.True(viewModel.AutoReleaseEnabled);
        Assert.Equal(85, viewModel.AutoReleaseThreshold);
        Assert.Contains("85", viewModel.AutoReleaseSummaryText);
        Assert.Contains("actualizado correctamente", viewModel.StatusMessage);
    }

    [Fact]
    public async Task RefreshNowCommand_ActualizaLasMetricas()
    {
        var manager = CreateManager(Snapshot(50));
        manager
            .SetupSequence(m => m.GetSnapshotAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Snapshot(50))
            .ReturnsAsync(Snapshot(70));

        var viewModel = CreateViewModel(manager);
        await viewModel.InitializationTask;

        await viewModel.RefreshNowCommand.ExecuteAsync(null);

        Assert.Equal(70, viewModel.RamUsagePercent);
        Assert.Contains("actualizado correctamente", viewModel.StatusMessage);
    }

    [Fact]
    public async Task ReleaseNowCommand_ActualizaElUltimoResultado()
    {
        var manager = CreateManager(Snapshot(60));
        var releaseResult = new MemoryReleaseResult(
            true,
            120,
            100,
            20,
            512L * 1024 * 1024,
            "Se liberó memoria de 100 de 120 procesos.");

        manager
            .Setup(m => m.ReleaseAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(releaseResult);

        var viewModel = CreateViewModel(manager);
        await viewModel.InitializationTask;

        await viewModel.ReleaseNowCommand.ExecuteAsync(null);

        manager.Verify(
            m => m.ReleaseAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.Contains("Liberación manual", viewModel.LastReleaseText);
        Assert.Contains("100 de 120", viewModel.LastReleaseText);
        Assert.Contains("MB", viewModel.LastReleaseText);
        Assert.Equal(releaseResult.Message, viewModel.StatusMessage);
    }

    [Fact]
    public async Task ReleaseNowCommand_Fallido_MuestraElMensaje()
    {
        var manager = CreateManager(Snapshot(60));
        manager
            .Setup(m => m.ReleaseAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(MemoryReleaseResult.Failed(
                "No se pudo liberar la memoria: acceso denegado."));

        var viewModel = CreateViewModel(manager);
        await viewModel.InitializationTask;

        await viewModel.ReleaseNowCommand.ExecuteAsync(null);

        Assert.Contains("acceso denegado", viewModel.StatusMessage);
        Assert.Contains("Liberación manual", viewModel.LastReleaseText);
    }

    [Fact]
    public async Task CambiarElInterruptor_GuardaLaConfiguracion()
    {
        var manager = CreateManager(
            Snapshot(50),
            new MemoryAutoReleaseOptions(false, 80));

        var viewModel = CreateViewModel(manager);
        await viewModel.InitializationTask;

        viewModel.AutoReleaseEnabled = true;
        await viewModel.PendingSaveTask;

        manager.Verify(
            m => m.SaveAutoReleaseOptionsAsync(
                It.Is<MemoryAutoReleaseOptions>(
                    options => options.Enabled && options.ThresholdPercent == 80),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CambiarElUmbral_GuardaLaConfiguracion()
    {
        var manager = CreateManager(
            Snapshot(50),
            new MemoryAutoReleaseOptions(false, 80));

        var viewModel = CreateViewModel(manager);
        await viewModel.InitializationTask;

        viewModel.AutoReleaseThreshold = 90;
        await viewModel.PendingSaveTask;

        manager.Verify(
            m => m.SaveAutoReleaseOptionsAsync(
                It.Is<MemoryAutoReleaseOptions>(
                    options => !options.Enabled && options.ThresholdPercent == 90),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UmbralFueraDeRango_SeLimita()
    {
        var manager = CreateManager(
            Snapshot(50),
            new MemoryAutoReleaseOptions(false, 80));

        var viewModel = CreateViewModel(manager);
        await viewModel.InitializationTask;

        viewModel.AutoReleaseThreshold = 99;
        await viewModel.PendingSaveTask;

        Assert.Equal(95, viewModel.AutoReleaseThreshold);

        viewModel.AutoReleaseThreshold = 60;
        await viewModel.PendingSaveTask;

        Assert.Equal(70, viewModel.AutoReleaseThreshold);

        manager.Verify(
            m => m.SaveAutoReleaseOptionsAsync(
                It.Is<MemoryAutoReleaseOptions>(options => options.ThresholdPercent == 95),
                It.IsAny<CancellationToken>()),
            Times.Once);

        manager.Verify(
            m => m.SaveAutoReleaseOptionsAsync(
                It.Is<MemoryAutoReleaseOptions>(options => options.ThresholdPercent == 70),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LiberacionAutomatica_ActualizaElEstado()
    {
        var autoResult = new MemoryReleaseResult(
            true,
            80,
            70,
            10,
            256L * 1024 * 1024,
            "Liberación automática ejecutada.");

        var manager = CreateManager(
            Snapshot(85),
            new MemoryAutoReleaseOptions(true, 80),
            autoResult);

        var viewModel = CreateViewModel(manager);

        await viewModel.InitializationTask;

        Assert.Contains("Liberación automática", viewModel.LastReleaseText);
        Assert.Contains("70 de 80", viewModel.LastReleaseText);
        Assert.Equal(autoResult.Message, viewModel.StatusMessage);

        manager.Verify(
            m => m.TryAutoReleaseAsync(
                It.IsAny<MemorySnapshot>(),
                It.Is<MemoryAutoReleaseOptions>(
                    options => options.Enabled && options.ThresholdPercent == 80),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LiberacionAutomatica_Desactivada_NoSeEjecuta()
    {
        var manager = CreateManager(
            Snapshot(95),
            new MemoryAutoReleaseOptions(false, 80));

        var viewModel = CreateViewModel(manager);

        await viewModel.InitializationTask;

        Assert.Equal("Sin liberaciones todavía.", viewModel.LastReleaseText);

        manager.Verify(
            m => m.TryAutoReleaseAsync(
                It.IsAny<MemorySnapshot>(),
                It.Is<MemoryAutoReleaseOptions>(
                    options => !options.Enabled && options.ThresholdPercent == 80),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AutoReleaseSummaryText_ReflejaElEstado()
    {
        var manager = CreateManager(
            Snapshot(50),
            new MemoryAutoReleaseOptions(false, 80));

        var viewModel = CreateViewModel(manager);
        await viewModel.InitializationTask;

        Assert.Contains("desactivada", viewModel.AutoReleaseSummaryText);

        viewModel.AutoReleaseEnabled = true;
        viewModel.AutoReleaseThreshold = 90;

        Assert.Contains("alcance o supere", viewModel.AutoReleaseSummaryText);
        Assert.Contains("90", viewModel.AutoReleaseSummaryText);
        Assert.Equal("90 %", viewModel.AutoReleaseThresholdText);

        await viewModel.PendingSaveTask;
    }

    private static MemoryViewModel CreateViewModel(Mock<IMemoryManager> manager) =>
        new(manager.Object, startTimer: false);

    private static Mock<IMemoryManager> CreateManager(
        MemorySnapshot snapshot,
        MemoryAutoReleaseOptions? options = null,
        MemoryReleaseResult? autoReleaseResult = null)
    {
        var manager = new Mock<IMemoryManager>();

        manager
            .Setup(m => m.GetSnapshotAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(snapshot);

        manager
            .Setup(m => m.GetAutoReleaseOptionsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(options ?? new MemoryAutoReleaseOptions(false, 80));

        manager
            .Setup(m => m.TryAutoReleaseAsync(
                It.IsAny<MemorySnapshot>(),
                It.IsAny<MemoryAutoReleaseOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(autoReleaseResult);

        manager
            .Setup(m => m.SaveAutoReleaseOptionsAsync(
                It.IsAny<MemoryAutoReleaseOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return manager;
    }

    private static MemorySnapshot Snapshot(double usagePercent) =>
        new(
            16L * 1024 * 1024 * 1024,
            4L * 1024 * 1024 * 1024,
            usagePercent,
            DateTimeOffset.Now);
}
