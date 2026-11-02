using System.Text.Json;
using SOK.Application.Abstractions;
using SOK.Application.Common;
using SOK.Application.DTOs;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Application.Services;

/// <summary>
/// Orquesta la restauración de respaldos: verifica la integridad SHA-256
/// del manifiesto, importa las claves de registro y devuelve los servicios
/// a su tipo de inicio anterior. Cada restauración queda en la bitácora con
/// el resultado <see cref="ActionResult.Revertido"/>.
/// </summary>
public sealed class RestoreManager : IRestoreManager
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IBackupRepository _backupRepository;
    private readonly IRegistryRestoreService _registryRestoreService;
    private readonly IServiceControlService _serviceControl;
    private readonly ILogService _logService;

    public RestoreManager(
        IBackupRepository backupRepository,
        IRegistryRestoreService registryRestoreService,
        IServiceControlService serviceControl,
        ILogService logService)
    {
        _backupRepository = backupRepository ?? throw new ArgumentNullException(nameof(backupRepository));
        _registryRestoreService = registryRestoreService ?? throw new ArgumentNullException(nameof(registryRestoreService));
        _serviceControl = serviceControl ?? throw new ArgumentNullException(nameof(serviceControl));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
    }

    public async Task<IReadOnlyList<BackupStatus>> GetBackupsAsync(
        CancellationToken cancellationToken = default)
    {
        var backups = await _backupRepository
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);

        var statuses = new List<BackupStatus>();

        foreach (var backup in backups ?? Array.Empty<Backup>())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var exists = File.Exists(backup.FilePath);
            var matches = false;

            if (exists)
            {
                try
                {
                    var hash = await Sha256Helper
                        .ComputeFileHashAsync(backup.FilePath, cancellationToken)
                        .ConfigureAwait(false);

                    matches = string.Equals(hash, backup.HashSha256, StringComparison.OrdinalIgnoreCase);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch
                {
                    matches = false;
                }
            }

            statuses.Add(new BackupStatus(
                backup.Id,
                backup.Name,
                backup.BackupType.ToString(),
                backup.FilePath,
                backup.Description ?? string.Empty,
                backup.CreatedAt,
                exists,
                matches));
        }

        return statuses;
    }

    public async Task<RestoreResult> RestoreAsync(
        int backupId,
        CancellationToken cancellationToken = default)
    {
        var backup = await _backupRepository
            .GetByIdAsync(backupId, cancellationToken)
            .ConfigureAwait(false);

        if (backup is null)
        {
            return await FailAsync(
                null,
                $"No existe el respaldo con identificador {backupId}.",
                cancellationToken).ConfigureAwait(false);
        }

        if (!File.Exists(backup.FilePath))
        {
            return await FailAsync(
                backup,
                $"No se encontró el archivo del respaldo: {backup.FilePath}",
                cancellationToken).ConfigureAwait(false);
        }

        var currentHash = await Sha256Helper
            .ComputeFileHashAsync(backup.FilePath, cancellationToken)
            .ConfigureAwait(false);

        if (!string.Equals(currentHash, backup.HashSha256, StringComparison.OrdinalIgnoreCase))
        {
            return await FailAsync(
                backup,
                "La verificación SHA-256 falló: el archivo del respaldo fue modificado.",
                cancellationToken).ConfigureAwait(false);
        }

        BackupManifest? manifest;
        try
        {
            var json = await File
                .ReadAllTextAsync(backup.FilePath, cancellationToken)
                .ConfigureAwait(false);

            manifest = JsonSerializer.Deserialize<BackupManifest>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            return await FailAsync(
                backup,
                $"No se pudo leer el manifiesto del respaldo: {ex.Message}",
                cancellationToken).ConfigureAwait(false);
        }

        if (manifest is null)
        {
            return await FailAsync(
                backup,
                "El manifiesto del respaldo está vacío.",
                cancellationToken).ConfigureAwait(false);
        }

        var failures = new List<string>();
        var registryRestored = 0;
        var servicesRestored = 0;

        if (!string.IsNullOrWhiteSpace(manifest.Registry))
        {
            try
            {
                var outcome = await _registryRestoreService
                    .RestoreFromManifestAsync(manifest.Registry, cancellationToken)
                    .ConfigureAwait(false);

                registryRestored = outcome.RestoredCount;
                failures.AddRange(outcome.Failures);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                failures.Add($"Registro: {ex.Message}");
            }
        }

        if (!string.IsNullOrWhiteSpace(manifest.Services))
        {
            servicesRestored = await RestoreServicesAsync(
                manifest.Services,
                failures,
                cancellationToken).ConfigureAwait(false);
        }

        var success = failures.Count == 0;
        var detail = $"{registryRestored} claves de registro y {servicesRestored} servicios restaurados.";
        if (!success)
        {
            detail += " Fallos: " + string.Join("; ", failures);
        }

        await _logService.LogAsync(
            LogModule.Respaldos,
            "Restaurar sistema",
            backup.Name,
            success ? ActionResult.Revertido : ActionResult.Fallido,
            detail,
            backup.Id,
            cancellationToken).ConfigureAwait(false);

        return new RestoreResult(
            success,
            registryRestored,
            servicesRestored,
            failures,
            success
                ? $"Respaldo «{backup.Name}» restaurado correctamente."
                : $"Restauración parcial de «{backup.Name}»: {failures.Count} con error.");
    }

    private async Task<int> RestoreServicesAsync(
        string servicesFilePath,
        ICollection<string> failures,
        CancellationToken cancellationToken)
    {
        List<ServiceBackupEntry>? entries;

        try
        {
            var json = await File
                .ReadAllTextAsync(servicesFilePath, cancellationToken)
                .ConfigureAwait(false);

            entries = JsonSerializer.Deserialize<List<ServiceBackupEntry>>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            failures.Add($"Servicios: no se pudo leer el respaldo ({ex.Message}).");
            return 0;
        }

        if (entries is null)
        {
            failures.Add("Servicios: el respaldo está vacío.");
            return 0;
        }

        var restored = 0;

        foreach (var entry in entries)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(entry.Name))
            {
                continue;
            }

            var mode = MapStartMode(entry.StartMode);
            if (mode == ServiceStartMode.Desconocido)
            {
                failures.Add($"{entry.Name}: tipo de inicio desconocido «{entry.StartMode}».");
                continue;
            }

            try
            {
                await _serviceControl
                    .SetStartModeAsync(entry.Name, mode, cancellationToken)
                    .ConfigureAwait(false);

                var after = await _serviceControl
                    .GetAsync(entry.Name, cancellationToken)
                    .ConfigureAwait(false);

                if (after?.StartMode == mode)
                {
                    restored++;
                }
                else
                {
                    failures.Add($"{entry.Name}: quedó en {after?.StartMode.ToString() ?? "desconocido"}.");
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                failures.Add($"{entry.Name}: {ex.Message}");
            }
        }

        return restored;
    }

    private async Task<RestoreResult> FailAsync(
        Backup? backup,
        string message,
        CancellationToken cancellationToken)
    {
        try
        {
            await _logService.LogAsync(
                LogModule.Respaldos,
                "Restaurar sistema",
                backup?.Name,
                ActionResult.Fallido,
                message,
                backup?.Id,
                cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            // Un fallo de la bitácora no debe ocultar el error de restauración.
        }

        return RestoreResult.Failed(message);
    }

    private static ServiceStartMode MapStartMode(string? value) =>
        value?.Trim().ToLowerInvariant() switch
        {
            "auto" or "automatic" => ServiceStartMode.Automatico,
            "manual" => ServiceStartMode.Manual,
            "disabled" => ServiceStartMode.Deshabilitado,
            "boot" or "system" => ServiceStartMode.Automatico,
            _ => ServiceStartMode.Desconocido
        };

    private sealed record BackupManifest(
        DateTime CreatedAt,
        string? Description,
        string? Registry,
        string? Services);

    private sealed record ServiceBackupEntry(
        string Name,
        string DisplayName,
        string StartMode,
        string State);
}
