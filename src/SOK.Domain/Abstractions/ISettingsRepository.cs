using SOK.Domain.Entities;

namespace SOK.Domain.Abstractions;

/// <summary>
/// Contrato de persistencia para los pares clave/valor de configuración.
/// </summary>
public interface ISettingsRepository
{
    Task<Setting?> GetAsync(string key, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Setting>> GetAllAsync(CancellationToken cancellationToken = default);

    Task SetAsync(string key, string value, CancellationToken cancellationToken = default);

    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}
