using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SOK.Application.Abstractions;
using SOK.Domain.Enums;

namespace SOK.UI.ViewModels;

/// <summary>
/// ViewModel del módulo de telemetría y privacidad. Carga el catálogo,
/// agrupa las claves por categoría y aplica cambios individuales o la
/// privacidad máxima, siempre con respaldo previo.
/// </summary>
public partial class TelemetryViewModel : ObservableObject
{
    private const string AllCategories = "Todas";

    private readonly ITelemetryManager _telemetryManager;
    private readonly List<TelemetryItemViewModel> _allItems = new();

    [ObservableProperty]
    private string? selectedCategory = AllCategories;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string statusMessage = "Consultando el estado del equipo...";

    [ObservableProperty]
    private string summaryText = string.Empty;

    [ObservableProperty]
    private string lastBackupText = string.Empty;

    public TelemetryViewModel(ITelemetryManager telemetryManager)
    {
        _telemetryManager = telemetryManager ?? throw new ArgumentNullException(nameof(telemetryManager));

        Categories.Add(AllCategories);
        InitializationTask = LoadAsync();
    }

    /// <summary>Tarea de carga inicial; se expone para las pruebas.</summary>
    internal Task InitializationTask { get; }

    public ObservableCollection<TelemetryCategoryGroup> Groups { get; } = new();

    public ObservableCollection<string> Categories { get; } = new();

    [RelayCommand]
    private Task RefreshAsync() => LoadAsync();

    [RelayCommand(CanExecute = nameof(CanApplyMaxPrivacy))]
    private async Task ApplyMaxPrivacyAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        StatusMessage = "Aplicando privacidad máxima (se creará un respaldo)...";

        try
        {
            var result = await _telemetryManager.ApplyMaxPrivacyAsync();

            if (!string.IsNullOrWhiteSpace(result.BackupFilePath))
            {
                LastBackupText = $"Respaldo: {result.BackupFilePath}";
            }

            await LoadCoreAsync();

            StatusMessage = result.Message;
            if (result.Failures.Count > 0)
            {
                StatusMessage += " " + string.Join(" ", result.Failures.Take(3));
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error al aplicar la privacidad máxima: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanApplyMaxPrivacy() => !IsBusy && _allItems.Count > 0;

    partial void OnSelectedCategoryChanged(string? value) => RebuildGroups();

    partial void OnIsBusyChanged(bool value) => ApplyMaxPrivacyCommand.NotifyCanExecuteChanged();

    private async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        StatusMessage = "Consultando el estado del equipo...";

        try
        {
            await LoadCoreAsync();
            StatusMessage = "Catálogo de telemetría cargado correctamente.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo cargar el catálogo de telemetría: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadCoreAsync()
    {
        var catalog = await _telemetryManager.GetCatalogAsync();

        _allItems.Clear();
        foreach (var key in catalog)
        {
            _allItems.Add(new TelemetryItemViewModel(key, OnItemToggleAsync));
        }

        Categories.Clear();
        Categories.Add(AllCategories);
        foreach (var category in catalog
                     .Select(key => key.Category)
                     .Distinct()
                     .OrderBy(category => TelemetryCategoryNames.GetName(category)))
        {
            Categories.Add(TelemetryCategoryNames.GetName(category));
        }

        if (string.IsNullOrWhiteSpace(SelectedCategory) ||
            !Categories.Contains(SelectedCategory))
        {
            SelectedCategory = AllCategories;
        }

        RebuildGroups();
        UpdateSummary();
    }

    private void RebuildGroups()
    {
        Groups.Clear();

        var filter = string.IsNullOrWhiteSpace(SelectedCategory) ||
                     string.Equals(SelectedCategory, AllCategories, StringComparison.Ordinal)
            ? null
            : SelectedCategory;

        var groups = _allItems
            .Where(item => filter is null ||
                           string.Equals(
                               TelemetryCategoryNames.GetName(item.Key.Category),
                               filter,
                               StringComparison.Ordinal))
            .GroupBy(item => item.Key.Category)
            .OrderBy(group => TelemetryCategoryNames.GetName(group.Key));

        foreach (var group in groups)
        {
            Groups.Add(new TelemetryCategoryGroup(group.Key, group.ToList()));
        }
    }

    internal async Task OnItemToggleAsync(TelemetryItemViewModel item, bool disabled)
    {
        if (IsBusy)
        {
            item.RevertToggle();
            return;
        }

        IsBusy = true;
        StatusMessage = disabled
            ? $"Deshabilitando «{item.DisplayName}»..."
            : $"Habilitando «{item.DisplayName}»...";

        try
        {
            var result = await _telemetryManager.SetDisabledAsync(item.Id, disabled);

            if (result.Success)
            {
                item.ApplyState(disabled ? TelemetryStatus.Deshabilitado : TelemetryStatus.Habilitado);
                StatusMessage = result.Message;

                if (!string.IsNullOrWhiteSpace(result.BackupFilePath))
                {
                    LastBackupText = $"Respaldo: {result.BackupFilePath}";
                }
            }
            else
            {
                item.RevertToggle();
                StatusMessage = result.Message;
            }
        }
        catch (Exception ex)
        {
            item.RevertToggle();
            StatusMessage = $"No se pudo cambiar «{item.DisplayName}»: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            UpdateSummary();
            RefreshGroupSummaries();
        }
    }

    private void UpdateSummary()
    {
        var total = _allItems.Count;
        var disabled = _allItems.Count(item => item.IsDisabled);
        SummaryText = total == 0
            ? "Sin claves de telemetría disponibles."
            : $"{disabled} de {total} claves deshabilitadas.";
    }

    private void RefreshGroupSummaries()
    {
        foreach (var group in Groups)
        {
            group.RefreshSummary();
        }
    }
}
