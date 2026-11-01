using SOK.Domain.Enums;

namespace SOK.Domain.Entities;

/// <summary>
/// Definición de una clave de telemetría del catálogo de SOK junto con el
/// estado observado en el equipo. La definición es estática; el estado se
/// actualiza cada vez que se consulta el registro de Windows.
/// </summary>
public sealed class TelemetryKey
{
    public string Id { get; init; } = string.Empty;

    public TelemetryCategory Category { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    /// <summary>Ruta completa de la clave, por ejemplo <c>HKCU\Software\...</c>.</summary>
    public string RegistryPath { get; init; } = string.Empty;

    public string ValueName { get; init; } = string.Empty;

    public TelemetryValueKind ValueKind { get; init; }

    /// <summary>Valor que se escribe cuando la telemetría se deshabilita.</summary>
    public string DisabledValue { get; init; } = string.Empty;

    /// <summary>Valor que se escribe cuando la telemetría se habilita.</summary>
    public string EnabledValue { get; init; } = string.Empty;

    /// <summary>Indica si la clave vive en HKLM y requiere elevación.</summary>
    public bool RequiresAdmin { get; init; }

    /// <summary>Indica si Windows necesita reiniciarse (o reiniciar el Explorador) para aplicar el cambio.</summary>
    public bool RequiresRestart { get; init; }

    /// <summary>Estado observado en el equipo; se recalcula al consultar el registro.</summary>
    public TelemetryStatus Status { get; set; } = TelemetryStatus.Desconocido;

    /// <summary>Ruta completa de la clave y el valor, útil para la bitácora.</summary>
    public string TargetKey => $@"{RegistryPath}\{ValueName}";
}
