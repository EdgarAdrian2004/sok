using System.Text.Json;
using Moq;
using SOK.Application.Abstractions;
using SOK.Application.DTOs;
using SOK.Application.Services;
using SOK.Domain.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;
using Xunit;

namespace SOK.UI.Tests.Application;

public sealed class ProfileManagerTests
{
    [Fact]
    public async Task GetActiveProfileAsync_SinConfiguracion_DevuelveElPerfilPorDefecto()
    {
        var profiles = new Mock<IProfileRepository>();
        profiles
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Profile>
            {
                new() { Id = 1, Name = "Ofimatico", Type = ProfileType.Ofimatico, IsDefault = true },
                new() { Id = 2, Name = "Gamer", Type = ProfileType.Gamer }
            });

        var settings = new Mock<ISettingsRepository>();
        settings
            .Setup(repository => repository.GetAsync("ActiveProfileId", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Setting?)null);

        var manager = CreateManager(profiles: profiles, settings: settings);

        var active = await manager.GetActiveProfileAsync();

        Assert.NotNull(active);
        Assert.Equal(1, active!.Id);
        settings.Verify(
            repository => repository.SetAsync(
                "ActiveProfileId",
                "1",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetActiveProfileAsync_PersisteElIdentificador()
    {
        var profiles = new Mock<IProfileRepository>();
        profiles
            .Setup(repository => repository.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Profile { Id = 2, Name = "Gamer", Type = ProfileType.Gamer });

        var settings = new Mock<ISettingsRepository>();
        settings
            .Setup(repository => repository.GetAsync("ActiveProfileId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Setting { Key = "ActiveProfileId", Value = "1" });

        var manager = CreateManager(profiles: profiles, settings: settings);

        await manager.SetActiveProfileAsync(2);

        settings.Verify(
            repository => repository.SetAsync(
                "ActiveProfileId",
                "2",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ApplyProfileAsync_DelegaLosAjustesEnCadaModulo()
    {
        var profile = new Profile
        {
            Id = 7,
            Name = "Mi perfil",
            Type = ProfileType.Personalizado
        };

        var profiles = new Mock<IProfileRepository>();
        profiles
            .Setup(repository => repository.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var profileSettings = new Mock<IProfileSettingRepository>();
        profileSettings
            .Setup(repository => repository.GetByProfileIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProfileSetting>
            {
                new() { Module = ProfileModule.Telemetria, TargetKey = "telemetry-allow", Action = ProfileAction.Deshabilitar },
                new() { Module = ProfileModule.Telemetria, TargetKey = "advertising-id", Action = ProfileAction.Deshabilitar },
                new() { Module = ProfileModule.Servicios, TargetKey = "DiagTrack", Action = ProfileAction.Deshabilitar },
                new() { Module = ProfileModule.Servicios, TargetKey = "Fax", Action = ProfileAction.Deshabilitar },
                new() { Module = ProfileModule.Memoria, TargetKey = "AutoReleaseEnabled", Action = ProfileAction.Valor, Value = "true" },
                new() { Module = ProfileModule.Memoria, TargetKey = "AutoReleaseThreshold", Action = ProfileAction.Umbral, Value = "75" }
            });

        var telemetry = new Mock<ITelemetryManager>();
        telemetry
            .Setup(manager => manager.ApplyDisabledAsync(
                It.IsAny<IReadOnlyCollection<string>>(),
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TelemetryOperationResult(
                true, 2, 0, @"C:\backups\telemetria", Array.Empty<string>(), "Telemetría aplicada."));

        var services = new Mock<IServiceManager>();
        services
            .Setup(manager => manager.ApplyDisabledAsync(
                It.IsAny<IReadOnlyCollection<string>>(),
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceOperationResult(
                true, 2, 0, @"C:\backups\servicios", Array.Empty<string>(), "Servicios aplicados."));

        var memory = new Mock<IMemoryManager>();
        memory
            .Setup(manager => manager.GetAutoReleaseOptionsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(MemoryAutoReleaseOptions.Default);

        MemoryAutoReleaseOptions? saved = null;
        memory
            .Setup(manager => manager.SaveAutoReleaseOptionsAsync(
                It.IsAny<MemoryAutoReleaseOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<MemoryAutoReleaseOptions, CancellationToken>((options, _) => saved = options)
            .Returns(Task.CompletedTask);

        var manager = CreateManager(
            profiles: profiles,
            profileSettings: profileSettings,
            telemetry: telemetry,
            services: services,
            memory: memory);

        var result = await manager.ApplyProfileAsync(7);

        Assert.True(result.Success);
        Assert.Equal(2, result.TelemetryApplied);
        Assert.Equal(2, result.ServicesApplied);
        Assert.True(result.MemoryApplied);
        Assert.Equal(2, result.BackupFilePaths.Count);

        telemetry.Verify(
            service => service.ApplyDisabledAsync(
                It.Is<IReadOnlyCollection<string>>(keys => keys.Count == 2 && keys.Contains("telemetry-allow")),
                true,
                It.IsAny<CancellationToken>()),
            Times.Once);

        services.Verify(
            service => service.ApplyDisabledAsync(
                It.Is<IReadOnlyCollection<string>>(names => names.Count == 2 && names.Contains("DiagTrack")),
                true,
                It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.NotNull(saved);
        Assert.True(saved!.Enabled);
        Assert.Equal(75, saved.ThresholdPercent);
    }

    [Fact]
    public async Task ApplyProfileAsync_SinAjustes_DevuelveFallo()
    {
        var profile = new Profile { Id = 9, Name = "Vacío", Type = ProfileType.Personalizado };

        var profiles = new Mock<IProfileRepository>();
        profiles
            .Setup(repository => repository.GetByIdAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var profileSettings = new Mock<IProfileSettingRepository>();
        profileSettings
            .Setup(repository => repository.GetByProfileIdAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProfileSetting>());

        var manager = CreateManager(profiles: profiles, profileSettings: profileSettings);

        var result = await manager.ApplyProfileAsync(9);

        Assert.False(result.Success);
        Assert.Contains("no tiene ajustes", result.Message);
    }

    [Fact]
    public async Task ExportProfileAsync_EscribeUnJsonValido()
    {
        var profile = new Profile
        {
            Id = 3,
            Name = "Gamer",
            Type = ProfileType.Gamer,
            Description = "Perfil de rendimiento"
        };

        var profiles = new Mock<IProfileRepository>();
        profiles
            .Setup(repository => repository.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var profileSettings = new Mock<IProfileSettingRepository>();
        profileSettings
            .Setup(repository => repository.GetByProfileIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProfileSetting>
            {
                new() { Module = ProfileModule.Telemetria, TargetKey = "telemetry-allow", Action = ProfileAction.Deshabilitar },
                new() { Module = ProfileModule.Memoria, TargetKey = "AutoReleaseThreshold", Action = ProfileAction.Umbral, Value = "75" }
            });

        var manager = CreateManager(profiles: profiles, profileSettings: profileSettings);
        var path = Path.Combine(Path.GetTempPath(), $"sok-perfil-{Guid.NewGuid():N}.json");

        try
        {
            await manager.ExportProfileAsync(3, path);

            Assert.True(File.Exists(path));

            var dto = JsonSerializer.Deserialize<ProfileExportDto>(
                await File.ReadAllTextAsync(path),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(dto);
            Assert.Equal(ProfileExportDto.CurrentSchemaVersion, dto!.SchemaVersion);
            Assert.Equal("Gamer", dto.Name);
            Assert.Equal("Gamer", dto.Type);
            Assert.Equal(2, dto.Settings.Count);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task ImportProfileAsync_CreaUnPerfilPersonalizadoConNombreUnico()
    {
        var profiles = new Mock<IProfileRepository>();
        profiles
            .Setup(repository => repository.GetByNameAsync("Gamer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Profile { Id = 2, Name = "Gamer", Type = ProfileType.Gamer });
        profiles
            .Setup(repository => repository.GetByNameAsync("Gamer (importado)", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Profile?)null);
        profiles
            .Setup(repository => repository.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        var profileSettings = new Mock<IProfileSettingRepository>();
        profileSettings
            .Setup(repository => repository.AddAsync(It.IsAny<ProfileSetting>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var manager = CreateManager(profiles: profiles, profileSettings: profileSettings);
        var path = Path.Combine(Path.GetTempPath(), $"sok-import-{Guid.NewGuid():N}.json");

        try
        {
            await File.WriteAllTextAsync(path, """
                {
                  "schemaVersion": 1,
                  "name": "Gamer",
                  "type": "Gamer",
                  "description": "Perfil compartido",
                  "settings": [
                    { "module": "Telemetria", "targetKey": "telemetry-allow", "action": "Deshabilitar" },
                    { "module": "Servicios", "targetKey": "DiagTrack", "action": "Deshabilitar" }
                  ]
                }
                """);

            var imported = await manager.ImportProfileAsync(path);

            Assert.Equal(10, imported.Id);
            Assert.Equal("Gamer (importado)", imported.Name);
            Assert.Equal(ProfileType.Personalizado, imported.Type);

            profileSettings.Verify(
                repository => repository.AddAsync(
                    It.Is<ProfileSetting>(setting => setting.ProfileId == 10),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task ImportProfileAsync_RechazaUnaVersionDeEsquemaNoSoportada()
    {
        var manager = CreateManager();
        var path = Path.Combine(Path.GetTempPath(), $"sok-import-{Guid.NewGuid():N}.json");

        try
        {
            await File.WriteAllTextAsync(path, """
                {
                  "schemaVersion": 99,
                  "name": "Perfil futuro",
                  "type": "Personalizado",
                  "settings": [
                    { "module": "Memoria", "targetKey": "AutoReleaseThreshold", "action": "Umbral", "value": "80" }
                  ]
                }
                """);

            await Assert.ThrowsAsync<InvalidDataException>(() => manager.ImportProfileAsync(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task ImportProfileAsync_RechazaUnaClaveDesconocida()
    {
        var manager = CreateManager();
        var path = Path.Combine(Path.GetTempPath(), $"sok-import-{Guid.NewGuid():N}.json");

        try
        {
            await File.WriteAllTextAsync(path, """
                {
                  "schemaVersion": 1,
                  "name": "Perfil inválido",
                  "type": "Personalizado",
                  "settings": [
                    { "module": "Telemetria", "targetKey": "clave-inexistente", "action": "Deshabilitar" }
                  ]
                }
                """);

            var error = await Assert.ThrowsAsync<InvalidDataException>(
                () => manager.ImportProfileAsync(path));

            Assert.Contains("desconocida", error.Message);
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static ProfileManager CreateManager(
        Mock<IProfileRepository>? profiles = null,
        Mock<IProfileSettingRepository>? profileSettings = null,
        Mock<ISettingsRepository>? settings = null,
        Mock<ITelemetryManager>? telemetry = null,
        Mock<IServiceManager>? services = null,
        Mock<IMemoryManager>? memory = null,
        Mock<ILogService>? logs = null) =>
        new(
            (profiles ?? new Mock<IProfileRepository>()).Object,
            (profileSettings ?? new Mock<IProfileSettingRepository>()).Object,
            (settings ?? new Mock<ISettingsRepository>()).Object,
            (telemetry ?? new Mock<ITelemetryManager>()).Object,
            (services ?? new Mock<IServiceManager>()).Object,
            (memory ?? new Mock<IMemoryManager>()).Object,
            (logs ?? new Mock<ILogService>()).Object);
}
