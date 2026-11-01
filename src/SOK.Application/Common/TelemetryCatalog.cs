using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Application.Common;

/// <summary>
/// Catálogo estático de las claves de telemetría y privacidad que SOK
/// conoce. Cada llamada devuelve definiciones nuevas para que el estado
/// observado no se comparta entre consultas.
/// </summary>
public static class TelemetryCatalog
{
    private const string DataCollectionPolicy =
        @"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection";

    private const string DataCollectionMachinePolicy =
        @"HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection";

    private const string SearchKey =
        @"HKCU\Software\Microsoft\Windows\CurrentVersion\Search";

    private const string SearchSettingsKey =
        @"HKCU\Software\Microsoft\Windows\CurrentVersion\SearchSettings";

    private const string DefenderSpynetPolicy =
        @"HKLM\SOFTWARE\Policies\Microsoft\Windows Defender\Spynet";

    private const string AdvertisingInfoUser =
        @"HKCU\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo";

    private const string AdvertisingInfoMachine =
        @"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\AdvertisingInfo";

    private const string ConsentStore =
        @"HKCU\Software\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore";

    private const string EdgePolicy =
        @"HKLM\SOFTWARE\Policies\Microsoft\Edge";

    private const string ErrorReporting =
        @"HKLM\SOFTWARE\Microsoft\Windows\Windows Error Reporting";

    private const string OneDrivePolicy =
        @"HKLM\SOFTWARE\Policies\Microsoft\OneDrive";

    private const string ContentDelivery =
        @"HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager";

    private const string SystemPolicy =
        @"HKLM\SOFTWARE\Policies\Microsoft\Windows\System";

