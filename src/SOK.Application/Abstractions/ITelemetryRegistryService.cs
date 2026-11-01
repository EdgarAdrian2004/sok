using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Application.Abstractions;

/// <summary>
/// Acceso de bajo nivel al registro de Windows para leer y escribir los
/// valores del catálogo de telemetría.
/// </summary>
public interface ITelemetryRegistryService
{
    /// <summary>
    /// Lee un valor del registro. Devuelve <c>null</c> si la clave o el
    /// valor no existen en el equipo.
    /// </summary>
    Task<TelemetryValue?> ReadValueAsync(
        string registryPath,
        string valueName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Escribe un valor del registro. Crea la clave si no existe.
    /// </summary>
    Task WriteValueAsync(
        string registryPath,
        string valueName,
        string value,
        TelemetryValueKind kind,
        CancellationToken cancellationToken = default);
}
