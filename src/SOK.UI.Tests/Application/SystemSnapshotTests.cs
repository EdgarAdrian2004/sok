using SOK.Application.DTOs;
using Xunit;

namespace SOK.UI.Tests.Application;

public sealed class SystemSnapshotTests
{
    [Fact]
    public void Porcentajes_SeCalculanYSeLimitanEntreCeroYCien()
    {
        var snapshot = new SystemSnapshot(
            "Windows",
            "CPU",
            "EQUIPO",
            0,
            RamTotalBytes: 100,
            RamUsedBytes: 50,
            DiskTotalBytes: 200,
            DiskFreeBytes: 50,
            TimeSpan.FromHours(2));

        Assert.Equal(50, snapshot.RamUsagePercent);
        Assert.Equal(75, snapshot.DiskUsagePercent);
    }

    [Fact]
    public void Porcentajes_SonCeroCuandoNoHayCapacidad()
    {
        var snapshot = new SystemSnapshot(
            "Windows",
            "CPU",
            "EQUIPO",
            0,
            RamTotalBytes: 0,
            RamUsedBytes: 0,
            DiskTotalBytes: 0,
            DiskFreeBytes: 0,
            TimeSpan.Zero);

        Assert.Equal(0, snapshot.RamUsagePercent);
        Assert.Equal(0, snapshot.DiskUsagePercent);
    }
}
