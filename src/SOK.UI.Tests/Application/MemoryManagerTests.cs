using Moq;
using SOK.Application.Abstractions;
using SOK.Application.DTOs;
using SOK.Application.Services;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;
using Xunit;

namespace SOK.UI.Tests.Application;

public sealed class MemoryManagerTests
{
    [Fact]
    public async Task GetSnapshotAsync_DelegaEnElProveedor()
    {
        var snapshot = Snapshot(42);
        var provider = new Mock<IMemoryProvider>();
        provider
            .Setup(p => p.GetSnapshotAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(snapshot);

        var manager = CreateManager(provider: provider);

        var result = await manager.GetSnapshotAsync();

        Assert.Same(snapshot, result);
    }

    [Fact]
    public async Task ReleaseAsync_Exitoso_RegistraLaBitacora()
    {
        var release = new Mock<IMemoryReleaseService>();
        release
            .Setup(r => r.ReleaseAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryReleaseResult(
                true,
                120,
                100,
                20,
                512L * 1024 * 1024,
                "Se liberó memoria."));

        var logs = new Mock<ILogService>();
        var manager = CreateManager(release: release, logs: logs);

        var result = await manager.ReleaseAsync();

        Assert.True(result.Success);
        Assert.Equal(100, result.ProcessesSucceeded);

        logs.Verify(
            l => l.LogAsync(
                LogModule.Memoria,
                "Liberar memoria",
                null,
                ActionResult.Exitoso,
                It.Is<string>(detail => detail.Contains("100 de 120")),
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ReleaseAsync_Fallido_RegistraLaBitacoraComoFallida()
    {
        var release = new Mock<IMemoryReleaseService>();
        release
            .Setup(r => r.ReleaseAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryReleaseResult(
                false,
                50,
                0,
                50,
                0,
                "No se pudo liberar memoria."));

        var logs = new Mock<ILogService>();
        var manager = CreateManager(release: release, logs: logs);

        var result = await manager.ReleaseAsync();

        Assert.False(result.Success);

        logs.Verify(
            l => l.LogAsync(
                LogModule.Memoria,
                "Liberar memoria",
                null,
                ActionResult.Fallido,
                It.IsAny<string>(),
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ReleaseAsync_Excepcion_DevuelveResultadoFallidoYRegistra()
    {
        var release = new Mock<IMemoryReleaseService>();
        release
            .Setup(r => r.ReleaseAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("fallo nativo"));

        var logs = new Mock<ILogService>();
        var manager = CreateManager(release: release, logs: logs);

        var result = await manager.ReleaseAsync();

        Assert.False(result.Success);
        Assert.Contains("fallo nativo", result.Message);

        logs.Verify(
            l => l.LogAsync(
                LogModule.Memoria,
                "Liberar memoria",
                null,
                ActionResult.Fallido,
                It.Is<string>(detail => detail.Contains("fallo nativo")),
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ReleaseAsync_Cancelacion_SePropagaSinRegistrar()
    {
        var release = new Mock<IMemoryReleaseService>();
        release
            .Setup(r => r.ReleaseAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var logs = new Mock<ILogService>();
        var manager = CreateManager(release: release, logs: logs);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => manager.ReleaseAsync());

        logs.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAutoReleaseOptionsAsync_SinConfiguracion_DevuelveLosValoresPorDefecto()
    {
        var manager = CreateManager();

        var options = await manager.GetAutoReleaseOptionsAsync();

        Assert.False(options.Enabled);
        Assert.Equal(MemoryAutoReleaseOptions.DefaultThresholdPercent, options.ThresholdPercent);
    }

    [Fact]
    public async Task GetAutoReleaseOptionsAsync_LeeLasClavesPersistidas()
    {
        var values = new Dictionary<string, string>
        {
            ["Memory.AutoReleaseEnabled"] = "true",
            ["Memory.AutoReleaseThreshold"] = "85"
        };

        var manager = CreateManager(settings: CreateSettingsMock(values));

        var options = await manager.GetAutoReleaseOptionsAsync();

        Assert.True(options.Enabled);
        Assert.Equal(85, options.ThresholdPercent);
    }

    [Fact]
    public async Task GetAutoReleaseOptionsAsync_UsaLaClaveHeredadaDelDdl()
    {
        var values = new Dictionary<string, string>
        {
            ["MemoryReleaseThreshold"] = "75"
        };

        var manager = CreateManager(settings: CreateSettingsMock(values));

        var options = await manager.GetAutoReleaseOptionsAsync();

        Assert.Equal(75, options.ThresholdPercent);
    }

    [Theory]
    [InlineData("50", 70)]
    [InlineData("99", 95)]
    [InlineData("no-numerico", 80)]
    public async Task GetAutoReleaseOptionsAsync_LimitaElUmbral(
        string storedValue,
        int expectedThreshold)
    {
        var values = new Dictionary<string, string>
        {
            ["Memory.AutoReleaseThreshold"] = storedValue
        };

        var manager = CreateManager(settings: CreateSettingsMock(values));

        var options = await manager.GetAutoReleaseOptionsAsync();

        Assert.Equal(expectedThreshold, options.ThresholdPercent);
    }

    [Fact]
    public async Task SaveAutoReleaseOptionsAsync_GuardaYNormaliza()
    {
        var values = new Dictionary<string, string>();
        var manager = CreateManager(settings: CreateSettingsMock(values));

        await manager.SaveAutoReleaseOptionsAsync(
            new MemoryAutoReleaseOptions(true, 99));

        Assert.Equal("true", values["Memory.AutoReleaseEnabled"]);
        Assert.Equal("95", values["Memory.AutoReleaseThreshold"]);
    }

    [Fact]
    public async Task TryAutoReleaseAsync_Desactivada_NoLibera()
    {
        var release = new Mock<IMemoryReleaseService>();
        var manager = CreateManager(release: release);

        var result = await manager.TryAutoReleaseAsync(
            Snapshot(95),
            new MemoryAutoReleaseOptions(false, 80));

        Assert.Null(result);
        release.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task TryAutoReleaseAsync_BajoElUmbral_NoLibera()
    {
        var release = new Mock<IMemoryReleaseService>();
        var manager = CreateManager(release: release);

        var result = await manager.TryAutoReleaseAsync(
            Snapshot(79.9),
            new MemoryAutoReleaseOptions(true, 80));

        Assert.Null(result);
        release.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task TryAutoReleaseAsync_AlcanzaElUmbral_LiberaYRegistra()
    {
        var release = new Mock<IMemoryReleaseService>();
        release
            .Setup(r => r.ReleaseAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryReleaseResult(
                true,
                100,
                80,
                20,
                0,
                "Liberación automática ejecutada."));

        var logs = new Mock<ILogService>();
        var manager = CreateManager(release: release, logs: logs);

        var result = await manager.TryAutoReleaseAsync(
            Snapshot(80),
            new MemoryAutoReleaseOptions(true, 80));

        Assert.NotNull(result);
        Assert.True(result!.Success);

        logs.Verify(
            l => l.LogAsync(
                LogModule.Memoria,
                "Liberación automática",
                null,
                ActionResult.Exitoso,
                It.Is<string>(detail => detail.Contains("80 de 100")),
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task TryAutoReleaseAsync_RespetaElIntervaloMinimo()
    {
        var release = new Mock<IMemoryReleaseService>();
        release
            .Setup(r => r.ReleaseAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryReleaseResult(true, 10, 10, 0, 0, "ok"));

        var time = new FakeTimeProvider();
        var manager = CreateManager(release: release, timeProvider: time);
        var snapshot = Snapshot(90);
        var options = new MemoryAutoReleaseOptions(true, 80);

        var first = await manager.TryAutoReleaseAsync(snapshot, options);

        time.UtcNow = time.UtcNow.AddSeconds(30);
        var second = await manager.TryAutoReleaseAsync(snapshot, options);

        time.UtcNow = time.UtcNow.AddSeconds(31);
        var third = await manager.TryAutoReleaseAsync(snapshot, options);

        Assert.NotNull(first);
        Assert.Null(second);
        Assert.NotNull(third);

        release.Verify(
            r => r.ReleaseAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task TryAutoReleaseAsync_NormalizaElUmbralAntesDeEvaluar()
    {
        var release = new Mock<IMemoryReleaseService>();
        var manager = CreateManager(release: release);

        var result = await manager.TryAutoReleaseAsync(
            Snapshot(65),
            new MemoryAutoReleaseOptions(true, 50));

        Assert.Null(result);
        release.VerifyNoOtherCalls();
    }

    private static MemoryManager CreateManager(
        Mock<IMemoryProvider>? provider = null,
        Mock<IMemoryReleaseService>? release = null,
        Mock<ISettingsRepository>? settings = null,
        Mock<ILogService>? logs = null,
        TimeProvider? timeProvider = null)
    {
        if (provider is null)
        {
            provider = new Mock<IMemoryProvider>();
            provider
                .Setup(p => p.GetSnapshotAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Snapshot(50));
        }

        if (release is null)
        {
            release = new Mock<IMemoryReleaseService>();
            release
                .Setup(r => r.ReleaseAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryReleaseResult(true, 0, 0, 0, 0, "Sin procesos."));
        }

        settings ??= CreateSettingsMock(new Dictionary<string, string>());
        logs ??= new Mock<ILogService>();

        return new MemoryManager(
            provider.Object,
            release.Object,
            settings.Object,
            logs.Object,
            timeProvider);
    }

    private static Mock<ISettingsRepository> CreateSettingsMock(
        Dictionary<string, string> values)
    {
        var settings = new Mock<ISettingsRepository>();

        settings
            .Setup(r => r.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string key, CancellationToken _) =>
                values.TryGetValue(key, out var value)
                    ? new Setting { Key = key, Value = value }
                    : null);

        settings
            .Setup(r => r.SetAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Callback((string key, string value, CancellationToken _) => values[key] = value)
            .Returns(Task.CompletedTask);

        return settings;
    }

    private static MemorySnapshot Snapshot(double usagePercent) =>
        new(
            16L * 1024 * 1024 * 1024,
            4L * 1024 * 1024 * 1024,
            usagePercent,
            DateTimeOffset.Now);

    private sealed class FakeTimeProvider : TimeProvider
    {
        public DateTimeOffset UtcNow { get; set; } =
            new(2026, 11, 1, 12, 0, 0, TimeSpan.Zero);

        public override DateTimeOffset GetUtcNow() => UtcNow;
    }
}