    /// <summary>
    /// Devuelve las definiciones del catálogo con estado inicial desconocido.
    /// </summary>
    public static IReadOnlyList<TelemetryKey> CreateDefinitions() =>
        new List<TelemetryKey>
        {
            // ---------------- Sistema / diagnóstico ----------------
            Dword(
                "telemetry-allow",
                TelemetryCategory.Sistema,
                "Nivel de telemetría de Windows",
                "Reduce el nivel de datos de diagnóstico que Windows envía a Microsoft.",
                DataCollectionPolicy,
                "AllowTelemetry",
                disabled: "0",
                enabled: "1",
                requiresAdmin: true,
                requiresRestart: false),

            Dword(
                "telemetry-allow-policy",
                TelemetryCategory.Sistema,
                "Nivel de telemetría (directiva de equipo)",
                "Aplica la directiva de equipo que fuerza el nivel mínimo de telemetría.",
                DataCollectionMachinePolicy,
                "AllowTelemetry",
                disabled: "0",
                enabled: "1",
                requiresAdmin: true,
                requiresRestart: false),

            Dword(
                "diagtrack-service",
                TelemetryCategory.Sistema,
                "Servicio DiagTrack",
                "Deshabilita el servicio de seguimiento de diagnóstico (Connected User Experiences and Telemetry).",
                @"HKLM\SYSTEM\CurrentControlSet\Services\DiagTrack",
                "Start",
                disabled: "4",
                enabled: "2",
                requiresAdmin: true,
                requiresRestart: true),

            Dword(
                "dmwappush-service",
                TelemetryCategory.Sistema,
                "Servicio dmwappushservice",
                "Deshabilita el servicio de enrutamiento de mensajes WAP push.",
                @"HKLM\SYSTEM\CurrentControlSet\Services\dmwappushservice",
                "Start",
                disabled: "4",
                enabled: "3",
                requiresAdmin: true,
                requiresRestart: true),

            Dword(
                "telemetry-device-name",
                TelemetryCategory.Sistema,
                "Nombre del equipo en la telemetría",
                "Evita que Windows incluya el nombre del equipo en los datos de diagnóstico.",
                DataCollectionPolicy,
                "AllowDeviceNameInTelemetry",
                disabled: "0",
                enabled: "1",
                requiresAdmin: true,
                requiresRestart: false),

            Dword(
                "telemetry-feedback-notifications",
                TelemetryCategory.Sistema,
                "Notificaciones de comentarios",
                "Impide que Windows solicite comentarios de forma periódica.",
                DataCollectionMachinePolicy,
                "DoNotShowFeedbackNotifications",
                disabled: "1",
                enabled: "0",
                requiresAdmin: true,
                requiresRestart: false),

            // ---------------- Cortana y búsqueda ----------------
            Dword(
                "cortana-allow",
                TelemetryCategory.Cortana,
                "Cortana",
                "Desactiva el asistente Cortana para el usuario actual.",
                SearchKey,
                "AllowCortana",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: true),

            Dword(
                "cortana-consent",
                TelemetryCategory.Cortana,
                "Consentimiento de Cortana",
                "Revoca el consentimiento del asistente en este equipo.",
                SearchKey,
                "CortanaConsent",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: true),

            Dword(
                "cortana-policy",
                TelemetryCategory.Cortana,
                "Cortana (directiva de equipo)",
                "Bloquea Cortana mediante directiva para todos los usuarios.",
                @"HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search",
                "AllowCortana",
                disabled: "0",
                enabled: "1",
                requiresAdmin: true,
                requiresRestart: true),

            Dword(
                "search-bing",
                TelemetryCategory.Cortana,
                "Búsqueda web en el menú Inicio",
                "Desactiva los resultados de Bing en la búsqueda de Windows.",
                SearchKey,
                "BingSearchEnabled",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: true),

            Dword(
                "search-location",
                TelemetryCategory.Cortana,
                "Ubicación en las búsquedas",
                "Impide que la búsqueda de Windows utilice la ubicación del equipo.",
                SearchKey,
                "AllowSearchToUseLocation",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: true),

            Dword(
                "search-highlights",
                TelemetryCategory.Cortana,
                "Sugerencias del cuadro de búsqueda",
                "Desactiva las sugerencias dinámicas del cuadro de búsqueda.",
                SearchSettingsKey,
                "IsDynamicSearchBoxEnabled",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: true),

            // ---------------- Defender y SmartScreen ----------------
            Dword(
                "defender-samples",
                TelemetryCategory.Defender,
                "Envío de muestras a Microsoft",
                "Evita que Defender envíe muestras de archivos sospechosos.",
                DefenderSpynetPolicy,
                "SubmitSamplesConsent",
                disabled: "2",
                enabled: "1",
                requiresAdmin: true,
                requiresRestart: false),

            Dword(
                "defender-spynet",
                TelemetryCategory.Defender,
                "Telemetría de Defender",
                "Reduce los datos que Defender envía a Microsoft.",
                DefenderSpynetPolicy,
                "SpynetReporting",
                disabled: "0",
                enabled: "1",
                requiresAdmin: true,
                requiresRestart: false),

            String(
                "smartscreen-system",
                TelemetryCategory.Defender,
                "SmartScreen del sistema",
                "Desactiva la comprobación SmartScreen de aplicaciones.",
                @"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer",
                "SmartScreenEnabled",
                disabled: "Off",
                enabled: "Warn",
                requiresAdmin: true,
                requiresRestart: true),

            Dword(
                "smartscreen-store-apps",
                TelemetryCategory.Defender,
                "SmartScreen para aplicaciones de la Store",
                "Desactiva la evaluación web del contenido de las aplicaciones.",
                @"HKCU\Software\Microsoft\Windows\CurrentVersion\AppHost",
                "EnableWebContentEvaluation",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: false),

            // ---------------- Publicidad ----------------
            Dword(
                "advertising-id-user",
                TelemetryCategory.Publicidad,
                "Identificador de publicidad",
                "Desactiva el identificador de publicidad del usuario actual.",
                AdvertisingInfoUser,
                "Enabled",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: false),

            Dword(
                "advertising-id-machine",
                TelemetryCategory.Publicidad,
                "Identificador de publicidad (equipo)",
                "Desactiva el identificador de publicidad a nivel de equipo.",
                AdvertisingInfoMachine,
                "Enabled",
                disabled: "0",
                enabled: "1",
                requiresAdmin: true,
                requiresRestart: false),

            Dword(
                "tailored-experiences",
                TelemetryCategory.Publicidad,
                "Experiencias personalizadas",
                "Impide que Microsoft use los datos de diagnóstico para personalizar sugerencias.",
                @"HKCU\Software\Microsoft\Windows\CurrentVersion\Privacy",
                "TailoredExperiencesWithDiagnosticDataEnabled",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: false),

            // ---------------- Privacidad (permisos de aplicaciones) ----------------
            String(
                "consent-location",
                TelemetryCategory.Privacidad,
                "Permiso de ubicación",
                "Niega el acceso a la ubicación para las aplicaciones.",
                ConsentStore + @"\location",
                "Value",
                disabled: "Deny",
                enabled: "Allow",
                requiresAdmin: false,
                requiresRestart: false),

            String(
                "consent-webcam",
                TelemetryCategory.Privacidad,
                "Permiso de cámara",
                "Niega el acceso a la cámara para las aplicaciones.",
                ConsentStore + @"\webcam",
                "Value",
                disabled: "Deny",
                enabled: "Allow",
                requiresAdmin: false,
                requiresRestart: false),

            String(
                "consent-microphone",
                TelemetryCategory.Privacidad,
                "Permiso de micrófono",
                "Niega el acceso al micrófono para las aplicaciones.",
                ConsentStore + @"\microphone",
                "Value",
                disabled: "Deny",
                enabled: "Allow",
                requiresAdmin: false,
                requiresRestart: false),

            String(
                "consent-contacts",
                TelemetryCategory.Privacidad,
                "Permiso de contactos",
                "Niega el acceso a los contactos para las aplicaciones.",
                ConsentStore + @"\contacts",
                "Value",
                disabled: "Deny",
                enabled: "Allow",
                requiresAdmin: false,
                requiresRestart: false),

            String(
                "consent-appointments",
                TelemetryCategory.Privacidad,
                "Permiso de calendario",
                "Niega el acceso al calendario para las aplicaciones.",
                ConsentStore + @"\appointments",
                "Value",
                disabled: "Deny",
                enabled: "Allow",
                requiresAdmin: false,
                requiresRestart: false),

            String(
                "consent-user-notifications",
                TelemetryCategory.Privacidad,
                "Permiso de notificaciones de usuario",
                "Niega el acceso al historial de notificaciones para las aplicaciones.",
                ConsentStore + @"\userNotificationListener",
                "Value",
                disabled: "Deny",
                enabled: "Allow",
                requiresAdmin: false,
                requiresRestart: false),

            String(
                "consent-radios",
                TelemetryCategory.Privacidad,
                "Permiso de radios (Bluetooth y Wi-Fi)",
                "Niega el control de radios para las aplicaciones.",
                ConsentStore + @"\radios",
                "Value",
                disabled: "Deny",
                enabled: "Allow",
                requiresAdmin: false,
                requiresRestart: false),

            // ---------------- Microsoft Edge ----------------
            Dword(
                "edge-metrics",
                TelemetryCategory.Edge,
                "Telemetría de Microsoft Edge",
                "Desactiva el envío de métricas de uso de Edge. Requiere reiniciar el navegador.",
                EdgePolicy,
                "MetricsReportingEnabled",
                disabled: "0",
                enabled: "1",
                requiresAdmin: true,
                requiresRestart: false),

            Dword(
                "edge-site-info",
                TelemetryCategory.Edge,
                "Información de sitios en Edge",
                "Evita que Edge envíe información de los sitios visitados. Requiere reiniciar el navegador.",
                EdgePolicy,
                "SendSiteInfoToImproveServices",
                disabled: "0",
                enabled: "1",
                requiresAdmin: true,
                requiresRestart: false),

            Dword(
                "edge-personalization",
                TelemetryCategory.Edge,
                "Personalización de Edge",
                "Desactiva los informes de personalización de Edge. Requiere reiniciar el navegador.",
                EdgePolicy,
                "PersonalizationReportingEnabled",
                disabled: "0",
                enabled: "1",
                requiresAdmin: true,
                requiresRestart: false),

            // ---------------- Reporte de errores de Windows ----------------
            Dword(
                "wer-disabled",
                TelemetryCategory.ReporteDeErrores,
                "Informe de errores de Windows",
                "Desactiva el servicio de informes de errores de Windows.",
                ErrorReporting,
                "Disabled",
                disabled: "1",
                enabled: "0",
                requiresAdmin: true,
                requiresRestart: false),

            Dword(
                "wer-ui",
                TelemetryCategory.ReporteDeErrores,
                "Interfaz de errores",
                "Evita que Windows muestre el cuadro de diálogo de errores.",
                ErrorReporting,
                "DontShowUI",
                disabled: "1",
                enabled: "0",
                requiresAdmin: true,
                requiresRestart: false),

            Dword(
                "wer-queue",
                TelemetryCategory.ReporteDeErrores,
                "Cola de informes de errores",
                "Fuerza el modo de cola en lugar del envío inmediato.",
                ErrorReporting,
                "ForceQueue",
                disabled: "1",
                enabled: "0",
                requiresAdmin: true,
                requiresRestart: false),

            // ---------------- OneDrive ----------------
            Dword(
                "onedrive-sync-policy",
                TelemetryCategory.OneDrive,
                "Sincronización de OneDrive (directiva)",
                "Bloquea la sincronización de archivos con OneDrive.",
                OneDrivePolicy,
                "DisableFileSyncNGSC",
                disabled: "1",
                enabled: "0",
                requiresAdmin: true,
                requiresRestart: false),

            Dword(
                "onedrive-sync-legacy",
                TelemetryCategory.OneDrive,
                "Sincronización de OneDrive (directiva heredada)",
                "Aplica la directiva heredada que bloquea la sincronización.",
                OneDrivePolicy,
                "DisableFileSync",
                disabled: "1",
                enabled: "0",
                requiresAdmin: true,
                requiresRestart: false),

            // ---------------- Sugerencias y contenido ----------------
            Dword(
                "suggestions-settings-pane",
                TelemetryCategory.Sugerencias,
                "Sugerencias en Configuración",
                "Desactiva las sugerencias en el panel de Configuración.",
                ContentDelivery,
                "SystemPaneSuggestionsEnabled",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: false),

            Dword(
                "suggestions-soft-landing",
                TelemetryCategory.Sugerencias,
                "Experiencia de bienvenida",
                "Desactiva la experiencia de bienvenida de Windows.",
                ContentDelivery,
                "SoftLandingEnabled",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: false),

            Dword(
                "suggestions-start-menu",
                TelemetryCategory.Sugerencias,
                "Sugerencias en el menú Inicio",
                "Desactiva las aplicaciones y sugerencias del menú Inicio.",
                ContentDelivery,
                "SubscribedContent-338388Enabled",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: false),

            Dword(
                "suggestions-tips",
                TelemetryCategory.Sugerencias,
                "Consejos de Windows",
                "Desactiva los consejos que Windows muestra al usuario.",
                ContentDelivery,
                "SubscribedContent-338389Enabled",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: false),

            Dword(
                "suggestions-welcome",
                TelemetryCategory.Sugerencias,
                "Sugerencias de bienvenida",
                "Desactiva las sugerencias mostradas tras una actualización.",
                ContentDelivery,
                "SubscribedContent-353694Enabled",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: false),

            Dword(
                "silent-app-installations",
                TelemetryCategory.Sugerencias,
                "Instalación silenciosa de aplicaciones",
                "Impide que Windows instale aplicaciones sugeridas sin intervención.",
                ContentDelivery,
                "SilentInstalledAppsEnabled",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: false),

            Dword(
                "lockscreen-spotlight",
                TelemetryCategory.Sugerencias,
                "Datos curiosos de la pantalla de bloqueo",
                "Desactiva los datos y consejos de Windows Spotlight en la pantalla de bloqueo.",
                ContentDelivery,
                "RotatingLockScreenOverlayEnabled",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: false),

            // ---------------- Historial de actividad ----------------
            Dword(
                "activity-feed",
                TelemetryCategory.Actividad,
                "Historial de actividad",
                "Desactiva el historial de actividad de Windows.",
                SystemPolicy,
                "EnableActivityFeed",
                disabled: "0",
                enabled: "1",
                requiresAdmin: true,
                requiresRestart: false),

            Dword(
                "activity-publish",
                TelemetryCategory.Actividad,
                "Publicación de actividad",
                "Evita que Windows publique las actividades del usuario.",
                SystemPolicy,
                "PublishUserActivities",
                disabled: "0",
                enabled: "1",
                requiresAdmin: true,
                requiresRestart: false),

            Dword(
                "activity-upload",
                TelemetryCategory.Actividad,
                "Carga de actividad",
                "Evita que Windows cargue las actividades a la nube.",
                SystemPolicy,
                "UploadUserActivities",
                disabled: "0",
                enabled: "1",
                requiresAdmin: true,
                requiresRestart: false),

            // ---------------- Comentarios e idioma ----------------
            Dword(
                "feedback-frequency",
                TelemetryCategory.Comentarios,
                "Frecuencia de comentarios",
                "Evita que Windows solicite comentarios de forma periódica.",
                @"HKCU\Software\Microsoft\Siuf\Rules",
                "NumberOfSIUFInPeriod",
                disabled: "0",
                enabled: "1",
                requiresAdmin: false,
                requiresRestart: false),

            Dword(
                "language-list-access",
                TelemetryCategory.Comentarios,
                "Acceso a la lista de idiomas",
                "Impide que los sitios web accedan a la lista de idiomas del usuario.",
                @"HKCU\Control Panel\International\User Profile",
                "HttpAcceptLanguageOptOut",
                disabled: "1",
                enabled: "0",
                requiresAdmin: false,
                requiresRestart: false)
        };

    private static TelemetryKey Dword(
        string id,
        TelemetryCategory category,
        string displayName,
        string description,
        string registryPath,
        string valueName,
        string disabled,
        string enabled,
        bool requiresAdmin,
        bool requiresRestart) =>
        new()
        {
            Id = id,
            Category = category,
            DisplayName = displayName,
            Description = description,
            RegistryPath = registryPath,
            ValueName = valueName,
            ValueKind = TelemetryValueKind.Dword,
            DisabledValue = disabled,
            EnabledValue = enabled,
            RequiresAdmin = requiresAdmin,
            RequiresRestart = requiresRestart
        };

    private static TelemetryKey String(
        string id,
        TelemetryCategory category,
        string displayName,
        string description,
        string registryPath,
        string valueName,
        string disabled,
        string enabled,
        bool requiresAdmin,
        bool requiresRestart) =>
        new()
        {
            Id = id,
            Category = category,
            DisplayName = displayName,
            Description = description,
            RegistryPath = registryPath,
            ValueName = valueName,
            ValueKind = TelemetryValueKind.String,
            DisabledValue = disabled,
            EnabledValue = enabled,
            RequiresAdmin = requiresAdmin,
            RequiresRestart = requiresRestart
        };
}
