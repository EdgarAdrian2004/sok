using SOK.Domain.Enums;

namespace SOK.Domain.Entities;

/// <summary>
/// Definición de un servicio del catálogo de SOK junto con el estado
/// observado en el equipo. La definición es estática; el estado se
/// actualiza cada vez que se consulta el Service Control Manager.
/// </summary>
public sealed class ServiceEntry
{
    /// <summary>Nombre interno del servicio (por ejemplo, <c>DiagTrack</c>).</summary>
    public string Id { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public ServiceCategory Category { get; init; }

    /// <summary>
    /// Tipo de inicio que SOK restaura cuando el servicio vuelve a
    /// habilitarse.
    /// </summary>
    public ServiceStartMode DefaultStartMode { get; init; } = ServiceStartMode.Manual;

    /// <summary>Indica si el perfil Gamer deshabilita este servicio.</summary>
    public bool DisabledInGamer { get; init; }

    /// <summary>Indica si el perfil Ofimático deshabilita este servicio.</summary>
    public bool DisabledInOfimatico { get; init; }

    /// <summary>
    /// Indica si deshabilitar el servicio puede afectar una función que el
    /// usuario percibe directamente (impresión, Bluetooth, búsqueda, etc.).
    /// </summary>
    public bool HighRisk { get; init; }

    /// <summary>Indica si el cambio surte efecto tras reiniciar Windows.</summary>
    public bool RequiresRestart { get; init; }

    /// <summary>Indica si el servicio existe en este equipo.</summary>
    public bool Exists { get; set; }

    /// <summary>Tipo de inicio observado en el equipo.</summary>
    public ServiceStartMode StartMode { get; set; } = ServiceStartMode.Desconocido;

    /// <summary>Estado de ejecución observado en el equipo.</summary>
    public ServiceState State { get; set; } = ServiceState.Desconocido;

    /// <summary>Estado de configuración derivado de la lectura del SCM.</summary>
    public ServiceStatus Status
    {
        get
        {
            if (!Exists)
            {
                return ServiceStatus.NoAplicable;
            }

            return StartMode switch
            {
                ServiceStartMode.Deshabilitado => ServiceStatus.Deshabilitado,
                ServiceStartMode.Desconocido => ServiceStatus.Desconocido,
                _ => ServiceStatus.Habilitado
            };
        }
    }

    /// <summary>
    /// Indica si el perfil indicado deshabilita este servicio.
    /// </summary>
    public bool IsDisabledByProfile(ServiceProfile profile) =>
        profile switch
        {
            ServiceProfile.Gamer => DisabledInGamer,
            ServiceProfile.Ofimatico => DisabledInOfimatico,
            _ => false
        };
}
