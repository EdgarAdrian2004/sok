namespace SOK.Application.DTOs;

/// <summary>
/// Resultado de aplicar un perfil de optimización. Resume los ajustes
/// aplicados por módulo, los fallos parciales y los respaldos creados.
/// </summary>
public sealed record ProfileOperationResult(
    bool Success,
    int TelemetryApplied,
    int TelemetryFailed,
    int ServicesApplied,
    int ServicesFailed,
    bool MemoryApplied,
    IReadOnlyList<string> Failures,
    IReadOnlyList<string> BackupFilePaths,
    string Message)
{
    public static ProfileOperationResult Failed(string message) =>
        new(
            false,
            0,
            0,
            0,
            0,
            false,
            Array.Empty<string>(),
            Array.Empty<string>(),
            message);

    public static ProfileOperationResult Completed(
        int telemetryApplied,
        int telemetryFailed,
        int servicesApplied,
        int servicesFailed,
        bool memoryApplied,
        IReadOnlyList<string> failures,
        IReadOnlyList<string> backupFilePaths,
        string message) =>
        new(
            failures.Count == 0,
            telemetryApplied,
            telemetryFailed,
            servicesApplied,
            servicesFailed,
            memoryApplied,
            failures,
            backupFilePaths,
            message);
}
