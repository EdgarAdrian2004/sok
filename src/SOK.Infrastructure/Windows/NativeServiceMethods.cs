using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using SOK.Domain.Enums;

namespace SOK.Infrastructure.Windows;

/// <summary>
/// Llamadas nativas al Service Control Manager para cambiar el tipo de
/// inicio de un servicio. .NET no expone <c>ChangeStartMode</c> en
/// <c>ServiceController</c> desde .NET Core, por lo que se usa
/// <c>ChangeServiceConfig</c> de <c>advapi32.dll</c>.
/// </summary>
[SupportedOSPlatform("windows")]
internal static class NativeServiceMethods
{
    private const uint ScManagerConnect = 0x0001;
    private const uint ServiceChangeConfig = 0x0002;
    private const uint ServiceNoChange = 0xFFFFFFFF;

    private const uint ServiceAutoStart = 0x00000002;
    private const uint ServiceDemandStart = 0x00000003;
    private const uint ServiceDisabled = 0x00000004;

    /// <summary>
    /// Cambia el tipo de inicio del servicio indicado. Lanza
    /// <see cref="Win32Exception"/> si el SCM rechaza la operación.
    /// </summary>
    public static void ChangeStartMode(string serviceName, ServiceStartMode mode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        var startType = mode switch
        {
            ServiceStartMode.Automatico => ServiceAutoStart,
            ServiceStartMode.Deshabilitado => ServiceDisabled,
            _ => ServiceDemandStart
        };

        var manager = OpenSCManager(null, null, ScManagerConnect);
        if (manager == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "No se pudo abrir el Service Control Manager.");
        }

        try
        {
            var service = OpenService(manager, serviceName, ServiceChangeConfig);
            if (service == IntPtr.Zero)
            {
                throw new Win32Exception(
                    Marshal.GetLastWin32Error(),
                    $"No se pudo abrir el servicio '{serviceName}'.");
            }

            try
            {
                var ok = ChangeServiceConfig(
                    service,
                    ServiceNoChange,
                    startType,
                    ServiceNoChange,
                    null,
                    null,
                    IntPtr.Zero,
                    null,
                    null,
                    null,
                    null);

                if (!ok)
                {
                    throw new Win32Exception(
                        Marshal.GetLastWin32Error(),
                        $"No se pudo cambiar el tipo de inicio de '{serviceName}'.");
                }
            }
            finally
            {
                CloseServiceHandle(service);
            }
        }
        finally
        {
            CloseServiceHandle(manager);
        }
    }

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr OpenSCManager(string? machineName, string? databaseName, uint desiredAccess);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr OpenService(IntPtr manager, string serviceName, uint desiredAccess);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ChangeServiceConfig(
        IntPtr service,
        uint serviceType,
        uint startType,
        uint errorControl,
        string? binaryPathName,
        string? loadOrderGroup,
        IntPtr tagId,
        string? dependencies,
        string? serviceStartName,
        string? password,
        string? displayName);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseServiceHandle(IntPtr handle);
}
