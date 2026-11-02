using SOK.Application.DTOs;
using SOK.Domain.Entities;

namespace SOK.Application.Abstractions;

/// <summary>
/// Fachada del módulo de telemetría y privacidad. Expone el catálogo con
/// su estado actual y las operaciones que modifican el sistema, siempre
/// con respaldo previo.
/// </summary>
public interface ITelemetryManager
{
    /// <summary>
    /// Devuelve el catálogo completo con el estado observado en el equipo.
    /// </summary>
    Task<IReadOnlyList<TelemetryKey>> GetCatalogAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deshabilita o habilita una clave del catálogo. Genera un respaldo
    /// antes de escribir el registro.
    /// </summary>
    Task<TelemetryOperationResult> SetDisabledAsync(
        string keyId,
        bool disabled,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aplica el valor deshabilitado a todas las claves que aún no lo
    /// tengan. Genera un único respaldo antes de escribir el registro.
    /// </summary>
    Task<TelemetryOperationResult> ApplyMaxPrivacyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deshabilita o habilita un conjunto explícito de claves del catálogo.
    /// Genera un único respaldo antes de escribir el registro. Lo usa el
    /// módulo de perfiles para aplicar sus ajustes de telemetría.
    /// </summary>
    Task<TelemetryOperationResult> ApplyDisabledAsync(
        IReadOnlyCollection<string> keyIds,
        bool disabled,
        CancellationToken cancellationToken = default);
}
