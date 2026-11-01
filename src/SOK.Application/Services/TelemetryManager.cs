using System.Globalization;
using SOK.Application.Abstractions;
using SOK.Application.Common;
using SOK.Application.DTOs;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Application.Services;

/// <summary>
/// Orquesta el módulo de telemetría y privacidad: consulta el estado del
/// registro, crea un respaldo antes de cada cambio y registra el resultado
/// en la bitácora. La regla de oro de SOK se aplica en todas las escrituras.
/// </summary>
public sealed class TelemetryManager : ITelemetryManager
{
    private readonly ITelemetryRegistryService _registryService;
    private readonly IRegistryBackupService _backupService;
    private readonly IBackupRepository _backupRepository;
    private readonly ILogService _logService;
    private readonly IAppPathProvider _pathProvider;

    public TelemetryManager(
        ITelemetryRegistryService registryService,
        IRegistryBackupService backupService,
        IBackupRepository backupRepository,
        ILogService logService,
        IAppPathProvider pathProvider)
    {
        _registryService = registryService ?? throw new ArgumentNullException(nameof(registryService));
        _backupService = backupService ?? throw new ArgumentNullException(nameof(backupService));
        _backupRepository = backupRepository ?? throw new ArgumentNullException(nameof(backupRepository));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
    }

    public async Task<IReadOnlyList<TelemetryKey>> GetCatalogAsync(CancellationToken cancellationToken = default)
    {
        var definitions = TelemetryCatalog.CreateDefinitions();

        foreach (var definition in definitions)
        {
            cancellationToken.ThrowIfCancellationRequested();
            definition.Status = await GetStatusAsync(definition, cancellationToken).ConfigureAwait(false);
        }

        return definitions;
    }

    public async Task<TelemetryOperationResult> SetDisabledAsync(
        string keyId,
        bool disabled,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyId);

        var item = TelemetryCatalog.CreateDefinitions()
            .FirstOrDefault(definition => string.Equals(definition.Id, keyId, StringComparison.Ordinal))
            ?? throw new ArgumentException($"No existe la clave de telemetría '{keyId}'.", nameof(keyId));

        item.Status = await GetStatusAsync(item, cancellationToken).ConfigureAwait(false);

        var desired = disabled ? TelemetryStatus.Deshabilitado : TelemetryStatus.Habilitado;
        if (item.Status == desired)
        {
            return TelemetryOperationResult.NoChange(
                disabled
                    ? $"«{item.DisplayName}» ya estaba deshabilitada."
                    : $"«{item.DisplayName}» ya estaba habilitada.");
        }

        var backup = await CreateBackupAsync(
            new[] { item.RegistryPath },
            $"Telemetría: {item.DisplayName}",
            cancellationToken).ConfigureAwait(false);

