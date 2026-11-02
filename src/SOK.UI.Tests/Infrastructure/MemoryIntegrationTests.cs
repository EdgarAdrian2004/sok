using SOK.Infrastructure.Windows;
using Xunit;

namespace SOK.UI.Tests.Infrastructure;

/// <summary>
/// Pruebas de integración con las API de memoria de Windows. Solo leen el
/// estado del equipo y liberan working sets; no requieren elevación ni
/// modifican la configuración persistente.
/// </summary>
public sealed class MemoryIntegrationTests
{
    [Fact]
    public async Task MemoryProvider_DevuelveValoresCoherentes()
    {
        var provider = new MemoryProvider();

        var snapshot = await provider.GetSnapshotAsync();

        Assert.True(snapshot.TotalBytes > 0);
        Assert.InRange(snapshot.AvailableBytes, 0, snapshot.TotalBytes);
        Assert.InRange(snapshot.UsagePercent, 0, 100);
        Assert.InRange(snapshot.UsedBytes, 0, snapshot.TotalBytes);
    }

    [Fact]
    public async Task MemoryProvider_ConservaElTotalEntreLecturas()
    {
        var provider = new MemoryProvider();

        var first = await provider.GetSnapshotAsync();
        var second = await provider.GetSnapshotAsync();

        Assert.Equal(first.TotalBytes, second.TotalBytes);
    }

    [Fact]
    public async Task MemoryProvider_RegistraLaHoraDeCaptura()
    {
        var provider = new MemoryProvider();
        var before = DateTimeOffset.Now.AddMinutes(-1);

        var snapshot = await provider.GetSnapshotAsync();

        var after = DateTimeOffset.Now.AddMinutes(1);
        Assert.InRange(snapshot.CapturedAt, before, after);
    }

    [Fact]
    public async Task MemoryReleaseService_SeEjecutaSinExcepcion()
    {
        var service = new MemoryReleaseService();

        var result = await service.ReleaseAsync();

        Assert.True(result.ProcessesTried > 0);
        Assert.InRange(result.ProcessesSucceeded, 0, result.ProcessesTried);
        Assert.InRange(result.ProcessesFailed, 0, result.ProcessesTried);
        Assert.True(result.FreedBytesEstimate >= 0);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }
}
