using SOK.Domain.Enums;

namespace SOK.Application.Abstractions;

/// <summary>
/// Registra acciones en la bitácora de SOK.
/// </summary>
public interface ILogService
{
    Task LogAsync(
        LogModule module,
        string action,
        string? targetKey,
        ActionResult result,
        string? detail = null,
        int? backupId = null,
        CancellationToken cancellationToken = default);
}
