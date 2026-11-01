using SOK.Application.DTOs;

namespace SOK.Application.Abstractions;

/// <summary>
/// Fachada del módulo de memoria: medición en tiempo real, liberación manual
/// y liberación automática cuando el uso de RAM supera el umbral configurado.
/// </summary>
public interface IMemoryManager
{
    /// <summary>Devuelve una fotografía del uso de memoria física.</summary>
    Task<MemorySnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default);

    /// <summary>Libera el working set de los procesos activos.</summary>
    Task<MemoryReleaseResult> ReleaseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Evalúa el umbral y, si se cumple, ejecuta una liberación automática.
    /// Devuelve <c>null</c> cuando no corresponde liberar memoria.
    /// </summary>
    Task<MemoryReleaseResult?> TryAutoReleaseAsync(
        MemorySnapshot snapshot,
        MemoryAutoReleaseOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>Lee la configuración persistida de la liberación automática.</summary>
    Task<MemoryAutoReleaseOptions> GetAutoReleaseOptionsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Guarda la configuración de la liberación automática.</summary>
    Task SaveAutoReleaseOptionsAsync(
        MemoryAutoReleaseOptions options,
        CancellationToken cancellationToken = default);
}
