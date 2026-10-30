using SOK.Domain.Entities;

namespace SOK.Domain.Abstractions;

/// <summary>
/// Contrato de persistencia para los ajustes que componen un perfil.
/// </summary>
public interface IProfileSettingRepository
{
    Task<IReadOnlyList<ProfileSetting>> GetByProfileIdAsync(int profileId, CancellationToken cancellationToken = default);

    Task<int> AddAsync(ProfileSetting setting, CancellationToken cancellationToken = default);

    Task DeleteByProfileIdAsync(int profileId, CancellationToken cancellationToken = default);
}
