using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SOK.Application.Abstractions;
using SOK.Application.DTOs;

namespace SOK.UI.ViewModels;

/// <summary>
/// ViewModel del módulo de memoria. Actualiza el medidor cada tres segundos,
/// permite liberar memoria manualmente y aplica la liberación automática
/// cuando el uso de RAM alcanza el umbral configurado.
/// </summary>
public partial class MemoryViewModel : ObservableObject, IDisposable
{
    private const int RefreshIntervalSeconds = 3;

    private readonly IMemoryManager _memoryManager;
    private readonly DispatcherTimer? _timer;

    private bool _isRefreshing;
    private bool _isLoadingSettings;
    private bool _savePending;
    private Task _pendingSaveTask = Task.CompletedTask;

    [ObservableProperty]
    private double ramUsagePercent;

    [ObservableProperty]
    private string ramUsageText = "—";

    [ObservableProperty]
    private string ramAvailableText = "—";

    [ObservableProperty]
    private string lastUpdatedText = "Sin datos";

    [ObservableProperty]
    private string statusMessage = "Consultando la memoria del equipo...";

    [ObservableProperty]
    private string lastReleaseText = "Sin liberaciones todavía.";

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AutoReleaseSummaryText))]
    private bool autoReleaseEnabled;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AutoReleaseSummaryText))]
    [NotifyPropertyChangedFor(nameof(AutoReleaseThresholdText))]
    private int autoReleaseThreshold = MemoryAutoReleaseOptions.DefaultThresholdPercent;

    public MemoryViewModel(IMemoryManager memoryManager)
        : this(memoryManager, startTimer: true)
    {
    }

    internal MemoryViewModel(IMemoryManager memoryManager, bool startTimer)
    {
        _memoryManager = memoryManager ?? throw new ArgumentNullException(nameof(memoryManager));

        if (startTimer)
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(RefreshIntervalSeconds)
            };
            _timer.Tick += async (_, _) => await RefreshAsync(isManual: false);
            _timer.Start();
        }

        InitializationTask = InitializeAsync();
    }

    /// <summary>Tarea de carga inicial; se expone para las pruebas.</summary>
    internal Task InitializationTask { get; }

    /// <summary>Última tarea de guardado de configuración; se expone para las pruebas.</summary>
    internal Task PendingSaveTask => _pendingSaveTask;

    public string AutoReleaseSummaryText => AutoReleaseEnabled
        ? $"La liberación automática se ejecutará cuando el uso de RAM alcance o supere el {AutoReleaseThreshold} %."
        : "La liberación automática está desactivada.";

    public string AutoReleaseThresholdText => $"{AutoReleaseThreshold} %";

    [RelayCommand(CanExecute = nameof(CanRelease))]
    private Task ReleaseNowAsync() => ReleaseAsync();

    [RelayCommand]
    private Task RefreshNowAsync() => RefreshAsync(isManual: true);

    private bool CanRelease() => !IsBusy;

    partial void OnIsBusyChanged(bool value) =>
        ReleaseNowCommand.NotifyCanExecuteChanged();

    partial void OnAutoReleaseEnabledChanged(bool value)
    {
        if (!_isLoadingSettings)
        {
            QueueSaveOptions();
        }
    }

    partial void OnAutoReleaseThresholdChanged(int value)
    {
        if (_isLoadingSettings)
        {
            return;
        }

        var normalized = Math.Clamp(
            value,
            MemoryAutoReleaseOptions.MinThresholdPercent,
            MemoryAutoReleaseOptions.MaxThresholdPercent);

        if (normalized != value)
        {
            AutoReleaseThreshold = normalized;
            return;
        }

        QueueSaveOptions();
    }

    private async Task InitializeAsync()
    {
        await LoadSettingsAsync();
        await RefreshAsync(isManual: false);
    }

    private async Task LoadSettingsAsync()
    {
        _isLoadingSettings = true;

        try
        {
            var options = await _memoryManager.GetAutoReleaseOptionsAsync();
            AutoReleaseEnabled = options.Enabled;
            AutoReleaseThreshold = options.ThresholdPercent;
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo cargar la configuración de memoria: {ex.Message}";
        }
        finally
        {
            _isLoadingSettings = false;
        }
    }

    private async Task RefreshAsync(bool isManual)
    {
        if (_isRefreshing || IsBusy)
        {
            return;
        }

        _isRefreshing = true;

        try
        {
            var snapshot = await _memoryManager.GetSnapshotAsync();
            ApplySnapshot(snapshot);

            var autoReleased = false;
            if (!IsBusy)
            {
                var result = await _memoryManager.TryAutoReleaseAsync(
                    snapshot,
                    new MemoryAutoReleaseOptions(AutoReleaseEnabled, AutoReleaseThreshold));

                if (result is not null)
                {
                    ApplyReleaseResult(result, automatic: true);
                    autoReleased = true;
                }
            }

            if (isManual && !autoReleased && !IsBusy)
            {
                StatusMessage = "Medidor de memoria actualizado correctamente.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo actualizar el medidor de memoria: {ex.Message}";
        }
        finally
        {
            _isRefreshing = false;
        }
    }

    private async Task ReleaseAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        StatusMessage = "Liberando memoria de los procesos activos...";

        try
        {
            var result = await _memoryManager.ReleaseAsync();
            ApplyReleaseResult(result, automatic: false);

            var snapshot = await _memoryManager.GetSnapshotAsync();
            ApplySnapshot(snapshot);
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo liberar la memoria: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplySnapshot(MemorySnapshot snapshot)
    {
        RamUsagePercent = snapshot.UsagePercent;
        RamUsageText = $"{ToGb(snapshot.UsedBytes):0.0} / {ToGb(snapshot.TotalBytes):0.0} GB en uso";
        RamAvailableText = $"{ToGb(snapshot.AvailableBytes):0.0} GB disponibles";
        LastUpdatedText = $"Actualizado: {DateTime.Now:HH:mm:ss}";
    }

    private void ApplyReleaseResult(MemoryReleaseResult result, bool automatic)
    {
        var origin = automatic ? "Liberación automática" : "Liberación manual";
        LastReleaseText =
            $"{origin} a las {DateTime.Now:HH:mm:ss}: " +
            $"{result.ProcessesSucceeded} de {result.ProcessesTried} procesos; " +
            $"{FormatBytes(result.FreedBytesEstimate)} estimados.";

        StatusMessage = result.Message;
    }

    private void QueueSaveOptions()
    {
        _savePending = true;

        if (_pendingSaveTask.IsCompleted)
        {
            _pendingSaveTask = SaveOptionsLoopAsync();
        }
    }

    private async Task SaveOptionsLoopAsync()
    {
        while (_savePending)
        {
            _savePending = false;

            var options = new MemoryAutoReleaseOptions(
                AutoReleaseEnabled,
                AutoReleaseThreshold);

            try
            {
                await _memoryManager.SaveAutoReleaseOptionsAsync(options);
            }
            catch (Exception ex)
            {
                StatusMessage = $"No se pudo guardar la configuración de memoria: {ex.Message}";
            }
        }
    }

    private static double ToGb(long bytes) => bytes / 1024d / 1024d / 1024d;

    private static string FormatBytes(long bytes)
    {
        if (bytes <= 0)
        {
            return "0 MB";
        }

        var megabytes = bytes / 1024d / 1024d;
        return megabytes >= 1024
            ? $"{megabytes / 1024d:0.00} GB"
            : $"{megabytes:0} MB";
    }

    public void Dispose() => _timer?.Stop();
}
