using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text.Json;
using SOK.Application.Abstractions;
using SOK.Application.Common;
using SOK.Application.DTOs;

namespace SOK.Infrastructure.Windows;

/// <summary>
/// Importa los archivos <c>.reg</c> de un manifiesto de respaldo con
/// <c>reg.exe</c>. Antes de importar cada archivo verifica su SHA-256
/// contra el valor guardado en el manifiesto.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class RegistryRestoreService : IRegistryRestoreService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<RegistryRestoreOutcome> RestoreFromManifestAsync(
        string registryManifestPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(registryManifestPath);

        if (!File.Exists(registryManifestPath))
        {
            throw new FileNotFoundException(
                "No se encontró el manifiesto del respaldo de registro.",
                registryManifestPath);
        }

        var json = await File
            .ReadAllTextAsync(registryManifestPath, cancellationToken)
            .ConfigureAwait(false);

        var entries = JsonSerializer.Deserialize<List<RegistryBackupEntry>>(json, JsonOptions)
                      ?? new List<RegistryBackupEntry>();

        var registryDirectory = Path.GetDirectoryName(registryManifestPath)
                                ?? throw new InvalidOperationException(
                                    "No se pudo determinar la carpeta del manifiesto de registro.");

        var restored = 0;
        var skipped = 0;
        var failures = new List<string>();

        foreach (var entry in entries)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(entry.FileName))
            {
                // La clave no existía al respaldar: no requiere restauración.
                skipped++;
                continue;
            }

            var filePath = Path.Combine(registryDirectory, entry.FileName);

            if (!File.Exists(filePath))
            {
                failures.Add($"{entry.Key}: no se encontró {entry.FileName}.");
                continue;
            }

            if (!string.IsNullOrWhiteSpace(entry.HashSha256))
            {
                var hash = await Sha256Helper
                    .ComputeFileHashAsync(filePath, cancellationToken)
                    .ConfigureAwait(false);

                if (!string.Equals(hash, entry.HashSha256, StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add($"{entry.Key}: el hash de {entry.FileName} no coincide.");
                    continue;
                }
            }

            var (success, error) = await ImportAsync(filePath, cancellationToken).ConfigureAwait(false);
            if (success)
            {
                restored++;
            }
            else
            {
                failures.Add($"{entry.Key}: {error}");
            }
        }

        return new RegistryRestoreOutcome(restored, skipped, failures);
    }

    private static async Task<(bool Success, string? Error)> ImportAsync(
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

            startInfo.ArgumentList.Add("import");
            startInfo.ArgumentList.Add(filePath);

            using var process = Process.Start(startInfo);
            if (process is null)
            {
                return (false, "No se pudo iniciar reg.exe.");
            }

            var error = await process.StandardError
                .ReadToEndAsync(cancellationToken)
                .ConfigureAwait(false);

            await process
                .WaitForExitAsync(cancellationToken)
                .ConfigureAwait(false);

            return process.ExitCode == 0
                ? (true, null)
                : (false, string.IsNullOrWhiteSpace(error)
                    ? $"reg.exe terminó con código {process.ExitCode}."
                    : error.Trim());
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    private sealed record RegistryBackupEntry(
        string Key,
        string? FileName,
        string? HashSha256,
        string? Error);
}
