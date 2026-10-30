using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Domain.Abstractions;

/// <summary>
/// Contrato de persistencia para la bitácora de acciones.
/// </summary>
public interface ILogRepository
{
    Task<int> AddAsync(LogEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LogEntry>> GetRecentAsync(int limit = 100, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LogEntry>> GetByModuleAsync(LogModule module, int limit = 100, CancellationToken cancellationToken = default);
}
