using SOK.Application.DTOs;
using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Application.Abstractions;

/// <summary>
/// Fachada del módulo de gestión de servicios. Expone el catálogo con su
/// estado actual, el cambio individual y la aplicación de perfiles, siempre
/// con respaldo previo.
/// </summary>
public interface IServiceManager
{
    /// <summary>
    /// Devuelve el catálogo completo con el estado observado en el equipo.
    /// </summary>
    Task<IReadOnlyList<ServiceEntry>> GetCatalogAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deshabilita o habilita un servicio del catálogo. Genera un respaldo
    /// antes de modificar el sistema.
    /// </summary>
    Task<ServiceOperationResult> SetDisabledAsync(
        string serviceName,
        bool disabled,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aplica un perfil (Gamer u Ofimático) a todos los servicios que aún no
    /// estén deshabilitados. Genera un único respaldo antes de escribir.
    /// </summary>
    Task<ServiceOperationResult> ApplyProfileAsync(
        ServiceProfile profile,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deshabilita o habilita un conjunto explícito de servicios del
    /// catálogo. Genera un único respaldo antes de modificar el sistema. Lo
    /// usa el módulo de perfiles para aplicar sus ajustes de servicios.
    /// </summary>
    Task<ServiceOperationResult> ApplyDisabledAsync(
        IReadOnlyCollection<string> serviceNames,
        bool disabled,
        CancellationToken cancellationToken = default);
}
