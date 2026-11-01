using System.Globalization;
using System.Runtime.Versioning;
using Microsoft.Win32;
using SOK.Application.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.Infrastructure.Windows;

/// <summary>
/// Lee y escribe valores del registro de Windows para el módulo de
/// telemetría. Usa la API nativa de .NET en lugar de reg.exe para poder
/// comparar tipos y valores con precisión.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class TelemetryRegistryService : ITelemetryRegistryService
{
    public Task<TelemetryValue?> ReadValueAsync(
        string registryPath,
        string valueName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.Run(() => ReadCore(registryPath, valueName), cancellationToken);
    }

    public Task WriteValueAsync(
        string registryPath,
        string valueName,
        string value,
        TelemetryValueKind kind,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.Run(() => WriteCore(registryPath, valueName, value, kind), cancellationToken);
    }

    private static TelemetryValue? ReadCore(string registryPath, string valueName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(registryPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(valueName);

        var (hive, subPath) = RegistryPathHelper.Parse(registryPath);

        using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
        using var key = baseKey.OpenSubKey(subPath, writable: false);
        if (key is null)
        {
            return null;
        }

        var rawValue = key.GetValue(valueName, null, RegistryValueOptions.DoNotExpandEnvironmentNames);
        if (rawValue is null)
        {
            return null;
        }

        var registryKind = key.GetValueKind(valueName);

        return registryKind switch
        {
            RegistryValueKind.DWord => new TelemetryValue(
                TelemetryValueKind.Dword,
                Convert.ToInt32(rawValue, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture)),
            RegistryValueKind.QWord => new TelemetryValue(
                TelemetryValueKind.Dword,
                Convert.ToInt64(rawValue, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture)),
            _ => new TelemetryValue(TelemetryValueKind.String, rawValue.ToString() ?? string.Empty)
        };
    }

    private static void WriteCore(
        string registryPath,
        string valueName,
        string value,
        TelemetryValueKind kind)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(registryPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(valueName);

        var (hive, subPath) = RegistryPathHelper.Parse(registryPath);

        using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
        using var key = baseKey.CreateSubKey(subPath, writable: true)
            ?? throw new InvalidOperationException($"No se pudo abrir la clave '{registryPath}' para escritura.");

        switch (kind)
        {
            case TelemetryValueKind.Dword:
                var number = int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                key.SetValue(valueName, number, RegistryValueKind.DWord);
                break;

            default:
                key.SetValue(valueName, value, RegistryValueKind.String);
                break;
        }
    }
}
