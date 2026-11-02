using SOK.Application.DTOs;

namespace SOK.Application.Abstractions;

/// <summary>
/// Fachada del módulo de restauración: lista los respaldos con su estado
/// de integridad y devuelve el sistema al punto guardado.
/// </summary>
public interface IRestoreManager
{
    /// <summary>
    /// Devuelve todos los respaldos registrados con el resultado de la
    /// verificación SHA-256 de su archivo.
    /// </summary>
    Task<IReadOnlyList<BackupStatus>> GetBackupsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Restaura un respaldo: verifica su hash, importa las claves de
    /// registro y devuelve los servicios a su tipo de inicio anterior.
    /// </summary>
    Task<RestoreResult> RestoreAsync(int backupId, CancellationToken cancellationToken = default);
}
