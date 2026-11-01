namespace SOK.Application.DTOs;

/// <summary>
/// Resultado de una liberación de memoria manual o automática. Incluye el
/// número de procesos intentados, los que aceptaron la operación y una
/// estimación de la memoria recuperada.
/// </summary>
public sealed record MemoryReleaseResult(
    bool Success,
    int ProcessesTried,
    int ProcessesSucceeded,
    int ProcessesFailed,
    long FreedBytesEstimate,
    string Message)
{
    /// <summary>Crea un resultado fallido sin procesos procesados.</summary>
    public static MemoryReleaseResult Failed(string message) =>
        new(false, 0, 0, 0, 0, message);
}