        try
        {
            var value = disabled ? item.DisabledValue : item.EnabledValue;

            await _registryService.WriteValueAsync(
                item.RegistryPath,
                item.ValueName,
                value,
                item.ValueKind,
                cancellationToken).ConfigureAwait(false);

            var resultStatus = await GetStatusAsync(item, cancellationToken).ConfigureAwait(false);
            var success = resultStatus == desired;
            var action = disabled ? "Deshabilitar telemetría" : "Habilitar telemetría";

            await _logService.LogAsync(
                LogModule.Telemetria,
                action,
                item.TargetKey,
                success ? ActionResult.Exitoso : ActionResult.Fallido,
                success ? item.DisplayName : $"Estado resultante: {resultStatus}.",
                backup.Id,
                cancellationToken).ConfigureAwait(false);

            return success
                ? TelemetryOperationResult.Completed(
                    1,
                    0,
                    backup.FilePath,
                    Array.Empty<string>(),
                    disabled
                        ? $"«{item.DisplayName}» se deshabilitó correctamente."
                        : $"«{item.DisplayName}» se habilitó correctamente.")
                : TelemetryOperationResult.Completed(
                    0,
                    1,
                    backup.FilePath,
                    new[] { $"{item.DisplayName}: el valor no se aplicó (estado {resultStatus})." },
                    $"No se pudo verificar el cambio en «{item.DisplayName}».");
        }
        catch (Exception ex)
        {
            await _logService.LogAsync(
                LogModule.Telemetria,
                disabled ? "Deshabilitar telemetría" : "Habilitar telemetría",
                item.TargetKey,
                ActionResult.Fallido,
                ex.Message,
                backup.Id,
                CancellationToken.None).ConfigureAwait(false);

            return TelemetryOperationResult.Completed(
                0,
                1,
                backup.FilePath,
                new[] { $"{item.DisplayName}: {ex.Message}" },
                $"No se pudo cambiar «{item.DisplayName}»: {ex.Message}");
        }
    }

    public async Task<TelemetryOperationResult> ApplyMaxPrivacyAsync(CancellationToken cancellationToken = default)
    {
        var catalog = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        var pending = catalog
            .Where(item => item.Status != TelemetryStatus.Deshabilitado)
            .ToList();

        if (pending.Count == 0)
        {
            return TelemetryOperationResult.NoChange("La privacidad máxima ya está aplicada en este equipo.");
        }

        var registryKeys = pending
            .Select(item => item.RegistryPath)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var backup = await CreateBackupAsync(
            registryKeys,
            "Privacidad máxima",
            cancellationToken).ConfigureAwait(false);

        var applied = 0;
        var failures = new List<string>();

        foreach (var item in pending)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await _registryService.WriteValueAsync(
                    item.RegistryPath,
                    item.ValueName,
                    item.DisabledValue,
                    item.ValueKind,
                    cancellationToken).ConfigureAwait(false);

                var status = await GetStatusAsync(item, cancellationToken).ConfigureAwait(false);
                if (status == TelemetryStatus.Deshabilitado)
                {
                    applied++;
                }
                else
                {
                    failures.Add($"{item.DisplayName}: estado resultante {status}.");
                }
            }
            catch (Exception ex)
            {
                failures.Add($"{item.DisplayName}: {ex.Message}");
            }
        }

        var success = failures.Count == 0;
        var detail = $"{applied} de {pending.Count} claves aplicadas.";
        if (!success)
        {
            detail += " Fallos: " + string.Join("; ", failures);
        }

        await _logService.LogAsync(
            LogModule.Telemetria,
            "Aplicar privacidad máxima",
            null,
            success ? ActionResult.Exitoso : ActionResult.Fallido,
            detail,
            backup.Id,
            cancellationToken).ConfigureAwait(false);

        return TelemetryOperationResult.Completed(
            applied,
            failures.Count,
            backup.FilePath,
            failures,
            success
                ? $"Privacidad máxima aplicada: {applied} claves deshabilitadas."
                : $"Privacidad máxima aplicada parcialmente: {applied} correctas y {failures.Count} con error.");
    }

    private async Task<TelemetryStatus> GetStatusAsync(
        TelemetryKey item,
        CancellationToken cancellationToken)
    {
        var current = await _registryService
            .ReadValueAsync(item.RegistryPath, item.ValueName, cancellationToken)
            .ConfigureAwait(false);

        if (current is null)
        {
            return TelemetryStatus.NoAplicable;
        }

        if (Matches(current, item, item.DisabledValue))
        {
            return TelemetryStatus.Deshabilitado;
        }

        if (Matches(current, item, item.EnabledValue))
        {
            return TelemetryStatus.Habilitado;
        }

        return TelemetryStatus.Desconocido;
    }

    private static bool Matches(TelemetryValue current, TelemetryKey item, string expected)
    {
        if (item.ValueKind == TelemetryValueKind.Dword)
        {
            return int.TryParse(current.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var currentNumber)
                && int.TryParse(expected, NumberStyles.Integer, CultureInfo.InvariantCulture, out var expectedNumber)
                && currentNumber == expectedNumber;
        }

        return string.Equals(current.Value, expected, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<Backup> CreateBackupAsync(
        IReadOnlyCollection<string> registryKeys,
        string description,
        CancellationToken cancellationToken)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff", CultureInfo.InvariantCulture);
        var backupDirectory = Path.Combine(_pathProvider.BackupDirectory, "telemetria_" + timestamp);
        Directory.CreateDirectory(backupDirectory);

        var manifestPath = await _backupService
            .CreateBackupForKeysAsync(registryKeys, backupDirectory, cancellationToken)
            .ConfigureAwait(false);

        var hash = await Sha256Helper
            .ComputeFileHashAsync(manifestPath, cancellationToken)
            .ConfigureAwait(false);

        var backup = new Backup
        {
            Name = $"Respaldo de telemetría {timestamp}",
            BackupType = BackupType.Registro,
            FilePath = manifestPath,
            HashSha256 = hash,
            Description = description,
            CreatedAt = DateTime.Now
        };

        backup.Id = await _backupRepository
            .AddAsync(backup, cancellationToken)
            .ConfigureAwait(false);

        return backup;
    }
}
