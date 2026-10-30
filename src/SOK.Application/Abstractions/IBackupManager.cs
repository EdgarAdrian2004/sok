using SOK.Domain.Entities;

namespace SOK.Application.Abstractions;

/// <summary>
/// Orquesta la creación de un respaldo completo (registro + servicios),
/// calcula su hash y guarda los metadatos en SQLite.
/// </summary>
public interface IBackupManager
{
    Task<Backup> CreateFullBackupAsync(string? description = null, CancellationToken cancellationToken = default);
}
