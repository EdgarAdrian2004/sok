using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SOK.Application.Abstractions;
using SOK.Domain.Enums;

namespace SOK.UI.ViewModels;

/// <summary>
/// ViewModel del módulo de gestión de servicios. Carga el catálogo, lo
/// agrupa por categoría y aplica cambios individuales o perfiles completos,
/// siempre con respaldo previo.
/// </summary>
public partial class ServicesViewModel : ObservableObject
{
    private const string AllCategories = "Todas";

    private readonly IServiceManager _serviceManager;
    private readonly List<ServiceItemViewModel> _allItems = new();

    [ObservableProperty]
    private string? selectedCategory = AllCategories;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string statusMessage = "Consultando los servicios del equipo...";

    [ObservableProperty]
    private string summaryText = string.Empty;

    [ObservableProperty]
    private string lastBackupText = string.Empty;

    public ServicesViewModel(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));

        Categories.Add(AllCategories);
        InitializationTask = LoadAsync();
    }

    /// <summary>Tarea de carga inicial; se expone para las pruebas.</summary>
    internal Task InitializationTask { get; }

    public ObservableCollection<ServiceCategoryGroup> Groups { get; } = new();

    public ObservableCollection<string> Categories { get; } = new();

    [RelayCommand]
    private Task RefreshAsync() => LoadAsync();

    [RelayCommand(CanExecute = nameof(CanApplyProfile))]
    private Task ApplyGamerProfileAsync() => ApplyProfileAsync(ServiceProfile.Gamer);

    [RelayCommand(CanExecute = nameof(CanApplyProfile))]
    private Task ApplyOfimaticoProfileAsync() => ApplyProfileAsync(ServiceProfile.Ofimatico);

    private bool CanApplyProfile() => !IsBusy && _allItems.Count > 0;

    partial void OnSelectedCategoryChanged(string? value) => RebuildGroups();

    partial void OnIsBusyChanged(bool value)
    {
        ApplyGamerProfileCommand.NotifyCanExecuteChanged();
        ApplyOfimaticoProfileCommand.NotifyCanExecuteChanged();
    }

    private async Task ApplyProfileAsync(ServiceProfile profile)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        var profileName = profile == ServiceProfile.Gamer ? "Gamer" : "Ofimático";
        StatusMessage = $"Aplicando el perfil {profileName} (se creará un respaldo)...";

        try
        {
            var result = await _serviceManager.ApplyProfileAsync(profile);

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
            StatusMessage = $"Error al aplicar el perfil {profileName}: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        StatusMessage = "Consultando los servicios del equipo...";

        try
        {
            await LoadCoreAsync();
            StatusMessage = "Catálogo de servicios cargado correctamente.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo cargar el catálogo de servicios: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadCoreAsync()
    {
        var catalog = await _serviceManager.GetCatalogAsync();

        _allItems.Clear();
        foreach (var entry in catalog)
        {
            _allItems.Add(new ServiceItemViewModel(entry, OnItemToggleAsync));
        }

        Categories.Clear();
        Categories.Add(AllCategories);
        foreach (var category in catalog
                     .Select(entry => entry.Category)
                     .Distinct()
                     .OrderBy(ServiceCategoryNames.GetName))
        {
            Categories.Add(ServiceCategoryNames.GetName(category));
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
                               ServiceCategoryNames.GetName(item.Category),
                               filter,
                               StringComparison.Ordinal))
            .GroupBy(item => item.Category)
            .OrderBy(group => ServiceCategoryNames.GetName(group.Key));

        foreach (var group in groups)
        {
            Groups.Add(new ServiceCategoryGroup(group.Key, group.ToList()));
        }
    }

    internal async Task OnItemToggleAsync(ServiceItemViewModel item, bool disabled)
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
            var result = await _serviceManager.SetDisabledAsync(item.Id, disabled);

            if (result.Success)
            {
                item.ApplyState(disabled);
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
        var applicable = _allItems.Where(item => item.Entry.Exists).ToList();
        var disabled = applicable.Count(item => item.IsDisabled);
        SummaryText = applicable.Count == 0
            ? "Sin servicios del catálogo disponibles en este equipo."
            : $"{disabled} de {applicable.Count} servicios deshabilitados.";
    }

    private void RefreshGroupSummaries()
    {
        foreach (var group in Groups)
        {
            group.RefreshSummary();
        }
    }
}
