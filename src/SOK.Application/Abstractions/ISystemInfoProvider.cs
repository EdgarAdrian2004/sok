using SOK.Application.DTOs;

namespace SOK.Application.Abstractions;

/// <summary>
/// Obtiene el estado actual del sistema (CPU, RAM, disco y datos del equipo).
/// </summary>
public interface ISystemInfoProvider
{
    Task<SystemSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default);
}
