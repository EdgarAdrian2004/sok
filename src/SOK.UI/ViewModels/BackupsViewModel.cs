using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SOK.Application.Abstractions;
using SOK.UI.Services;

namespace SOK.UI.ViewModels;

/// <summary>
/// ViewModel del módulo de respaldos: lista los respaldos registrados con
/// su integridad SHA-256 y restaura el sistema al punto seleccionado.
/// </summary>
public partial class BackupsViewModel : ObservableObject
{
    private readonly IRestoreManager _restoreManager;
    private readonly IUserDialogService _userDialogService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string statusMessage = "Consultando los respaldos registrados...";

    [ObservableProperty]
    private string summaryText = string.Empty;

    [ObservableProperty]
    private BackupItemViewModel? selectedBackup;

    public BackupsViewModel(
        IRestoreManager restoreManager,
        IUserDialogService userDialogService)
    {
        _restoreManager = restoreManager ?? throw new ArgumentNullException(nameof(restoreManager));
        _userDialogService = userDialogService ?? throw new ArgumentNullException(nameof(userDialogService));

        InitializationTask = LoadAsync();
    }

    /// <summary>Tarea de carga inicial; se expone para las pruebas.</summary>
    internal Task InitializationTask { get; }

    public ObservableCollection<BackupItemViewModel> Backups { get; } = new();

    [RelayCommand]
    private Task RefreshAsync() => LoadAsync();

    [RelayCommand(CanExecute = nameof(CanRestore))]
    private Task RestoreAsync(BackupItemViewModel? item) =>
        RestoreCoreAsync(item ?? SelectedBackup);

    private bool CanRestore() => !IsBusy && SelectedBackup is { CanRestore: true };

    partial void OnIsBusyChanged(bool value) =>
        RestoreCommand.NotifyCanExecuteChanged();

    partial void OnSelectedBackupChanged(BackupItemViewModel? value) =>
        RestoreCommand.NotifyCanExecuteChanged();

    private async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        StatusMessage = "Consultando los respaldos registrados...";

        try
        {
            await LoadCoreAsync();
            StatusMessage = Backups.Count == 0
                ? "Todavía no hay respaldos registrados. Cree uno desde el panel de control."
                : "Respaldos cargados y verificados con SHA-256.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudieron cargar los respaldos: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadCoreAsync()
    {
        var statuses = await _restoreManager.GetBackupsAsync();

        Backups.Clear();
        foreach (var status in statuses)
        {
            Backups.Add(new BackupItemViewModel(status));
        }

        var restorable = Backups.Count(backup => backup.CanRestore);
        SummaryText = Backups.Count == 0
            ? "Sin respaldos registrados."
            : $"{restorable} de {Backups.Count} respaldos listos para restaurar.";

        SelectedBackup = Backups.FirstOrDefault(backup => backup.CanRestore)
                         ?? Backups.FirstOrDefault();
    }

    private async Task RestoreCoreAsync(BackupItemViewModel? item)
    {
        if (item is null || IsBusy)
        {
            return;
        }

        if (!item.CanRestore)
        {
            _userDialogService.ShowError(
                "Respaldo no restaurable",
                "El archivo del respaldo no existe o su hash SHA-256 no coincide. " +
                "Por seguridad, SOK no restaura respaldos alterados.");
            return;
        }

        var confirmed = _userDialogService.Confirm(
            "Restaurar sistema",
            $"Se restaurará el respaldo «{item.Name}» del {item.DateText}.\n\n" +
            "SOK devolverá las claves de registro y los servicios a su estado anterior. " +
            "Los cambios de telemetría pueden requerir reiniciar Windows para surtir efecto.\n\n" +
            "¿Desea continuar?");

        if (!confirmed)
        {
            return;
        }

        IsBusy = true;
        StatusMessage = $"Restaurando «{item.Name}» (verificando SHA-256)...";

        try
        {
            var result = await _restoreManager.RestoreAsync(item.Id);

            await LoadCoreAsync();
            StatusMessage = result.Message;
            if (result.Failures.Count > 0)
            {
                StatusMessage += " " + string.Join(" ", result.Failures.Take(3));
            }

            if (result.Success)
            {
                _userDialogService.ShowInfo(
                    "Restauración completada",
                    $"{result.Message}\n\n" +
                    $"Claves de registro restauradas: {result.RegistryKeysRestored}\n" +
                    $"Servicios restaurados: {result.ServicesRestored}\n\n" +
                    "Si modificó telemetrías que requieren reinicio, reinicie Windows para completar el cambio.");
            }
            else
            {
                _userDialogService.ShowError("Restauración parcial", result.Message);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo restaurar el respaldo: {ex.Message}";
            _userDialogService.ShowError("Error al restaurar", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
