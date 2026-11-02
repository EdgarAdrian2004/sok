namespace SOK.Application.DTOs;

/// <summary>
/// Resultado de importar los archivos <c>.reg</c> de un manifiesto de
/// respaldo de registro.
/// </summary>
public sealed record RegistryRestoreOutcome(
    int RestoredCount,
    int SkippedCount,
    IReadOnlyList<string> Failures);
