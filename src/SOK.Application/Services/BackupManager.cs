using System.Text.Json;
using SOK.Application.Abstractions;
using SOK.Application.Common;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Application.Services;

/// <summary>
/// Crea un respaldo completo: exporta el registro, exporta los servicios,
/// genera un manifiesto, calcula su SHA-256 y registra los metadatos.
/// </summary>
public sealed class BackupManager : IBackupManager
{
    private readonly IRegistryBackupService _registryBackupService;
    private readonly IServicesBackupService _servicesBackupService;
    private readonly IBackupRepository _backupRepository;
    private readonly ILogService _logService;
    private readonly IAppPathProvider _pathProvider;

    public BackupManager(
        IRegistryBackupService registryBackupService,
        IServicesBackupService servicesBackupService,
        IBackupRepository backupRepository,
        ILogService logService,
        IAppPathProvider pathProvider)
    {
        _registryBackupService = registryBackupService ?? throw new ArgumentNullException(nameof(registryBackupService));
        _servicesBackupService = servicesBackupService ?? throw new ArgumentNullException(nameof(servicesBackupService));
        _backupRepository = backupRepository ?? throw new ArgumentNullException(nameof(backupRepository));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
    }

    public async Task<Backup> CreateFullBackupAsync(string? description = null, CancellationToken cancellationToken = default)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var backupDirectory = Path.Combine(_pathProvider.BackupDirectory, timestamp);
        Directory.CreateDirectory(backupDirectory);

        try
        {
            var registryManifestPath = await _registryBackupService
                .CreateBackupAsync(backupDirectory, cancellationToken)
                .ConfigureAwait(false);

            var servicesFilePath = await _servicesBackupService
                .CreateBackupAsync(backupDirectory, cancellationToken)
                .ConfigureAwait(false);

            var manifestPath = Path.Combine(backupDirectory, "backup-manifest.json");
            var manifest = new
            {
                CreatedAt = DateTime.Now,
                Description = description,
                Registry = registryManifestPath,
                Services = servicesFilePath
            };

            var manifestJson = JsonSerializer.Serialize(manifest, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(manifestPath, manifestJson, cancellationToken).ConfigureAwait(false);

            var hash = await Sha256Helper
                .ComputeFileHashAsync(manifestPath, cancellationToken)
                .ConfigureAwait(false);

            var backup = new Backup
            {
                Name = $"Respaldo completo {timestamp}",
                BackupType = BackupType.Completo,
                FilePath = manifestPath,
                HashSha256 = hash,
                Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
                CreatedAt = DateTime.Now
            };

            backup.Id = await _backupRepository
                .AddAsync(backup, cancellationToken)
                .ConfigureAwait(false);

            await _logService.LogAsync(
                LogModule.Respaldos,
                "Crear respaldo completo",
                manifestPath,
                ActionResult.Exitoso,
                backup.Description,
                backup.Id,
                cancellationToken).ConfigureAwait(false);

            return backup;
        }
        catch (Exception ex)
        {
            await _logService.LogAsync(
                LogModule.Respaldos,
                "Crear respaldo completo",
                backupDirectory,
                ActionResult.Fallido,
                ex.Message,
                null,
                CancellationToken.None).ConfigureAwait(false);

            throw;
        }
    }
}
