namespace SOK.Domain.Enums;

/// <summary>
/// Estado de configuración observado de un servicio respecto del catálogo
/// de SOK.
/// </summary>
public enum ServiceStatus
{
    /// <summary>El servicio existe y su tipo de inicio no es «Deshabilitado».</summary>
    Habilitado,

    /// <summary>El servicio existe y su tipo de inicio es «Deshabilitado».</summary>
    Deshabilitado,

    /// <summary>El servicio no existe en este equipo.</summary>
    NoAplicable,

    /// <summary>El servicio existe pero su tipo de inicio no se pudo determinar.</summary>
    Desconocido
}
