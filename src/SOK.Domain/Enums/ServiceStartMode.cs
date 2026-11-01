namespace SOK.Domain.Enums;

/// <summary>
/// Tipo de inicio de un servicio de Windows. Se usan nombres en español
/// para mantener la coherencia con el resto del dominio.
/// </summary>
public enum ServiceStartMode
{
    /// <summary>El servicio arranca automáticamente con Windows.</summary>
    Automatico,

    /// <summary>El servicio se inicia manualmente o cuando otra aplicación lo solicita.</summary>
    Manual,

    /// <summary>El servicio está deshabilitado y no puede iniciarse.</summary>
    Deshabilitado,

    /// <summary>El tipo de inicio no se pudo determinar.</summary>
    Desconocido
}
