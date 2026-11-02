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

        return await ApplyPendingAsync(
            pending,
            disabled: true,
            "Aplicar privacidad máxima",
            "Privacidad máxima",
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<TelemetryOperationResult> ApplyDisabledAsync(
        IReadOnlyCollection<string> keyIds,
        bool disabled,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keyIds);

        var definitions = TelemetryCatalog.CreateDefinitions()
            .ToDictionary(item => item.Id, StringComparer.Ordinal);

        var pending = new List<TelemetryKey>();
        var failures = new List<string>();

        foreach (var keyId in keyIds.Distinct(StringComparer.Ordinal))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!definitions.TryGetValue(keyId, out var item))
            {
                failures.Add($"Clave de telemetría desconocida: «{keyId}».");
                continue;
            }

            item.Status = await GetStatusAsync(item, cancellationToken).ConfigureAwait(false);
            var desired = disabled ? TelemetryStatus.Deshabilitado : TelemetryStatus.Habilitado;
            if (item.Status != desired)
            {
                pending.Add(item);
            }
        }

        if (pending.Count == 0)
        {
            var message = failures.Count == 0
                ? disabled
                    ? "Las telemetrías del perfil ya estaban deshabilitadas."
                    : "Las telemetrías del perfil ya estaban habilitadas."
                : string.Join(" ", failures);

            return failures.Count == 0
                ? TelemetryOperationResult.NoChange(message)
                : TelemetryOperationResult.Completed(0, failures.Count, null, failures, message);
        }

        var result = await ApplyPendingAsync(
            pending,
            disabled,
            disabled ? "Deshabilitar telemetrías del perfil" : "Habilitar telemetrías del perfil",
            "Ajustes de telemetría del perfil",
            cancellationToken).ConfigureAwait(false);

        if (failures.Count == 0)
        {
            return result;
        }

        var merged = result.Failures.Concat(failures).ToList();
        return TelemetryOperationResult.Completed(
            result.AppliedCount,
            result.FailedCount + failures.Count,
            result.BackupFilePath,
            merged,
            result.Message + " " + string.Join(" ", failures));
    }

    private async Task<TelemetryOperationResult> ApplyPendingAsync(
        IReadOnlyCollection<TelemetryKey> pending,
        bool disabled,
        string action,
        string description,
        CancellationToken cancellationToken)
    {
        var registryKeys = pending
            .Select(item => item.RegistryPath)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var backup = await CreateBackupAsync(
            registryKeys,
            description,
            cancellationToken).ConfigureAwait(false);

        var applied = 0;
        var failures = new List<string>();

        foreach (var item in pending)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var value = disabled ? item.DisabledValue : item.EnabledValue;

                await _registryService.WriteValueAsync(
                    item.RegistryPath,
                    item.ValueName,
                    value,
                    item.ValueKind,
                    cancellationToken).ConfigureAwait(false);

                var status = await GetStatusAsync(item, cancellationToken).ConfigureAwait(false);
                var desired = disabled ? TelemetryStatus.Deshabilitado : TelemetryStatus.Habilitado;
                if (status == desired)
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
            action,
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
                ? $"{action}: {applied} claves actualizadas."
                : $"{action}: {applied} correctas y {failures.Count} con error.");
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
