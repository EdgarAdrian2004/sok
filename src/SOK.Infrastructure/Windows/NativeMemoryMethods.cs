using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace SOK.Infrastructure.Windows;

/// <summary>
/// Llamadas nativas para medir y liberar memoria. Usa
/// <c>GlobalMemoryStatusEx</c> (kernel32), <c>EnumProcesses</c> y
/// <c>EmptyWorkingSet</c> (psapi), y <c>OpenProcess</c>/<c>CloseHandle</c>
/// (kernel32).
/// </summary>
[SupportedOSPlatform("windows")]
internal static class NativeMemoryMethods
{
    private const uint ProcessQueryLimitedInformation = 0x1000;
    private const uint ProcessSetQuota = 0x0100;

    private const int InitialProcessCapacity = 1024;
    private const int MaxProcessCapacity = 65536;

    /// <summary>Estructura nativa <c>MEMORYSTATUSEX</c>.</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MemoryStatusEx
    {
        public uint Length;
        public uint MemoryLoad;
        public ulong TotalPhys;
        public ulong AvailPhys;
        public ulong TotalPageFile;
        public ulong AvailPageFile;
        public ulong TotalVirtual;
        public ulong AvailVirtual;
        public ulong AvailExtendedVirtual;
    }

    /// <summary>Consulta el estado global de la memoria física y virtual.</summary>
    public static MemoryStatusEx GetMemoryStatus()
    {
        var status = new MemoryStatusEx
        {
            Length = (uint)Marshal.SizeOf<MemoryStatusEx>()
        };

        if (!GlobalMemoryStatusEx(ref status))
        {
            throw new Win32Exception(
                Marshal.GetLastWin32Error(),
                "No se pudo consultar el estado de la memoria.");
        }

        return status;
    }

    /// <summary>
    /// Enumera los identificadores de los procesos activos. Si la lista no
    /// cabe en el búfer inicial, se amplía hasta el límite soportado.
    /// </summary>
    public static IReadOnlyList<uint> EnumProcessIds()
    {
        var capacity = InitialProcessCapacity;

        while (capacity <= MaxProcessCapacity)
        {
            var buffer = new uint[capacity];
            var size = (uint)(buffer.Length * sizeof(uint));

            if (!EnumProcesses(buffer, size, out var bytesReturned))
            {
                throw new Win32Exception(
                    Marshal.GetLastWin32Error(),
                    "No se pudieron enumerar los procesos.");
            }

            var count = (int)(bytesReturned / sizeof(uint));
            if (count < capacity)
            {
                var processIds = new List<uint>(count);
                for (var index = 0; index < count; index++)
                {
                    if (buffer[index] != 0)
                    {
                        processIds.Add(buffer[index]);
                    }
                }

                return processIds;
            }

            capacity *= 2;
        }

        throw new InvalidOperationException(
            "La cantidad de procesos supera el límite soportado.");
    }

    /// <summary>Abre un proceso con permisos para vaciar su working set.</summary>
    public static IntPtr OpenProcess(uint processId) =>
        OpenProcessNative(
            ProcessQueryLimitedInformation | ProcessSetQuota,
            inheritHandle: false,
            processId);

    /// <summary>Solicita al sistema que vacíe el working set del proceso.</summary>
    public static bool EmptyWorkingSet(IntPtr processHandle) =>
        EmptyWorkingSetNative(processHandle);

    /// <summary>Cierra un handle nativo; ignora el handle nulo.</summary>
    public static void CloseHandle(IntPtr handle)
    {
        if (handle != IntPtr.Zero)
        {
            CloseHandleNative(handle);
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(ref MemoryStatusEx buffer);

    [DllImport("psapi.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumProcesses(
        [Out] uint[] processIds,
        uint size,
        out uint bytesReturned);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcessNative(
        uint desiredAccess,
        [MarshalAs(UnmanagedType.Bool)] bool inheritHandle,
        uint processId);

    [DllImport("psapi.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EmptyWorkingSetNative(IntPtr processHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandleNative(IntPtr handle);
}
