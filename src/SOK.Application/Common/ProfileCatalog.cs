using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Application.Common;

/// <summary>
/// Define los ajustes que componen los perfiles base (Gamer y Ofimático) a
/// partir de los catálogos de telemetría y servicios. Los perfiles
/// personalizados se incorporan únicamente por importación JSON.
/// </summary>
public static class ProfileCatalog
{
    /// <summary>Clave del ajuste que activa la liberación automática de memoria.</summary>
    public const string AutoReleaseEnabledKey = "AutoReleaseEnabled";

    /// <summary>Clave del ajuste que fija el umbral de liberación automática.</summary>
    public const string AutoReleaseThresholdKey = "AutoReleaseThreshold";

    /// <summary>
    /// Devuelve los ajustes por defecto de un perfil base. Para el tipo
    /// <see cref="ProfileType.Personalizado"/> devuelve una lista vacía:
    /// esos perfiles solo se crean por importación.
    /// </summary>
    public static IReadOnlyList<ProfileSetting> CreateDefaultSettings(ProfileType type)
    {
        if (type == ProfileType.Personalizado)
        {
            return Array.Empty<ProfileSetting>();
        }

        var settings = new List<ProfileSetting>();

        foreach (var key in TelemetryCatalog.CreateDefinitions())
        {
            settings.Add(new ProfileSetting
            {
                Module = ProfileModule.Telemetria,
                TargetKey = key.Id,
                Action = ProfileAction.Deshabilitar
            });
        }

        var serviceProfile = type == ProfileType.Gamer
            ? ServiceProfile.Gamer
            : ServiceProfile.Ofimatico;

        foreach (var entry in ServiceCatalog
                     .CreateDefinitions()
                     .Where(entry => entry.IsDisabledByProfile(serviceProfile)))
        {
            settings.Add(new ProfileSetting
            {
                Module = ProfileModule.Servicios,
                TargetKey = entry.Id,
                Action = ProfileAction.Deshabilitar
            });
        }

        settings.Add(new ProfileSetting
        {
            Module = ProfileModule.Memoria,
            TargetKey = AutoReleaseEnabledKey,
            Action = ProfileAction.Valor,
            Value = "true"
        });

        settings.Add(new ProfileSetting
        {
            Module = ProfileModule.Memoria,
            TargetKey = AutoReleaseThresholdKey,
            Action = ProfileAction.Umbral,
            Value = type == ProfileType.Gamer ? "75" : "85"
        });

        return settings;
    }

    /// <summary>
    /// Describe el criterio del perfil para mostrarlo en la interfaz y en
    /// la bitácora.
    /// </summary>
    public static string GetCriterion(ProfileType type) =>
        type switch
        {
            ProfileType.Gamer =>
                "Prioriza el rendimiento: deshabilita telemetrías y servicios no esenciales, " +
                "incluida la notificación de impresión, y libera memoria al 75 %.",
            ProfileType.Ofimatico =>
                "Prioriza la estabilidad: deshabilita telemetrías y servicios no esenciales " +
                "sin tocar impresión ni Xbox, y libera memoria al 85 %.",
            _ =>
                "Perfil personalizado incorporado por importación JSON."
        };
}
