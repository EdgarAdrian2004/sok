using System.Globalization;
using SOK.Application.Abstractions;
using SOK.Application.Common;
using SOK.Application.DTOs;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Application.Services;

/// <summary>
/// Orquesta el módulo de gestión de servicios: consulta el estado del SCM,
/// crea un respaldo antes de cada cambio y registra el resultado en la
/// bitácora. La regla de oro de SOK se aplica en todas las escrituras.
/// </summary>
public sealed class ServiceManager : IServiceManager
{
    private readonly IServiceControlService _serviceControl;
    private readonly IServicesBackupService _backupService;
    private readonly IBackupRepository _backupRepository;
    private readonly ILogService _logService;
    private readonly IAppPathProvider _pathProvider;

    public ServiceManager(
        IServiceControlService serviceControl,
        IServicesBackupService backupService,
        IBackupRepository backupRepository,
        ILogService logService,
        IAppPathProvider pathProvider)
    {
        _serviceControl = serviceControl ?? throw new ArgumentNullException(nameof(serviceControl));
        _backupService = backupService ?? throw new ArgumentNullException(nameof(backupService));
        _backupRepository = backupRepository ?? throw new ArgumentNullException(nameof(backupRepository));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
    }

    public async Task<IReadOnlyList<ServiceEntry>> GetCatalogAsync(CancellationToken cancellationToken = default)
    {
        var definitions = ServiceCatalog.CreateDefinitions();
        var snapshots = await _serviceControl
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);

