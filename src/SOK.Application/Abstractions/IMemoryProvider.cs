using SOK.Application.DTOs;

namespace SOK.Application.Abstractions;

/// <summary>
/// Obtiene el estado actual de la memoria física del equipo.
/// </summary>
public interface IMemoryProvider
{
    Task<MemorySnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default);
}
