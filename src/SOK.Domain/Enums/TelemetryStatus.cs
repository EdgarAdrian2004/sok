namespace SOK.Domain.Enums;

/// <summary>
/// Estado observado de una clave de telemetría en el equipo.
/// </summary>
public enum TelemetryStatus
{
    /// <summary>El valor actual coincide con el valor esperado cuando la telemetría está habilitada.</summary>
    Habilitado,

    /// <summary>El valor actual coincide con el valor esperado cuando la telemetría está deshabilitada.</summary>
    Deshabilitado,

    /// <summary>La clave o el valor no existen en este equipo.</summary>
    NoAplicable,

    /// <summary>El valor existe pero no coincide con ninguno de los dos valores esperados.</summary>
    Desconocido
}
