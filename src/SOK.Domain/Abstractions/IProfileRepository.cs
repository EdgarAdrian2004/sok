using SOK.Domain.Entities;

namespace SOK.Domain.Abstractions;

/// <summary>
/// Contrato de persistencia para los perfiles de optimización.
/// </summary>
public interface IProfileRepository
{
    Task<IReadOnlyList<Profile>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Profile?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Profile?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<int> AddAsync(Profile profile, CancellationToken cancellationToken = default);

    Task UpdateAsync(Profile profile, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
