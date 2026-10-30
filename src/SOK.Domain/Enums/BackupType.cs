namespace SOK.Domain.Enums;

/// <summary>
/// Tipo de respaldo almacenado. Coincide con los valores permitidos por
/// la columna Backups.BackupType.
/// </summary>
public enum BackupType
{
    Registro,
    Servicios,
    Completo
}
