namespace SOK.Application.DTOs;

/// <summary>
/// Fotografía del estado del equipo en un instante determinado.
/// </summary>
public sealed record SystemSnapshot(
    string OsName,
    string ProcessorName,
    string MachineName,
    double CpuUsagePercent,
    long RamTotalBytes,
    long RamUsedBytes,
    long DiskTotalBytes,
    long DiskFreeBytes,
    TimeSpan Uptime)
{
    public double RamUsagePercent =>
        RamTotalBytes <= 0 ? 0 : Math.Clamp((double)RamUsedBytes / RamTotalBytes * 100, 0, 100);

    public double DiskUsagePercent =>
        DiskTotalBytes <= 0 ? 0 : Math.Clamp((double)(DiskTotalBytes - DiskFreeBytes) / DiskTotalBytes * 100, 0, 100);
}
