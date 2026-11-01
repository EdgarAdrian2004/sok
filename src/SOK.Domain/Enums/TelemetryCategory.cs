namespace SOK.Domain.Enums;

/// <summary>
/// Categoría funcional de una telemetría. Agrupa las claves del registro
/// que persiguen el mismo objetivo de privacidad.
/// </summary>
public enum TelemetryCategory
{
    Sistema,
    Cortana,
    Defender,
    Publicidad,
    Privacidad,
    Edge,
    ReporteDeErrores,
    OneDrive,
    Sugerencias,
    Actividad,
    Comentarios
}
