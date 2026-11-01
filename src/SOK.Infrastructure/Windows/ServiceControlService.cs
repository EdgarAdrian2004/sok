using System.Management;
using System.Runtime.Versioning;
using System.ServiceProcess;
using SOK.Application.Abstractions;
using SOK.Domain.Entities;
using SOK.Domain.Enums;
using ServiceStartMode = SOK.Domain.Enums.ServiceStartMode;

namespace SOK.Infrastructure.Windows;

/// <summary>
/// Acceso al Service Control Manager. Lee con WMI (<c>Win32_Service</c>) y
/// escribe con <c>ServiceController</c> y <c>ChangeServiceConfig</c>.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class ServiceControlService : IServiceControlService
{
    private const string SelectAll =
        "SELECT Name, DisplayName, StartMode, State FROM Win32_Service";

    private static readonly TimeSpan StopTimeout = TimeSpan.FromSeconds(20);
    private static readonly TimeSpan StartTimeout = TimeSpan.FromSeconds(20);

    public Task<IReadOnlyList<ServiceSnapshot>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.Run<IReadOnlyList<ServiceSnapshot>>(() => Query(SelectAll), cancellationToken);
    }

    public Task<ServiceSnapshot?> GetAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        cancellationToken.ThrowIfCancellationRequested();

        return Task.Run(
            () =>
            {
                var escaped = name.Replace("'", "''", StringComparison.Ordinal);
                var query = $"{SelectAll} WHERE Name = '{escaped}'";
                return Query(query).FirstOrDefault();
            },
            cancellationToken);
    }

    public Task<bool> TryStopAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        cancellationToken.ThrowIfCancellationRequested();

        return Task.Run(() => StopCore(name), cancellationToken);
    }

    public Task<bool> TryStartAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        cancellationToken.ThrowIfCancellationRequested();

        return Task.Run(() => StartCore(name), cancellationToken);
    }

    public Task SetStartModeAsync(
        string name,
        ServiceStartMode mode,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        cancellationToken.ThrowIfCancellationRequested();

        return Task.Run(() => NativeServiceMethods.ChangeStartMode(name, mode), cancellationToken);
    }

    private static List<ServiceSnapshot> Query(string wql)
    {
        var snapshots = new List<ServiceSnapshot>();

        using var searcher = new ManagementObjectSearcher(wql);
        using var results = searcher.Get();

        foreach (ManagementBaseObject item in results)
        {
            using var service = item;
            snapshots.Add(new ServiceSnapshot(
                service["Name"]?.ToString() ?? string.Empty,
                service["DisplayName"]?.ToString() ?? string.Empty,
                MapStartMode(service["StartMode"]?.ToString()),
                MapState(service["State"]?.ToString())));
        }

        return snapshots;
    }

    private static bool StopCore(string name)
    {
        try
        {
            using var controller = new ServiceController(name);

            if (controller.Status == ServiceControllerStatus.Stopped)
            {
                return true;
            }

            if (!controller.CanStop)
            {
                return false;
            }

            controller.Stop();
            controller.WaitForStatus(ServiceControllerStatus.Stopped, StopTimeout);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            return false;
        }
        catch (System.TimeoutException)
        {
            return false;
        }
    }

    private static bool StartCore(string name)
    {
        try
        {
            using var controller = new ServiceController(name);

            if (controller.Status == ServiceControllerStatus.Running)
            {
                return true;
            }

            controller.Start();
            controller.WaitForStatus(ServiceControllerStatus.Running, StartTimeout);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            return false;
        }
        catch (System.TimeoutException)
        {
            return false;
        }
    }

    private static ServiceStartMode MapStartMode(string? value) =>
        value?.Trim().ToLowerInvariant() switch
        {
            "auto" or "automatic" => ServiceStartMode.Automatico,
            "manual" => ServiceStartMode.Manual,
            "disabled" => ServiceStartMode.Deshabilitado,
            "boot" or "system" => ServiceStartMode.Automatico,
            _ => ServiceStartMode.Desconocido
        };

    private static ServiceState MapState(string? value) =>
        value?.Trim().ToLowerInvariant() switch
        {
            "running" => ServiceState.EnEjecucion,
            "stopped" => ServiceState.Detenido,
            "paused" => ServiceState.Pausado,
            "start pending" => ServiceState.Iniciando,
            "stop pending" => ServiceState.Deteniendo,
            _ => ServiceState.Desconocido
        };
}
