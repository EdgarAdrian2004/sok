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
    private const string SelectAll =
        "SELECT Name, DisplayName, StartMode, State FROM Win32_Service";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public Task<string> CreateBackupAsync(
        string destinationDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);

        return CreateBackupCoreAsync(destinationDirectory, null, cancellationToken);
    }

    public Task<string> CreateBackupForServicesAsync(
        IReadOnlyCollection<string> serviceNames,
        string destinationDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceNames);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);

        var wanted = serviceNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (wanted.Count == 0)
        {
            throw new ArgumentException(
                "Debe indicar al menos un servicio para respaldar.",
                nameof(serviceNames));
        }

        return CreateBackupCoreAsync(destinationDirectory, wanted, cancellationToken);
    }

    private static async Task<string> CreateBackupCoreAsync(
        string destinationDirectory,
        IReadOnlySet<string>? filter,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(destinationDirectory);
        cancellationToken.ThrowIfCancellationRequested();

        var services = ReadServices(filter, cancellationToken);

        if (filter is not null && services.Count == 0)
        {
            throw new InvalidOperationException(
                "Ninguno de los servicios indicados existe en este equipo; " +
                "no se generó el respaldo.");
        }

        var filePath = Path.Combine(destinationDirectory, "services-backup.json");
        var json = JsonSerializer.Serialize(services, JsonOptions);
        await File.WriteAllTextAsync(filePath, json, cancellationToken).ConfigureAwait(false);

        return filePath;
    }

    private static List<ServiceBackupEntry> ReadServices(
        IReadOnlySet<string>? filter,
        CancellationToken cancellationToken)
    {
        var services = new List<ServiceBackupEntry>();

        try
        {
            using var searcher = new ManagementObjectSearcher(SelectAll);
            using var results = searcher.Get();

            foreach (ManagementBaseObject item in results)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var service = item;

                var name = service["Name"]?.ToString() ?? string.Empty;
                if (filter is not null && !filter.Contains(name))
                {
                    continue;
                }

                services.Add(new ServiceBackupEntry(
                    name,
                    service["DisplayName"]?.ToString() ?? string.Empty,
                    service["StartMode"]?.ToString() ?? string.Empty,
                    service["State"]?.ToString() ?? string.Empty));
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "No se pudo consultar los servicios de Windows.",
                ex);
        }

        return services;
    }

    private sealed record ServiceBackupEntry(
        string Name,
        string DisplayName,
        string StartMode,
        string State);
}
