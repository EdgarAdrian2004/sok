namespace SOK.Application.DTOs;

/// <summary>
/// Resultado de una operación de servicios (cambio individual o aplicación
/// de perfil). Incluye el respaldo creado y los fallos parciales.
/// </summary>
public sealed record ServiceOperationResult(
    bool Success,
    int AppliedCount,
    int FailedCount,
    string? BackupFilePath,
    IReadOnlyList<string> Failures,
    string Message)
{
    public static ServiceOperationResult NoChange(string message) =>
        new(true, 0, 0, null, Array.Empty<string>(), message);

    public static ServiceOperationResult Completed(
        int appliedCount,
        int failedCount,
        string? backupFilePath,
        IReadOnlyList<string> failures,
        string message) =>
        new(failedCount == 0, appliedCount, failedCount, backupFilePath, failures, message);
}
