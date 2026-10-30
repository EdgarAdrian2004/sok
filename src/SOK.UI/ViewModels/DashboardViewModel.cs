using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SOK.Application.Abstractions;
using SOK.Domain.Enums;

namespace SOK.UI.ViewModels;

/// <summary>
/// ViewModel del panel de control. Actualiza las métricas cada tres
/// segundos y permite crear un respaldo completo.
/// </summary>
public partial class DashboardViewModel : ObservableObject, IDisposable
{
    private readonly ISystemInfoProvider _systemInfoProvider;
    private readonly IBackupManager _backupManager;
    private readonly ILogService _logService;
    private readonly DispatcherTimer _timer;
    private bool _isRefreshing;

    [ObservableProperty]
    private double cpuUsage;

    [ObservableProperty]
    private double ramUsagePercent;

    [ObservableProperty]
    private double diskUsagePercent;

    [ObservableProperty]
    private string cpuUsageText = "—";

    [ObservableProperty]
    private string ramUsageText = "—";

    [ObservableProperty]
    private string diskUsageText = "—";

    [ObservableProperty]
    private string osName = "—";

    [ObservableProperty]
    private string processorName = "—";

    [ObservableProperty]
    private string machineName = "—";

    [ObservableProperty]
    private string uptimeText = "—";

    [ObservableProperty]
    private string lastUpdatedText = "Sin datos";

    [ObservableProperty]
    private string statusMessage = "Listo.";

    [ObservableProperty]
    private string lastBackupPath = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public DashboardViewModel(
        ISystemInfoProvider systemInfoProvider,
        IBackupManager backupManager,
        ILogService logService)
    {
        _systemInfoProvider = systemInfoProvider ?? throw new ArgumentNullException(nameof(systemInfoProvider));
        _backupManager = backupManager ?? throw new ArgumentNullException(nameof(backupManager));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(3)
        };
        _timer.Tick += async (_, _) => await RefreshAsync(isManual: false);
        _timer.Start();

        _ = RefreshAsync(isManual: false);
    }

    [RelayCommand]
    private Task RefreshNowAsync() => RefreshAsync(isManual: true);

    [RelayCommand]
    private async Task CreateBackupAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        StatusMessage = "Creando respaldo completo...";

        try
        {
            var backup = await _backupManager.CreateFullBackupAsync(
                "Respaldo manual desde el panel de control");

            LastBackupPath = $"Archivo: {backup.FilePath}";
            StatusMessage = $"Respaldo creado correctamente ({backup.Name}).";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error al crear el respaldo: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RefreshAsync(bool isManual)
    {
        if (_isRefreshing)
        {
            return;
        }

        _isRefreshing = true;

        try
        {
            var snapshot = await _systemInfoProvider.GetSnapshotAsync();

            CpuUsage = snapshot.CpuUsagePercent;
            RamUsagePercent = snapshot.RamUsagePercent;
            DiskUsagePercent = snapshot.DiskUsagePercent;

            CpuUsageText = $"{snapshot.CpuUsagePercent:0} %";
            RamUsageText = $"{ToGb(snapshot.RamUsedBytes):0.0} / {ToGb(snapshot.RamTotalBytes):0.0} GB";
            DiskUsageText = $"{ToGb(snapshot.DiskFreeBytes):0.0} / {ToGb(snapshot.DiskTotalBytes):0.0} GB";

            OsName = snapshot.OsName;
            ProcessorName = snapshot.ProcessorName;
            MachineName = snapshot.MachineName;
            UptimeText = $"Encendido hace {snapshot.Uptime.Days} d {snapshot.Uptime.Hours} h {snapshot.Uptime.Minutes} min";
            LastUpdatedText = $"Actualizado: {DateTime.Now:HH:mm:ss}";
            StatusMessage = "Panel actualizado correctamente.";

            if (isManual)
            {
                await _logService.LogAsync(
                    LogModule.Dashboard,
                    "Actualizar panel",
                    null,
                    ActionResult.Exitoso);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo actualizar el panel: {ex.Message}";

            if (isManual)
            {
                await _logService.LogAsync(
                    LogModule.Dashboard,
                    "Actualizar panel",
                    null,
                    ActionResult.Fallido,
                    ex.Message);
            }
        }
        finally
        {
            _isRefreshing = false;
        }
    }

    private static double ToGb(long bytes) => bytes / 1024d / 1024d / 1024d;

    public void Dispose()
    {
        _timer.Stop();
    }
}
