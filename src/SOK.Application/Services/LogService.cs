using SOK.Application.Abstractions;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Application.Services;

/// <summary>
/// Implementación de la bitácora: construye la entrada y delega su
/// persistencia en el repositorio.
/// </summary>
public sealed class LogService : ILogService
{
    private readonly ILogRepository _repository;

    public LogService(ILogRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public Task LogAsync(
        LogModule module,
        string action,
        string? targetKey,
        ActionResult result,
        string? detail = null,
        int? backupId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        var entry = new LogEntry
        {
            Timestamp = DateTime.Now,
            Module = module,
            Action = action.Trim(),
            TargetKey = string.IsNullOrWhiteSpace(targetKey) ? null : targetKey.Trim(),
            Result = result,
            Detail = string.IsNullOrWhiteSpace(detail) ? null : detail.Trim(),
            BackupId = backupId
        };

        return _repository.AddAsync(entry, cancellationToken);
    }
}
