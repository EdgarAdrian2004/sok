using System.Management;
using System.Runtime.Versioning;
using System.Text.Json;
using SOK.Application.Abstractions;

namespace SOK.Infrastructure.Windows;

/// <summary>
/// Exporta el estado de los servicios de Windows a un archivo JSON.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class ServicesBackupService : IServicesBackupService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public async Task<string> CreateBackupAsync(string destinationDirectory, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);
        Directory.CreateDirectory(destinationDirectory);

        var services = new List<ServiceBackupEntry>();

        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT Name, DisplayName, StartMode, State FROM Win32_Service");
            using var results = searcher.Get();

            foreach (ManagementBaseObject item in results)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var service = item;
                services.Add(new ServiceBackupEntry(
                    service["Name"]?.ToString() ?? string.Empty,
                    service["DisplayName"]?.ToString() ?? string.Empty,
                    service["StartMode"]?.ToString() ?? string.Empty,
                    service["State"]?.ToString() ?? string.Empty));
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("No se pudo consultar los servicios de Windows.", ex);
        }

        var filePath = Path.Combine(destinationDirectory, "services-backup.json");
        var json = JsonSerializer.Serialize(services, JsonOptions);
        await File.WriteAllTextAsync(filePath, json, cancellationToken).ConfigureAwait(false);

        return filePath;
    }

    private sealed record ServiceBackupEntry(
        string Name,
        string DisplayName,
        string StartMode,
        string State);
}
