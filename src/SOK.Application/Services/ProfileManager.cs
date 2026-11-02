using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using SOK.Application.Abstractions;
using SOK.Application.Common;
using SOK.Application.DTOs;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Application.Services;

/// <summary>
/// Orquesta el módulo de perfiles: completa los perfiles base a partir de
/// los catálogos, activa el perfil de la sesión, aplica sus ajustes con la
/// regla de oro de SOK y los exporta o importa en JSON.
/// </summary>
public sealed class ProfileManager : IProfileManager
{
    private const string ActiveProfileKey = "ActiveProfileId";

    private static readonly JsonSerializerOptions ExportOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private static readonly JsonSerializerOptions ImportOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IProfileRepository _profileRepository;
    private readonly IProfileSettingRepository _profileSettingRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly ITelemetryManager _telemetryManager;
    private readonly IServiceManager _serviceManager;
    private readonly IMemoryManager _memoryManager;
    private readonly ILogService _logService;

    public ProfileManager(
        IProfileRepository profileRepository,
        IProfileSettingRepository profileSettingRepository,
        ISettingsRepository settingsRepository,
        ITelemetryManager telemetryManager,
        IServiceManager serviceManager,
        IMemoryManager memoryManager,
        ILogService logService)
    {
        _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
        _profileSettingRepository = profileSettingRepository ?? throw new ArgumentNullException(nameof(profileSettingRepository));
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
        _telemetryManager = telemetryManager ?? throw new ArgumentNullException(nameof(telemetryManager));
        _serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
        _memoryManager = memoryManager ?? throw new ArgumentNullException(nameof(memoryManager));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
    }

    public async Task<IReadOnlyList<Profile>> GetAllProfilesAsync(
        CancellationToken cancellationToken = default)
    {
        var profiles = await _profileRepository
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var profile in profiles ?? Array.Empty<Profile>())
        {
            await EnsureDefaultSettingsAsync(profile, cancellationToken).ConfigureAwait(false);
        }

