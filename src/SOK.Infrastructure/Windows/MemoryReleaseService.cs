using System.Runtime.Versioning;
using SOK.Application.Abstractions;
using SOK.Application.DTOs;

namespace SOK.Infrastructure.Windows;

/// <summary>
/// Libera el working set de los procesos activos con <c>EnumProcesses</c>,
/// <c>OpenProcess</c> y <c>EmptyWorkingSet</c>. Los procesos protegidos o sin
/// permisos suficientes se contabilizan como fallidos y no interrumpen la
/// operación.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class MemoryReleaseService : IMemoryReleaseService
{
    public Task<MemoryReleaseResult> ReleaseAsync(CancellationToken cancellationToken = default) =>
        Task.Run(() => ReleaseCore(cancellationToken), cancellationToken);

    private static MemoryReleaseResult ReleaseCore(CancellationToken cancellationToken)
    {
        var availableBefore = ReadAvailableBytes();
        var processIds = NativeMemoryMethods.EnumProcessIds();

        var succeeded = 0;
        var failed = 0;

        foreach (var processId in processIds)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var handle = NativeMemoryMethods.OpenProcess(processId);
            if (handle == IntPtr.Zero)
            {
                failed++;
                continue;
            }

            try
            {
                if (NativeMemoryMethods.EmptyWorkingSet(handle))
                {
                    succeeded++;
                }
                else
                {
                    failed++;
                }
            }
            finally
            {
                NativeMemoryMethods.CloseHandle(handle);
            }
        }

        var availableAfter = ReadAvailableBytes();
        var freedBytes = Math.Max(0, availableAfter - availableBefore);
        var success = succeeded > 0;

        var message = success
            ? $"Se liberó memoria de {succeeded} de {processIds.Count} procesos; " +
              $"{failed} no permitieron acceso."
            : $"No se pudo liberar memoria de ningún proceso " +
              $"({failed} intentos sin acceso).";

        return new MemoryReleaseResult(
            success,
            processIds.Count,
            succeeded,
            failed,
            freedBytes,
            message);
    }

    private static long ReadAvailableBytes()
    {
        var status = NativeMemoryMethods.GetMemoryStatus();
        return status.AvailPhys > long.MaxValue
            ? long.MaxValue
            : (long)status.AvailPhys;
    }
}
