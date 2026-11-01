using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Application.Abstractions;

/// <summary>
/// Acceso al Service Control Manager de Windows. Las lecturas se hacen con
/// WMI (<c>Win32_Service</c>) y las acciones con <c>ServiceController</c> y
/// <c>ChangeServiceConfig</c>.
/// </summary>
public interface IServiceControlService
{
    /// <summary>Devuelve todos los servicios instalados en el equipo.</summary>
    Task<IReadOnlyList<ServiceSnapshot>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Devuelve un servicio por su nombre interno, o <c>null</c> si no
    /// existe en el equipo.
    /// </summary>
    Task<ServiceSnapshot?> GetAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Detiene el servicio. Devuelve <c>false</c> si no se pudo detener
    /// (por ejemplo, porque tiene servicios dependientes activos).
    /// </summary>
    Task<bool> TryStopAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>Inicia el servicio. Devuelve <c>false</c> si no se pudo iniciar.</summary>
    Task<bool> TryStartAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>Cambia el tipo de inicio del servicio.</summary>
    Task SetStartModeAsync(
        string name,
        ServiceStartMode mode,
        CancellationToken cancellationToken = default);
}
