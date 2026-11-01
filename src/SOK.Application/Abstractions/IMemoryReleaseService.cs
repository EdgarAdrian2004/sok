using SOK.Application.DTOs;

namespace SOK.Application.Abstractions;

/// <summary>
/// Libera el working set de los procesos activos para devolver memoria
/// física al sistema operativo.
/// </summary>
public interface IMemoryReleaseService
{
    Task<MemoryReleaseResult> ReleaseAsync(CancellationToken cancellationToken = default);
}
