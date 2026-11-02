namespace SOK.Application.DTOs;

/// <summary>
/// Resultado de restaurar un respaldo: claves de registro y servicios
/// devueltos a su estado anterior, más los fallos parciales.
/// </summary>
public sealed record RestoreResult(
    bool Success,
    int RegistryKeysRestored,
    int ServicesRestored,
    IReadOnlyList<string> Failures,
    string Message)
{
    public static RestoreResult Failed(string message) =>
        new(false, 0, 0, Array.Empty<string>(), message);
}
