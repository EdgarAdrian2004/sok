using System.Runtime.Versioning;
using SOK.Application.Abstractions;
using SOK.Application.DTOs;

namespace SOK.Infrastructure.Windows;

/// <summary>
/// Obtiene el estado de la memoria física mediante
/// <c>GlobalMemoryStatusEx</c>.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class MemoryProvider : IMemoryProvider
{
    public Task<MemorySnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default) =>
        Task.Run(ReadSnapshot, cancellationToken);

    private static MemorySnapshot ReadSnapshot()
    {
        var status = NativeMemoryMethods.GetMemoryStatus();

        return new MemorySnapshot(
            ToInt64(status.TotalPhys),
            ToInt64(status.AvailPhys),
            Math.Clamp((double)status.MemoryLoad, 0, 100),
            DateTimeOffset.Now);
    }

    private static long ToInt64(ulong value) =>
        value > long.MaxValue ? long.MaxValue : (long)value;
}
