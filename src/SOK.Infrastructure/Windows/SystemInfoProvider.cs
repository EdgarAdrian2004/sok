using System.Management;
using System.Runtime.Versioning;
using SOK.Application.Abstractions;
using SOK.Application.DTOs;

namespace SOK.Infrastructure.Windows;

/// <summary>
/// Obtiene el estado del equipo mediante WMI (Win32_OperatingSystem,
/// Win32_Processor y Win32_LogicalDisk).
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class SystemInfoProvider : ISystemInfoProvider
{
    public Task<SystemSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(ReadSnapshot, cancellationToken);
    }

    private static SystemSnapshot ReadSnapshot()
    {
        var osName = "Windows";
        var processorName = "Procesador no disponible";
        var machineName = Environment.MachineName;
        var cpuUsage = 0d;
        long ramTotalBytes = 0;
        long ramFreeBytes = 0;
        long diskTotalBytes = 0;
        long diskFreeBytes = 0;
        var bootTime = DateTime.Now;

        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT Caption, TotalVisibleMemorySize, FreePhysicalMemory, LastBootUpTime FROM Win32_OperatingSystem");
            using var results = searcher.Get();

            foreach (ManagementBaseObject item in results)
            {
                using var os = item;
                osName = os["Caption"]?.ToString()?.Trim() ?? osName;
                ramTotalBytes = Convert.ToInt64(os["TotalVisibleMemorySize"] ?? 0) * 1024;
                ramFreeBytes = Convert.ToInt64(os["FreePhysicalMemory"] ?? 0) * 1024;

                var bootValue = os["LastBootUpTime"]?.ToString();
                if (!string.IsNullOrWhiteSpace(bootValue))
                {
                    try
                    {
                        bootTime = ManagementDateTimeConverter.ToDateTime(bootValue);
                    }
                    catch
                    {
                        // Si WMI devuelve un formato inesperado se conserva la hora actual.
                    }
                }

                break;
            }
        }
        catch
        {
            // El panel muestra valores parciales si WMI no está disponible.
        }

        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT Name, LoadPercentage FROM Win32_Processor");
            using var results = searcher.Get();

            foreach (ManagementBaseObject item in results)
            {
                using var processor = item;
                processorName = processor["Name"]?.ToString()?.Trim() ?? processorName;
                if (processor["LoadPercentage"] is not null)
                {
                    cpuUsage = Convert.ToDouble(processor["LoadPercentage"]);
                }

                break;
            }
        }
        catch
        {
            // Se conserva el valor por defecto.
        }

        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT DeviceID, Size, FreeSpace FROM Win32_LogicalDisk WHERE DriveType = 3");
            using var results = searcher.Get();

            ManagementBaseObject? selectedDisk = null;
            foreach (ManagementBaseObject item in results)
            {
                var deviceId = item["DeviceID"]?.ToString();
                if (string.Equals(deviceId, "C:", StringComparison.OrdinalIgnoreCase))
                {
                    selectedDisk = item;
                    break;
                }

                selectedDisk ??= item;
            }

            if (selectedDisk is not null)
            {
                using (selectedDisk)
                {
                    diskTotalBytes = Convert.ToInt64(selectedDisk["Size"] ?? 0);
                    diskFreeBytes = Convert.ToInt64(selectedDisk["FreeSpace"] ?? 0);
                }
            }
        }
        catch
        {
            // Se conservan los valores por defecto.
        }

        var uptime = DateTime.Now - bootTime;
        if (uptime < TimeSpan.Zero)
        {
            uptime = TimeSpan.Zero;
        }

        return new SystemSnapshot(
            osName,
            processorName,
            machineName,
            Math.Clamp(cpuUsage, 0, 100),
            ramTotalBytes,
            Math.Max(0, ramTotalBytes - ramFreeBytes),
            diskTotalBytes,
            diskFreeBytes,
            uptime);
    }
}
