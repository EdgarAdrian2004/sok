namespace SOK.Application.DTOs;

/// <summary>
/// Fotografía del uso de memoria física del equipo en un instante
/// determinado. Los valores provienen de <c>GlobalMemoryStatusEx</c>.
/// </summary>
public sealed record MemorySnapshot(
    long TotalBytes,
    long AvailableBytes,
    double UsagePercent,
    DateTimeOffset CapturedAt)
{
    /// <summary>Memoria física en uso, calculada a partir del total y lo disponible.</summary>
    public long UsedBytes => Math.Max(0, TotalBytes - AvailableBytes);
}
