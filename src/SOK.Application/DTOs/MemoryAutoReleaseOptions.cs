namespace SOK.Application.DTOs;

/// <summary>
/// Configuración de la liberación automática de memoria. El umbral se
/// expresa como porcentaje de uso de RAM y se limita al rango permitido.
/// </summary>
public sealed record MemoryAutoReleaseOptions(bool Enabled, int ThresholdPercent)
{
    public const int MinThresholdPercent = 70;
    public const int MaxThresholdPercent = 95;
    public const int DefaultThresholdPercent = 80;

    /// <summary>Configuración inicial: liberación desactivada y umbral del 80 %.</summary>
    public static MemoryAutoReleaseOptions Default { get; } =
        new(false, DefaultThresholdPercent);

    /// <summary>Devuelve una copia con el umbral dentro del rango permitido.</summary>
    public MemoryAutoReleaseOptions Normalized() =>
        this with
        {
            ThresholdPercent = Math.Clamp(
                ThresholdPercent,
                MinThresholdPercent,
                MaxThresholdPercent)
        };

    /// <summary>Indica si el uso observado alcanza el umbral configurado.</summary>
    public bool IsThresholdMet(double usagePercent) =>
        Enabled && usagePercent >= ThresholdPercent;
}
