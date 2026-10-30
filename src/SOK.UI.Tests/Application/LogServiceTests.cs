using Moq;
using SOK.Application.Services;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;
using Xunit;

namespace SOK.UI.Tests.Application;

public sealed class LogServiceTests
{
    [Fact]
    public async Task LogAsync_ConstruyeLaEntradaYDelegaEnElRepositorio()
    {
        LogEntry? captured = null;
        var repository = new Mock<ILogRepository>();
        repository
            .Setup(r => r.AddAsync(It.IsAny<LogEntry>(), It.IsAny<CancellationToken>()))
            .Callback<LogEntry, CancellationToken>((entry, _) => captured = entry)
            .ReturnsAsync(7);

        var service = new LogService(repository.Object);

        await service.LogAsync(
            LogModule.Respaldos,
            "  Crear respaldo  ",
            "  C:\\temp  ",
            ActionResult.Exitoso,
            "  ok  ",
            3);

        Assert.NotNull(captured);
        Assert.Equal(LogModule.Respaldos, captured!.Module);
        Assert.Equal("Crear respaldo", captured.Action);
        Assert.Equal("C:\\temp", captured.TargetKey);
        Assert.Equal(ActionResult.Exitoso, captured.Result);
        Assert.Equal("ok", captured.Detail);
        Assert.Equal(3, captured.BackupId);

        repository.Verify(
            r => r.AddAsync(It.IsAny<LogEntry>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LogAsync_RechazaAccionesVacias()
    {
        var repository = new Mock<ILogRepository>();
        var service = new LogService(repository.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.LogAsync(LogModule.Dashboard, "   ", null, ActionResult.Fallido));
    }
}