        var byName = snapshots
            .GroupBy(snapshot => snapshot.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var definition in definitions)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (byName.TryGetValue(definition.Id, out var snapshot))
            {
                definition.Exists = true;
                definition.StartMode = snapshot.StartMode;
                definition.State = snapshot.State;
            }
            else
            {
                definition.Exists = false;
                definition.StartMode = ServiceStartMode.Desconocido;
                definition.State = ServiceState.Desconocido;
            }
        }

        return definitions;
    }

    public async Task<ServiceOperationResult> SetDisabledAsync(
        string serviceName,
        bool disabled,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        var entry = ServiceCatalog.CreateDefinitions()
            .FirstOrDefault(definition => string.Equals(definition.Id, serviceName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"No existe el servicio '{serviceName}' en el catálogo.", nameof(serviceName));

        var snapshot = await _serviceControl
            .GetAsync(entry.Id, cancellationToken)
            .ConfigureAwait(false);

        if (snapshot is null)
        {
            return ServiceOperationResult.Completed(
                0,
                1,
                null,
                new[] { $"{entry.DisplayName}: el servicio no existe en este equipo." },
                $"«{entry.DisplayName}» no está instalado en este equipo.");
        }

        entry.Exists = true;
        entry.StartMode = snapshot.StartMode;
        entry.State = snapshot.State;

        var desired = disabled ? ServiceStatus.Deshabilitado : ServiceStatus.Habilitado;
        if (entry.Status == desired)
        {
            return ServiceOperationResult.NoChange(
                disabled
                    ? $"«{entry.DisplayName}» ya estaba deshabilitado."
                    : $"«{entry.DisplayName}» ya estaba habilitado.");
        }

        var backup = await CreateBackupAsync(
            new[] { entry.Id },
            $"Servicios: {entry.DisplayName}",
            cancellationToken).ConfigureAwait(false);

        try
        {
            var stopped = true;
            if (disabled)
            {
                stopped = await _serviceControl
                    .TryStopAsync(entry.Id, cancellationToken)
                    .ConfigureAwait(false);

                await _serviceControl
                    .SetStartModeAsync(entry.Id, ServiceStartMode.Deshabilitado, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await _serviceControl
                    .SetStartModeAsync(entry.Id, entry.DefaultStartMode, cancellationToken)
                    .ConfigureAwait(false);
            }

            var after = await _serviceControl
                .GetAsync(entry.Id, cancellationToken)
                .ConfigureAwait(false);

            var applied = after is not null &&
                          (disabled
                              ? after.StartMode == ServiceStartMode.Deshabilitado
                              : after.StartMode == entry.DefaultStartMode);

            var action = disabled ? "Deshabilitar servicio" : "Habilitar servicio";
            var detail = applied
                ? BuildDetail(entry, after!, stopped)
                : $"Estado resultante: {after?.StartMode.ToString() ?? "desconocido"}.";

            await _logService.LogAsync(
                LogModule.Servicios,
                action,
                entry.Id,
                applied ? ActionResult.Exitoso : ActionResult.Fallido,
                detail,
                backup.Id,
                cancellationToken).ConfigureAwait(false);

            if (!applied)
            {
                return ServiceOperationResult.Completed(
                    0,
                    1,
                    backup.FilePath,
                    new[] { $"{entry.DisplayName}: el tipo de inicio no se aplicó." },
                    $"No se pudo verificar el cambio en «{entry.DisplayName}».");
            }

            var message = disabled
                ? $"«{entry.DisplayName}» se deshabilitó correctamente."
                : $"«{entry.DisplayName}» se habilitó correctamente.";

            if (!stopped)
            {
                message += " El servicio seguía en ejecución; se detendrá al reiniciar Windows.";
            }

            return ServiceOperationResult.Completed(
                1,
                0,
                backup.FilePath,
                Array.Empty<string>(),
                message);
        }
        catch (Exception ex)
        {
            await _logService.LogAsync(
                LogModule.Servicios,
                disabled ? "Deshabilitar servicio" : "Habilitar servicio",
                entry.Id,
                ActionResult.Fallido,
                ex.Message,
                backup.Id,
                CancellationToken.None).ConfigureAwait(false);

            return ServiceOperationResult.Completed(
                0,
                1,
                backup.FilePath,
                new[] { $"{entry.DisplayName}: {ex.Message}" },
                $"No se pudo cambiar «{entry.DisplayName}»: {ex.Message}");
        }
    }

    public async Task<ServiceOperationResult> ApplyProfileAsync(
        ServiceProfile profile,
        CancellationToken cancellationToken = default)
    {
        var catalog = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);

        var pending = catalog
            .Where(entry => entry.Exists &&
                            entry.IsDisabledByProfile(profile) &&
                            entry.Status != ServiceStatus.Deshabilitado)
            .ToList();

        if (pending.Count == 0)
        {
            return ServiceOperationResult.NoChange(
                $"El perfil {ProfileName(profile)} ya está aplicado en este equipo.");
        }

        var backup = await CreateBackupAsync(
            pending.Select(entry => entry.Id).ToList(),
            $"Perfil de servicios: {ProfileName(profile)}",
            cancellationToken).ConfigureAwait(false);

        var applied = 0;
        var failures = new List<string>();

        foreach (var entry in pending)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await _serviceControl
                    .TryStopAsync(entry.Id, cancellationToken)
                    .ConfigureAwait(false);

                await _serviceControl
                    .SetStartModeAsync(entry.Id, ServiceStartMode.Deshabilitado, cancellationToken)
                    .ConfigureAwait(false);

                var after = await _serviceControl
                    .GetAsync(entry.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (after?.StartMode == ServiceStartMode.Deshabilitado)
                {
                    applied++;
                }
                else
                {
                    failures.Add($"{entry.DisplayName}: tipo de inicio {after?.StartMode.ToString() ?? "desconocido"}.");
                }
            }
            catch (Exception ex)
            {
                failures.Add($"{entry.DisplayName}: {ex.Message}");
            }
        }

        var success = failures.Count == 0;
        var detail = $"{applied} de {pending.Count} servicios deshabilitados.";
        if (!success)
        {
            detail += " Fallos: " + string.Join("; ", failures);
        }

        await _logService.LogAsync(
            LogModule.Servicios,
            $"Aplicar perfil {ProfileName(profile)}",
            null,
            success ? ActionResult.Exitoso : ActionResult.Fallido,
            detail,
            backup.Id,
            cancellationToken).ConfigureAwait(false);

        return ServiceOperationResult.Completed(
            applied,
            failures.Count,
            backup.FilePath,
            failures,
            success
                ? $"Perfil {ProfileName(profile)} aplicado: {applied} servicios deshabilitados."
                : $"Perfil {ProfileName(profile)} aplicado parcialmente: {applied} correctos y {failures.Count} con error.");
    }

    private static string ProfileName(ServiceProfile profile) =>
        profile == ServiceProfile.Gamer ? "Gamer" : "Ofimático";

    private static string BuildDetail(ServiceEntry entry, ServiceSnapshot after, bool stopped)
    {
        var state = after.State == ServiceState.EnEjecucion ? "en ejecución" : "detenido";
        var stopNote = stopped ? string.Empty : " (no se pudo detener)";
        return $"{entry.DisplayName}: tipo de inicio {after.StartMode}; estado {state}{stopNote}.";
    }

    private async Task<Backup> CreateBackupAsync(
        IReadOnlyCollection<string> serviceNames,
        string description,
        CancellationToken cancellationToken)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff", CultureInfo.InvariantCulture);
        var backupDirectory = Path.Combine(_pathProvider.BackupDirectory, "servicios_" + timestamp);
        Directory.CreateDirectory(backupDirectory);

        var filePath = await _backupService
            .CreateBackupForServicesAsync(serviceNames, backupDirectory, cancellationToken)
            .ConfigureAwait(false);

        var hash = await Sha256Helper
            .ComputeFileHashAsync(filePath, cancellationToken)
            .ConfigureAwait(false);

        var backup = new Backup
        {
            Name = $"Respaldo de servicios {timestamp}",
            BackupType = BackupType.Servicios,
            FilePath = filePath,
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
