using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Application.Common;

/// <summary>
/// Catálogo estático de los servicios de Windows que SOK conoce. Cada
/// llamada devuelve definiciones nuevas para que el estado observado no se
/// comparta entre consultas.
/// </summary>
/// <remarks>
/// El criterio de los perfiles es conservador en usabilidad: ningún perfil
/// deshabilita Bluetooth, Windows Search, Windows Hello, impresión, soporte
/// remoto ni las aplicaciones nativas de Windows. Los servicios de alto
/// riesgo se listan para cambio manual, nunca automático.
/// </remarks>
public static class ServiceCatalog
{
    /// <summary>
    /// Devuelve las definiciones del catálogo con estado inicial no
    /// aplicable.
    /// </summary>
    public static IReadOnlyList<ServiceEntry> CreateDefinitions() =>
        new List<ServiceEntry>
        {
            // ---------------- Telemetría ----------------
            Entry(
                "DiagTrack",
                "Experiencias de usuario conectado y telemetría",
                "Servicio de seguimiento de diagnóstico que envía datos de uso a Microsoft.",
                ServiceCategory.Telemetria,
                ServiceStartMode.Automatico,
                gamer: true,
                ofimatico: true,
                requiresRestart: true),

            Entry(
                "dmwappushservice",
                "Enrutamiento de mensajes WAP push",
                "Servicio de enrutamiento de mensajes WAP push usado por la telemetría.",
                ServiceCategory.Telemetria,
                ServiceStartMode.Manual,
                gamer: true,
                ofimatico: true,
                requiresRestart: true),

            Entry(
                "WerSvc",
                "Informe de errores de Windows",
                "Recopila y envía informes de errores de las aplicaciones a Microsoft.",
                ServiceCategory.Telemetria,
                ServiceStartMode.Manual,
                gamer: true,
                ofimatico: true),

            // ---------------- Obsoletos ----------------
            Entry(
                "Fax",
                "Fax",
                "Servicio de envío y recepción de faxes. Prácticamente en desuso.",
                ServiceCategory.Obsoleto,
                ServiceStartMode.Manual,
                gamer: true,
                ofimatico: true),

            Entry(
                "RpcLocator",
                "Localizador de llamadas a procedimiento remoto",
                "Servicio heredado para equipos sin Active Directory. No existe en Windows 10/11 modernos.",
                ServiceCategory.Obsoleto,
                ServiceStartMode.Manual,
                gamer: true,
                ofimatico: true),

            Entry(
                "PrintNotify",
                "Notificaciones de impresión",
                "Extensión de notificaciones de la cola de impresión. No impide imprimir.",
                ServiceCategory.Obsoleto,
                ServiceStartMode.Manual,
                gamer: true,
                ofimatico: false),

            Entry(
                "WalletService",
                "Servicio de cartera",
                "Almacena la cartera digital de Microsoft. Sin uso en entornos educativos.",
                ServiceCategory.Obsoleto,
                ServiceStartMode.Manual,
                gamer: true,
                ofimatico: true),

            Entry(
                "PhoneSvc",
                "Servicio de telefonía",
                "Administra el estado de telefonía del equipo. No afecta a la conexión de red.",
                ServiceCategory.Obsoleto,
                ServiceStartMode.Manual,
                gamer: true,
                ofimatico: true),

            Entry(
                "SDRSVC",
                "Copia de seguridad de Windows",
                "Servicio de copia de seguridad y restauración heredado de Windows 7.",
                ServiceCategory.Obsoleto,
                ServiceStartMode.Manual,
                gamer: true,
                ofimatico: true),

            Entry(
                "WMPNetworkSvc",
                "Uso compartido multimedia de Windows Media Player",
                "Comparte bibliotecas multimedia en la red local.",
                ServiceCategory.Obsoleto,
                ServiceStartMode.Manual,
                gamer: true,
                ofimatico: true),

            // ---------------- Rendimiento ----------------
            Entry(
                "SysMain",
                "SysMain (Superfetch)",
                "Precarga aplicaciones en memoria. En SSD no aporta beneficio y consume recursos.",
                ServiceCategory.Rendimiento,
                ServiceStartMode.Automatico,
                gamer: true,
                ofimatico: true,
                requiresRestart: true),

            Entry(
                "TrkWks",
                "Seguimiento de vínculos distribuidos",
                "Mantiene vínculos entre archivos movidos dentro del equipo.",
                ServiceCategory.Rendimiento,
                ServiceStartMode.Automatico,
                gamer: true,
                ofimatico: true),

            Entry(
                "MapsBroker",
                "Administrador de mapas descargados",
                "Descarga y actualiza mapas sin conexión para la aplicación Mapas.",
                ServiceCategory.Rendimiento,
                ServiceStartMode.Automatico,
                gamer: true,
                ofimatico: true),

            Entry(
                "WSearch",
                "Windows Search",
                "Indexa archivos y correos para la búsqueda de Windows. Deshabilitarlo afecta la búsqueda del Explorador y del menú Inicio.",
                ServiceCategory.Rendimiento,
                ServiceStartMode.Automatico,
                gamer: false,
                ofimatico: false,
                highRisk: true,
                requiresRestart: true),

            // ---------------- Privacidad ----------------
            Entry(
                "lfsvc",
                "Servicio de geolocalización",
                "Determina la ubicación del equipo para aplicaciones y servicios.",
                ServiceCategory.Privacidad,
                ServiceStartMode.Manual,
                gamer: true,
                ofimatico: true),

            // ---------------- Red ----------------
            Entry(
                "SSDPSRV",
                "Detección SSDP",
                "Descubre dispositivos UPnP en la red local.",
                ServiceCategory.Red,
                ServiceStartMode.Manual,
                gamer: true,
                ofimatico: true),

            Entry(
                "upnphost",
                "Host de dispositivos UPnP",
                "Aloja dispositivos UPnP conectados al equipo.",
                ServiceCategory.Red,
                ServiceStartMode.Manual,
                gamer: true,
                ofimatico: true),

            Entry(
                "lmhosts",
                "Compatibilidad con NetBIOS sobre TCP/IP",
                "Resuelve nombres NetBIOS heredados en la red local.",
                ServiceCategory.Red,
                ServiceStartMode.Manual,
                gamer: true,
                ofimatico: true),

            Entry(
                "SharedAccess",
                "Uso compartido de conexión a Internet",
                "Permite compartir la conexión a Internet del equipo con otros dispositivos.",
                ServiceCategory.Red,
                ServiceStartMode.Manual,
                gamer: false,
                ofimatico: false,
                highRisk: true),

            // ---------------- Xbox ----------------
            Entry(
                "XboxGipSvc",
                "Entrada de juegos de Xbox",
                "Administra los mandos y accesorios de Xbox. La Barra de juegos no lo requiere.",
                ServiceCategory.Xbox,
                ServiceStartMode.Manual,
                gamer: false,
                ofimatico: true),

            Entry(
                "XboxNetApiSvc",
                "Red de Xbox Live",
                "Habilita la red de Xbox Live para juegos y aplicaciones.",
                ServiceCategory.Xbox,
                ServiceStartMode.Manual,
                gamer: false,
                ofimatico: true),

            Entry(
                "XblAuthManager",
                "Administrador de autenticación de Xbox Live",
                "Gestiona la autenticación de cuentas de Xbox Live.",
                ServiceCategory.Xbox,
                ServiceStartMode.Manual,
                gamer: false,
                ofimatico: true),

            Entry(
                "XblGameSave",
                "Guardado de juegos de Xbox Live",
                "Sincroniza las partidas guardadas con la nube de Xbox Live.",
                ServiceCategory.Xbox,
                ServiceStartMode.Manual,
                gamer: false,
                ofimatico: true),

            // ---------------- Bluetooth ----------------
            Entry(
                "bthserv",
                "Compatibilidad con Bluetooth",
                "Soporte del adaptador Bluetooth: auriculares, ratón, transferencia de archivos y Phone Link.",
                ServiceCategory.Bluetooth,
                ServiceStartMode.Manual,
                gamer: false,
                ofimatico: false,
                highRisk: true,
                requiresRestart: true),

            // ---------------- Biometría ----------------
            Entry(
                "WbioSrvc",
                "Servicio biométrico de Windows",
                "Habilita Windows Hello (huella digital o reconocimiento facial).",
                ServiceCategory.Biometria,
                ServiceStartMode.Manual,
                gamer: false,
                ofimatico: false,
                highRisk: true),

            // ---------------- Seguridad ----------------
            Entry(
                "RemoteRegistry",
                "Registro remoto",
                "Permite que usuarios remotos modifiquen el registro de este equipo.",
                ServiceCategory.Seguridad,
                ServiceStartMode.Manual,
                gamer: false,
                ofimatico: false),

            Entry(
                "RemoteAccess",
                "Acceso remoto",
                "Proporciona enrutamiento y acceso remoto (VPN entrante y RDP).",
                ServiceCategory.Seguridad,
                ServiceStartMode.Manual,
                gamer: false,
                ofimatico: false,
                highRisk: true),

            Entry(
                "seclogon",
                "Inicio de sesión secundario",
                "Permite iniciar procesos con credenciales alternativas («Ejecutar como otro usuario»).",
                ServiceCategory.Seguridad,
                ServiceStartMode.Manual,
                gamer: false,
                ofimatico: false),

            // ---------------- Periféricos ----------------
            Entry(
                "stisvc",
                "Adquisición de imágenes (escáneres)",
                "Detecta escáneres y cámaras conectados al equipo.",
                ServiceCategory.Perifericos,
                ServiceStartMode.Manual,
                gamer: false,
                ofimatico: false),

            Entry(
                "Spooler",
                "Cola de impresión",
                "Administra la impresión. Deshabilitarlo impide imprimir por completo.",
                ServiceCategory.Perifericos,
                ServiceStartMode.Automatico,
                gamer: false,
                ofimatico: false,
                highRisk: true,
                requiresRestart: true)
        };

    private static ServiceEntry Entry(
        string id,
        string displayName,
        string description,
        ServiceCategory category,
        ServiceStartMode defaultStartMode,
        bool gamer,
        bool ofimatico,
        bool highRisk = false,
        bool requiresRestart = false) =>
        new()
        {
            Id = id,
            DisplayName = displayName,
            Description = description,
            Category = category,
            DefaultStartMode = defaultStartMode,
            DisabledInGamer = gamer,
            DisabledInOfimatico = ofimatico,
            HighRisk = highRisk,
            RequiresRestart = requiresRestart
        };
}
