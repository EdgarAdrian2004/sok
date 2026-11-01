namespace SOK.Domain.Enums;

/// <summary>
/// Perfil de optimización que agrupa un conjunto de servicios.
/// </summary>
public enum ServiceProfile
{
    /// <summary>Prioriza el rendimiento en juegos; conserva Game Bar y Bluetooth.</summary>
    Gamer,

    /// <summary>Prioriza el ahorro de recursos en tareas de oficina; conserva la usabilidad.</summary>
    Ofimatico
}
