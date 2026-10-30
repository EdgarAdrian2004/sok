using SOK.Domain.Enums;

namespace SOK.Domain.Entities;

/// <summary>
/// Metadatos de un respaldo. El archivo físico vive en el sistema de
/// archivos y aquí se conserva su ruta y su hash SHA-256.
/// </summary>
public sealed class Backup
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public BackupType BackupType { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public string HashSha256 { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
