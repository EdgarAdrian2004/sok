using SOK.Domain.Entities;

namespace SOK.Domain.Abstractions;

/// <summary>
/// Contrato de persistencia para los metadatos de respaldos.
/// </summary>
public interface IBackupRepository
{
    Task<IReadOnlyList<Backup>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Backup?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> AddAsync(Backup backup, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
