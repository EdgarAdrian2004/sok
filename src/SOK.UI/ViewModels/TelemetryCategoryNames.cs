using SOK.Domain.Enums;

namespace SOK.UI.ViewModels;

/// <summary>
/// Nombres en español de las categorías de telemetría que se muestran en
/// la interfaz.
/// </summary>
public static class TelemetryCategoryNames
{
    public static string GetName(TelemetryCategory category) =>
        category switch
        {
            TelemetryCategory.Sistema => "Sistema y diagnóstico",
            TelemetryCategory.Cortana => "Cortana y búsqueda",
            TelemetryCategory.Defender => "Defender y SmartScreen",
            TelemetryCategory.Publicidad => "Publicidad",
            TelemetryCategory.Privacidad => "Privacidad y permisos",
            TelemetryCategory.Edge => "Microsoft Edge",
            TelemetryCategory.ReporteDeErrores => "Reporte de errores",
            TelemetryCategory.OneDrive => "OneDrive",
            TelemetryCategory.Sugerencias => "Sugerencias y contenido",
            TelemetryCategory.Actividad => "Historial de actividad",
            TelemetryCategory.Comentarios => "Comentarios e idioma",
            _ => category.ToString()
        };
}
