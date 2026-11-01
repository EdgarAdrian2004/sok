using System.Runtime.Versioning;
using Microsoft.Win32;

namespace SOK.Infrastructure.Windows;

/// <summary>
/// Utilidades para interpretar rutas del registro de Windows con el
/// formato abreviado que usa SOK (<c>HKCU\...</c>, <c>HKLM\...</c>).
/// </summary>
[SupportedOSPlatform("windows")]
internal static class RegistryPathHelper
{
    public static (RegistryHive Hive, string SubPath) Parse(string fullPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullPath);

        var separator = fullPath.IndexOf('\\');
        var hiveName = separator < 0 ? fullPath : fullPath[..separator];
        var subPath = separator < 0 ? string.Empty : fullPath[(separator + 1)..];

        var hive = hiveName.ToUpperInvariant() switch
        {
            "HKCU" or "HKEY_CURRENT_USER" => RegistryHive.CurrentUser,
            "HKLM" or "HKEY_LOCAL_MACHINE" => RegistryHive.LocalMachine,
            "HKCR" or "HKEY_CLASSES_ROOT" => RegistryHive.ClassesRoot,
            "HKU" or "HKEY_USERS" => RegistryHive.Users,
            "HKCC" or "HKEY_CURRENT_CONFIG" => RegistryHive.CurrentConfig,
            _ => throw new ArgumentException($"Hive de registro desconocido: '{hiveName}'.", nameof(fullPath))
        };

        return (hive, subPath);
    }

    public static bool KeyExists(string fullPath)
    {
        var (hive, subPath) = Parse(fullPath);

        using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
        using var key = baseKey.OpenSubKey(subPath, writable: false);
        return key is not null;
    }
}
