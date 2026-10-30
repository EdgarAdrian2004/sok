using SOK.Domain.Enums;

namespace SOK.Domain.Entities;

/// <summary>
/// Entrada append-only de la bitácora de acciones de SOK.
/// </summary>
public sealed class LogEntry
{
    public int Id { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public LogModule Module { get; set; }

    public string Action { get; set; } = string.Empty;

    public string? TargetKey { get; set; }

    public ActionResult Result { get; set; }

    public string? Detail { get; set; }

    public int? BackupId { get; set; }
}
