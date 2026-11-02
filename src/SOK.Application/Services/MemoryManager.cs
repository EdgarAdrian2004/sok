using System.Globalization;
using SOK.Application.Abstractions;
using SOK.Application.DTOs;
using SOK.Domain.Abstractions;
using SOK.Domain.Enums;

namespace SOK.Application.Services;

/// <summary>
/// Orquesta el módulo de memoria: obtiene las métricas, ejecuta la liberación
/// manual y aplica la liberación automática cuando el uso de RAM alcanza el
/// umbral configurado. Cada liberación queda registrada en la bitácora con el
/// módulo <see cref="LogModule.Memoria"/>.
/// </summary>
public sealed class MemoryManager : IMemoryManager
{
    private const string AutoReleaseEnabledKey = "Memory.AutoReleaseEnabled";
    private const string AutoReleaseThresholdKey = "Memory.AutoReleaseThreshold";
    private const string LegacyThresholdKey = "MemoryReleaseThreshold";

    /// <summary>
    /// Intervalo mínimo entre liberaciones automáticas. Evita que el medidor
    /// dispare una liberación en cada actualización mientras la RAM sigue por
    /// encima del umbral.
    /// </summary>
    internal static readonly TimeSpan MinimumAutoReleaseInterval = TimeSpan.FromMinutes(1);

    private readonly IMemoryProvider _memoryProvider;
    private readonly IMemoryReleaseService _releaseService;
    private readonly ISettingsRepository _settingsRepository;
    private readonly ILogService _logService;
    private readonly TimeProvider _timeProvider;

    private DateTimeOffset? _lastAutoRelease;

    public MemoryManager(
        IMemoryProvider memoryProvider,
        IMemoryReleaseService releaseService,
        ISettingsRepository settingsRepository,
        ILogService logService,
        TimeProvider? timeProvider = null)
    {
        _memoryProvider = memoryProvider ?? throw new ArgumentNullException(nameof(memoryProvider));
        _releaseService = releaseService ?? throw new ArgumentNullException(nameof(releaseService));
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public Task<MemorySnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default) =>
        _memoryProvider.GetSnapshotAsync(cancellationToken);

    public Task<MemoryReleaseResult> ReleaseAsync(CancellationToken cancellationToken = default) =>
        ReleaseCoreAsync(automatic: false, cancellationToken);

    public async Task<MemoryReleaseResult?> TryAutoReleaseAsync(
        MemorySnapshot snapshot,
        MemoryAutoReleaseOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(options);

        var normalized = options.Normalized();
        if (!normalized.IsThresholdMet(snapshot.UsagePercent))
        {
            return null;
        }

        var now = _timeProvider.GetUtcNow();
        if (_lastAutoRelease is { } last &&
            now - last < MinimumAutoReleaseInterval)
        {
            return null;
        }

        _lastAutoRelease = now;
        return await ReleaseCoreAsync(automatic: true, cancellationToken).ConfigureAwait(false);
    }

    public async Task<MemoryAutoReleaseOptions> GetAutoReleaseOptionsAsync(
        CancellationToken cancellationToken = default)
    {
        var enabledSetting = await _settingsRepository
            .GetAsync(AutoReleaseEnabledKey, cancellationToken)
            .ConfigureAwait(false);

        var thresholdSetting = await _settingsRepository
            .GetAsync(AutoReleaseThresholdKey, cancellationToken)
            .ConfigureAwait(false);

        // El DDL inicial siembra «MemoryReleaseThreshold»; se usa como valor de
        // partida mientras el usuario no guarde una preferencia propia.
        thresholdSetting ??= await _settingsRepository
            .GetAsync(LegacyThresholdKey, cancellationToken)
            .ConfigureAwait(false);

        var enabled = ParseBoolean(enabledSetting?.Value);
        var threshold = ParseThreshold(thresholdSetting?.Value);

        return new MemoryAutoReleaseOptions(enabled, threshold).Normalized();
    }

    public async Task SaveAutoReleaseOptionsAsync(
        MemoryAutoReleaseOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        var normalized = options.Normalized();

        await _settingsRepository
            .SetAsync(
                AutoReleaseEnabledKey,
                normalized.Enabled ? "true" : "false",
                cancellationToken)
            .ConfigureAwait(false);

        await _settingsRepository
            .SetAsync(
                AutoReleaseThresholdKey,
                normalized.ThresholdPercent.ToString(CultureInfo.InvariantCulture),
                cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<MemoryReleaseResult> ReleaseCoreAsync(
        bool automatic,
        CancellationToken cancellationToken)
    {
        var action = automatic ? "Liberación automática" : "Liberar memoria";
        MemoryReleaseResult result;

        try
        {
            result = await _releaseService
                .ReleaseAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            result = MemoryReleaseResult.Failed(
                $"No se pudo liberar la memoria: {ex.Message}");
        }

        try
        {
            await _logService
                .LogAsync(
                    LogModule.Memoria,
                    action,
                    null,
                    result.Success ? ActionResult.Exitoso : ActionResult.Fallido,
                    BuildDetail(result),
                    null,
                    cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            // Un fallo de la bitácora no debe ocultar el resultado de la
            // liberación en la interfaz.
        }

        return result;
    }

    private static string BuildDetail(MemoryReleaseResult result)
    {
        if (!result.Success && result.ProcessesTried == 0)
        {
            return result.Message;
        }

        var megabytes = result.FreedBytesEstimate / 1024d / 1024d;
        return $"{result.ProcessesSucceeded} de {result.ProcessesTried} procesos liberados; " +
               $"{result.ProcessesFailed} sin acceso; " +
               $"{megabytes:0.0} MB estimados.";
    }

    private static bool ParseBoolean(string? value)
    {
        if (bool.TryParse(value, out var parsed))
        {
            return parsed;
        }

        return string.Equals(value, "1", StringComparison.Ordinal);
    }

    private static int ParseThreshold(string? value) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : MemoryAutoReleaseOptions.DefaultThresholdPercent;
}