        return profiles ?? Array.Empty<Profile>();
    }

    public async Task<Profile?> GetActiveProfileAsync(CancellationToken cancellationToken = default)
    {
        var setting = await _settingsRepository
            .GetAsync(ActiveProfileKey, cancellationToken)
            .ConfigureAwait(false);

        if (setting is not null &&
            int.TryParse(setting.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var activeId))
        {
            var profile = await _profileRepository
                .GetByIdAsync(activeId, cancellationToken)
                .ConfigureAwait(false);

            if (profile is not null)
            {
                await EnsureDefaultSettingsAsync(profile, cancellationToken).ConfigureAwait(false);
                return profile;
            }
        }

        var profiles = await GetAllProfilesAsync(cancellationToken).ConfigureAwait(false);
        var fallback = profiles.FirstOrDefault(profile => profile.IsDefault)
                       ?? profiles.FirstOrDefault();

        if (fallback is not null)
        {
            await _settingsRepository
                .SetAsync(
                    ActiveProfileKey,
                    fallback.Id.ToString(CultureInfo.InvariantCulture),
                    cancellationToken)
                .ConfigureAwait(false);
        }

        return fallback;
    }

    public async Task SetActiveProfileAsync(int profileId, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository
            .GetByIdAsync(profileId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new ArgumentException($"No existe el perfil con identificador {profileId}.", nameof(profileId));

        var current = await GetActiveProfileAsync(cancellationToken).ConfigureAwait(false);
        if (current?.Id == profile.Id)
        {
            return;
        }

        await _settingsRepository
            .SetAsync(
                ActiveProfileKey,
                profile.Id.ToString(CultureInfo.InvariantCulture),
                cancellationToken)
            .ConfigureAwait(false);

        await _logService.LogAsync(
            LogModule.Perfiles,
            "Activar perfil",
            profile.Name,
            ActionResult.Exitoso,
            $"Perfil activo: «{profile.Name}» ({profile.Type}).",
            null,
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<ProfileOperationResult> ApplyActiveProfileAsync(
        CancellationToken cancellationToken = default)
    {
        var profile = await GetActiveProfileAsync(cancellationToken).ConfigureAwait(false);
        if (profile is null)
        {
            return ProfileOperationResult.Failed("No hay un perfil activo para aplicar.");
        }

        return await ApplyProfileAsync(profile.Id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ProfileOperationResult> ApplyProfileAsync(
        int profileId,
        CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository
            .GetByIdAsync(profileId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new ArgumentException($"No existe el perfil con identificador {profileId}.", nameof(profileId));

        await EnsureDefaultSettingsAsync(profile, cancellationToken).ConfigureAwait(false);

        var settings = await _profileSettingRepository
            .GetByProfileIdAsync(profile.Id, cancellationToken)
            .ConfigureAwait(false);

        if (settings is null || settings.Count == 0)
        {
            return ProfileOperationResult.Failed(
                $"El perfil «{profile.Name}» no tiene ajustes definidos.");
        }

        var telemetrySettings = settings
            .Where(setting => setting.Module == ProfileModule.Telemetria)
            .ToList();
        var serviceSettings = settings
            .Where(setting => setting.Module == ProfileModule.Servicios)
            .ToList();
        var memorySettings = settings
            .Where(setting => setting.Module == ProfileModule.Memoria)
            .ToList();

        var failures = new List<string>();
        var backupPaths = new List<string>();
        var telemetryApplied = 0;
        var telemetryFailed = 0;
        var servicesApplied = 0;
        var servicesFailed = 0;
        var memoryApplied = false;

        if (telemetrySettings.Count > 0)
        {
            var disable = telemetrySettings
                .Where(setting => setting.Action == ProfileAction.Deshabilitar)
                .Select(setting => setting.TargetKey)
                .ToList();
            var enable = telemetrySettings
                .Where(setting => setting.Action == ProfileAction.Habilitar)
                .Select(setting => setting.TargetKey)
                .ToList();

            if (disable.Count > 0)
            {
                var result = await _telemetryManager
                    .ApplyDisabledAsync(disable, disabled: true, cancellationToken)
                    .ConfigureAwait(false);

                telemetryApplied += result.AppliedCount;
                telemetryFailed += result.FailedCount;
                failures.AddRange(result.Failures);
                AddBackupPath(backupPaths, result.BackupFilePath);
            }

            if (enable.Count > 0)
            {
                var result = await _telemetryManager
                    .ApplyDisabledAsync(enable, disabled: false, cancellationToken)
                    .ConfigureAwait(false);

                telemetryApplied += result.AppliedCount;
                telemetryFailed += result.FailedCount;
                failures.AddRange(result.Failures);
                AddBackupPath(backupPaths, result.BackupFilePath);
            }
        }

        if (serviceSettings.Count > 0)
        {
            var disable = serviceSettings
                .Where(setting => setting.Action == ProfileAction.Deshabilitar)
                .Select(setting => setting.TargetKey)
                .ToList();
            var enable = serviceSettings
                .Where(setting => setting.Action == ProfileAction.Habilitar)
                .Select(setting => setting.TargetKey)
                .ToList();

            if (disable.Count > 0)
            {
                var result = await _serviceManager
                    .ApplyDisabledAsync(disable, disabled: true, cancellationToken)
                    .ConfigureAwait(false);

                servicesApplied += result.AppliedCount;
                servicesFailed += result.FailedCount;
                failures.AddRange(result.Failures);
                AddBackupPath(backupPaths, result.BackupFilePath);
            }

            if (enable.Count > 0)
            {
                var result = await _serviceManager
                    .ApplyDisabledAsync(enable, disabled: false, cancellationToken)
                    .ConfigureAwait(false);

                servicesApplied += result.AppliedCount;
                servicesFailed += result.FailedCount;
                failures.AddRange(result.Failures);
                AddBackupPath(backupPaths, result.BackupFilePath);
            }
        }

        if (memorySettings.Count > 0)
        {
            try
            {
                var options = await _memoryManager
                    .GetAutoReleaseOptionsAsync(cancellationToken)
                    .ConfigureAwait(false);

                foreach (var setting in memorySettings)
                {
                    options = ApplyMemorySetting(options, setting, failures);
                }

                await _memoryManager
                    .SaveAutoReleaseOptionsAsync(options, cancellationToken)
                    .ConfigureAwait(false);

                memoryApplied = true;
            }
            catch (Exception ex)
            {
                failures.Add($"Memoria: {ex.Message}");
            }
        }

        var success = failures.Count == 0;
        var detail = $"Telemetría: {telemetryApplied} aplicadas y {telemetryFailed} con error; " +
                     $"servicios: {servicesApplied} aplicados y {servicesFailed} con error; " +
                     $"memoria: {(memoryApplied ? "configurada" : "sin cambios")}.";
        if (!success)
        {
            detail += " Fallos: " + string.Join("; ", failures);
        }

        await _logService.LogAsync(
            LogModule.Perfiles,
            $"Aplicar perfil {profile.Name}",
            profile.Name,
            success ? ActionResult.Exitoso : ActionResult.Fallido,
            detail,
            null,
            cancellationToken).ConfigureAwait(false);

        return ProfileOperationResult.Completed(
            telemetryApplied,
            telemetryFailed,
            servicesApplied,
            servicesFailed,
            memoryApplied,
            failures,
            backupPaths,
            success
                ? $"Perfil «{profile.Name}» aplicado correctamente."
                : $"Perfil «{profile.Name}» aplicado parcialmente: {failures.Count} con error.");
    }

    public async Task<string> ExportProfileAsync(
        int profileId,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var profile = await _profileRepository
            .GetByIdAsync(profileId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new ArgumentException($"No existe el perfil con identificador {profileId}.", nameof(profileId));

        await EnsureDefaultSettingsAsync(profile, cancellationToken).ConfigureAwait(false);

        var settings = await _profileSettingRepository
            .GetByProfileIdAsync(profile.Id, cancellationToken)
            .ConfigureAwait(false);

        var dto = new ProfileExportDto(
            ProfileExportDto.CurrentSchemaVersion,
            profile.Name,
            profile.Type.ToString(),
            profile.Description,
            settings
                .Select(setting => new ProfileSettingExportDto(
                    setting.Module.ToString(),
                    setting.TargetKey,
                    setting.Action.ToString(),
                    setting.Value))
                .ToList());

        var json = JsonSerializer.Serialize(dto, ExportOptions);
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(filePath, json, cancellationToken).ConfigureAwait(false);

        await _logService.LogAsync(
            LogModule.Perfiles,
            "Exportar perfil",
            profile.Name,
            ActionResult.Exitoso,
            $"Archivo: {filePath}",
            null,
            cancellationToken).ConfigureAwait(false);

        return filePath;
    }

    public async Task<Profile> ImportProfileAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("No se encontró el archivo de perfil.", filePath);
        }

        var json = await File
            .ReadAllTextAsync(filePath, cancellationToken)
            .ConfigureAwait(false);

        ProfileExportDto? dto;
        try
        {
            dto = JsonSerializer.Deserialize<ProfileExportDto>(json, ImportOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"El archivo no es un perfil JSON válido: {ex.Message}", ex);
        }

        if (dto is null)
        {
            throw new InvalidDataException("El archivo de perfil está vacío.");
        }

        var settings = ValidateProfile(dto);
        var name = await BuildUniqueNameAsync(dto.Name, cancellationToken).ConfigureAwait(false);

        var profile = new Profile
        {
            Name = name,
            Type = ProfileType.Personalizado,
            Description = string.IsNullOrWhiteSpace(dto.Description)
                ? "Perfil incorporado por importación JSON."
                : dto.Description.Trim(),
            IsDefault = false,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        profile.Id = await _profileRepository
            .AddAsync(profile, cancellationToken)
            .ConfigureAwait(false);

        foreach (var setting in settings)
        {
            setting.ProfileId = profile.Id;
            await _profileSettingRepository
                .AddAsync(setting, cancellationToken)
                .ConfigureAwait(false);
        }

        await _logService.LogAsync(
            LogModule.Perfiles,
            "Importar perfil",
            profile.Name,
            ActionResult.Exitoso,
            $"{settings.Count} ajustes importados desde {Path.GetFileName(filePath)}.",
            null,
            cancellationToken).ConfigureAwait(false);

        return profile;
    }

    private async Task EnsureDefaultSettingsAsync(
        Profile profile,
        CancellationToken cancellationToken)
    {
        if (profile.Type == ProfileType.Personalizado)
        {
            return;
        }

        var existing = await _profileSettingRepository
            .GetByProfileIdAsync(profile.Id, cancellationToken)
            .ConfigureAwait(false);

        if (existing is { Count: > 0 })
        {
            return;
        }

        foreach (var setting in ProfileCatalog.CreateDefaultSettings(profile.Type))
        {
            setting.ProfileId = profile.Id;
            await _profileSettingRepository
                .AddAsync(setting, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private static MemoryAutoReleaseOptions ApplyMemorySetting(
        MemoryAutoReleaseOptions options,
        ProfileSetting setting,
        ICollection<string> failures)
    {
        if (string.Equals(setting.TargetKey, ProfileCatalog.AutoReleaseEnabledKey, StringComparison.OrdinalIgnoreCase))
        {
            if (bool.TryParse(setting.Value, out var enabled))
            {
                return options with { Enabled = enabled };
            }

            failures.Add($"Memoria: valor inválido para {setting.TargetKey}.");
            return options;
        }

        if (string.Equals(setting.TargetKey, ProfileCatalog.AutoReleaseThresholdKey, StringComparison.OrdinalIgnoreCase))
        {
            if (int.TryParse(setting.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var threshold))
            {
                return (options with { ThresholdPercent = threshold }).Normalized();
            }

            failures.Add($"Memoria: umbral inválido «{setting.Value}».");
            return options;
        }

        failures.Add($"Memoria: ajuste desconocido «{setting.TargetKey}».");
        return options;
    }

    private static IReadOnlyList<ProfileSetting> ValidateProfile(ProfileExportDto dto)
    {
        if (dto.SchemaVersion != ProfileExportDto.CurrentSchemaVersion)
        {
            throw new InvalidDataException(
                $"Versión de esquema no soportada: {dto.SchemaVersion}. " +
                $"Se esperaba {ProfileExportDto.CurrentSchemaVersion}.");
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new InvalidDataException("El perfil no tiene nombre.");
        }

        if (dto.Settings is null || dto.Settings.Count == 0)
        {
            throw new InvalidDataException("El perfil no tiene ajustes.");
        }

        var telemetryIds = TelemetryCatalog.CreateDefinitions()
            .Select(key => key.Id)
            .ToHashSet(StringComparer.Ordinal);
        var serviceNames = ServiceCatalog.CreateDefinitions()
            .Select(entry => entry.Id)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var result = new List<ProfileSetting>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in dto.Settings)
        {
            if (!Enum.TryParse<ProfileModule>(item.Module, ignoreCase: true, out var module))
            {
                throw new InvalidDataException($"Módulo desconocido: «{item.Module}».");
            }

            if (!Enum.TryParse<ProfileAction>(item.Action, ignoreCase: true, out var action))
            {
                throw new InvalidDataException($"Acción desconocida: «{item.Action}».");
            }

            if (string.IsNullOrWhiteSpace(item.TargetKey))
            {
                throw new InvalidDataException("Un ajuste no tiene TargetKey.");
            }

            var targetKey = item.TargetKey.Trim();

            switch (module)
            {
                case ProfileModule.Telemetria when !telemetryIds.Contains(targetKey):
                    throw new InvalidDataException($"Clave de telemetría desconocida: «{targetKey}».");

                case ProfileModule.Servicios when !serviceNames.Contains(targetKey):
                    throw new InvalidDataException($"Servicio desconocido: «{targetKey}».");

                case ProfileModule.Memoria when
                    !string.Equals(targetKey, ProfileCatalog.AutoReleaseEnabledKey, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(targetKey, ProfileCatalog.AutoReleaseThresholdKey, StringComparison.OrdinalIgnoreCase):
                    throw new InvalidDataException($"Ajuste de memoria desconocido: «{targetKey}».");
            }

            if ((action is ProfileAction.Umbral or ProfileAction.Valor) &&
                string.IsNullOrWhiteSpace(item.Value))
            {
                throw new InvalidDataException(
                    $"El ajuste «{targetKey}» requiere un valor para la acción {action}.");
            }

            var dedupeKey = $"{module}:{targetKey}";
            if (!seen.Add(dedupeKey))
            {
                continue;
            }

            result.Add(new ProfileSetting
            {
                Module = module,
                TargetKey = targetKey,
                Action = action,
                Value = string.IsNullOrWhiteSpace(item.Value) ? null : item.Value.Trim()
            });
        }

        if (result.Count == 0)
        {
            throw new InvalidDataException("El perfil no contiene ajustes válidos.");
        }

        return result;
    }

    private async Task<string> BuildUniqueNameAsync(
        string name,
        CancellationToken cancellationToken)
    {
        var candidate = name.Trim();
        var existing = await _profileRepository
            .GetByNameAsync(candidate, cancellationToken)
            .ConfigureAwait(false);

        if (existing is null)
        {
            return candidate;
        }

        var suffix = 1;
        while (true)
        {
            candidate = $"{name.Trim()} (importado{(suffix == 1 ? string.Empty : $" {suffix}")})";
            existing = await _profileRepository
                .GetByNameAsync(candidate, cancellationToken)
                .ConfigureAwait(false);

            if (existing is null)
            {
                return candidate;
            }

            suffix++;
        }
    }

    private static void AddBackupPath(ICollection<string> paths, string? path)
    {
        if (!string.IsNullOrWhiteSpace(path) && !paths.Contains(path, StringComparer.OrdinalIgnoreCase))
        {
            paths.Add(path);
        }
    }
}
