using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text.Json;
using SOK.Application.Abstractions;
using SOK.Application.Common;

namespace SOK.Infrastructure.Windows;

/// <summary>
/// Exporta claves del registro de Windows con reg.exe y genera un
/// manifiesto con el hash SHA-256 de cada archivo .reg.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class RegistryBackupService : IRegistryBackupService
{
    private static readonly string[] RegistryKeys =
    {
        @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
        @"HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
        @"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection",
        @"HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection",
        @"HKLM\SYSTEM\CurrentControlSet\Services\DiagTrack"
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public async Task<string> CreateBackupAsync(string destinationDirectory, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);

        var registryDirectory = Path.Combine(destinationDirectory, "registry");
        Directory.CreateDirectory(registryDirectory);

        var entries = new List<RegistryBackupEntry>();

        foreach (var key in RegistryKeys)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fileName = SanitizeFileName(key) + ".reg";
            var filePath = Path.Combine(registryDirectory, fileName);
            var (success, error) = await ExportKeyAsync(key, filePath, cancellationToken).ConfigureAwait(false);

            string? hash = null;
            if (success && File.Exists(filePath))
            {
                hash = await Sha256Helper.ComputeFileHashAsync(filePath, cancellationToken).ConfigureAwait(false);
            }

            entries.Add(new RegistryBackupEntry(
                key,
                success ? fileName : null,
                hash,
                error));
        }

        if (entries.All(entry => entry.HashSha256 is null))
        {
            throw new InvalidOperationException(
                "No se pudo exportar ninguna clave del registro. Ejecute SOK con permisos de administrador.");
        }

        var manifestPath = Path.Combine(registryDirectory, "registry-manifest.json");
        var manifestJson = JsonSerializer.Serialize(entries, JsonOptions);
        await File.WriteAllTextAsync(manifestPath, manifestJson, cancellationToken).ConfigureAwait(false);

        return manifestPath;
    }

    public async Task<string> CreateBackupForKeysAsync(
        IReadOnlyCollection<string> registryKeys,
        string destinationDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(registryKeys);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);

        var registryDirectory = Path.Combine(destinationDirectory, "registry");
        Directory.CreateDirectory(registryDirectory);

        var entries = new List<RegistryBackupEntry>();

        foreach (var key in registryKeys.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!RegistryPathHelper.KeyExists(key))
            {
                entries.Add(new RegistryBackupEntry(
                    key,
                    null,
                    null,
                    "La clave no existe en este equipo; no requiere respaldo."));
                continue;
            }

            var fileName = SanitizeFileName(key) + ".reg";
            var filePath = Path.Combine(registryDirectory, fileName);
            var (success, error) = await ExportKeyAsync(key, filePath, cancellationToken).ConfigureAwait(false);

            if (!success)
            {
                // La regla de oro exige respaldar antes de modificar: si una
                // clave existente no se puede exportar, se aborta la operación.
                throw new InvalidOperationException(
                    $"No se pudo respaldar la clave '{key}': {error}");
            }

            var hash = await Sha256Helper
                .ComputeFileHashAsync(filePath, cancellationToken)
                .ConfigureAwait(false);

            entries.Add(new RegistryBackupEntry(key, fileName, hash, null));
        }

        var manifestPath = Path.Combine(registryDirectory, "registry-manifest.json");
        var manifestJson = JsonSerializer.Serialize(entries, JsonOptions);
        await File.WriteAllTextAsync(manifestPath, manifestJson, cancellationToken).ConfigureAwait(false);

        return manifestPath;
    }

    private static async Task<(bool Success, string? Error)> ExportKeyAsync(
        string key,
        string filePath,
        CancellationToken cancellationToken)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "reg.exe",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            startInfo.ArgumentList.Add("export");
            startInfo.ArgumentList.Add(key);
            startInfo.ArgumentList.Add(filePath);
            startInfo.ArgumentList.Add("/y");

            using var process = Process.Start(startInfo);
            if (process is null)
            {
                return (false, "No se pudo iniciar reg.exe.");
            }

            var error = await process.StandardError.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

            return process.ExitCode == 0
                ? (true, null)
                : (false, string.IsNullOrWhiteSpace(error) ? $"reg.exe terminó con código {process.ExitCode}." : error.Trim());
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    private static string SanitizeFileName(string key)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var chars = key.Select(c => invalidChars.Contains(c) || c == '\\' ? '_' : c).ToArray();
        return new string(chars);
    }

    private sealed record RegistryBackupEntry(
        string Key,
        string? FileName,
        string? HashSha256,
        string? Error);
}
