namespace SOK.Application.DTOs;

/// <summary>
/// Estado de integridad de un respaldo registrado: si el archivo existe y
/// si su hash SHA-256 coincide con el guardado en la base de datos.
/// </summary>
public sealed record BackupStatus(
    int Id,
    string Name,
    string BackupType,
    string FilePath,
    string Description,
    DateTime CreatedAt,
    bool FileExists,
    bool HashMatches)
{
    /// <summary>Indica si el respaldo se puede restaurar sin riesgo de corrupción.</summary>
    public bool CanRestore => FileExists && HashMatches;

    /// <summary>Texto de integridad listo para la interfaz.</summary>
    public string IntegrityText =>
        !FileExists
            ? "Archivo no encontrado"
            : HashMatches
                ? "Íntegro (SHA-256 verificado)"
                : "Hash alterado";

    /// <summary>Texto del tipo de respaldo listo para la interfaz.</summary>
    public string TypeText =>
        BackupType switch
        {
            "Registro" => "Registro",
            "Servicios" => "Servicios",
            "Completo" => "Completo",
            _ => BackupType
        };
}
