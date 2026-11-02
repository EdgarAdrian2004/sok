using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SOK.Application.Abstractions;
using SOK.Application.Common;
using SOK.Domain.Entities;
using SOK.UI.Services;

namespace SOK.UI.ViewModels;

/// <summary>
/// ViewModel del módulo de perfiles: lista los perfiles disponibles, marca
/// el activo, aplica sus ajustes con respaldo previo y permite exportarlos
/// o importarlos en JSON.
/// </summary>
public partial class ProfilesViewModel : ObservableObject
{
    private readonly IProfileManager _profileManager;
    private readonly IFileDialogService _fileDialogService;
    private readonly IUserDialogService _userDialogService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string statusMessage = "Consultando los perfiles disponibles...";

    [ObservableProperty]
    private string activeProfileText = "Sin perfil activo";

    [ObservableProperty]
    private string lastBackupText = string.Empty;

    [ObservableProperty]
    private ProfileItemViewModel? selectedProfile;

    public ProfilesViewModel(
        IProfileManager profileManager,
        IFileDialogService fileDialogService,
        IUserDialogService userDialogService)
    {
        _profileManager = profileManager ?? throw new ArgumentNullException(nameof(profileManager));
        _fileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
        _userDialogService = userDialogService ?? throw new ArgumentNullException(nameof(userDialogService));

        InitializationTask = LoadAsync();
    }

    /// <summary>Tarea de carga inicial; se expone para las pruebas.</summary>
    internal Task InitializationTask { get; }

    public ObservableCollection<ProfileItemViewModel> Profiles { get; } = new();

    [RelayCommand]
    private Task RefreshAsync() => LoadAsync();

    [RelayCommand(CanExecute = nameof(CanOperate))]
    private Task ActivateProfileAsync(ProfileItemViewModel? item) =>
        ActivateCoreAsync(item ?? SelectedProfile);

    [RelayCommand(CanExecute = nameof(CanOperate))]
    private Task ApplyProfileAsync(ProfileItemViewModel? item) =>
        ApplyCoreAsync(item ?? SelectedProfile);

    [RelayCommand(CanExecute = nameof(CanOperate))]
    private Task ExportProfileAsync(ProfileItemViewModel? item) =>
        ExportCoreAsync(item ?? SelectedProfile);

    [RelayCommand(CanExecute = nameof(CanImport))]
    private Task ImportProfileAsync() => ImportCoreAsync();

    private bool CanOperate() => !IsBusy && SelectedProfile is not null;

    private bool CanImport() => !IsBusy;

    partial void OnIsBusyChanged(bool value)
    {
        ActivateProfileCommand.NotifyCanExecuteChanged();
        ApplyProfileCommand.NotifyCanExecuteChanged();
        ExportProfileCommand.NotifyCanExecuteChanged();
        ImportProfileCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedProfileChanged(ProfileItemViewModel? value)
    {
        ActivateProfileCommand.NotifyCanExecuteChanged();
        ApplyProfileCommand.NotifyCanExecuteChanged();
        ExportProfileCommand.NotifyCanExecuteChanged();
    }

    private async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        StatusMessage = "Consultando los perfiles disponibles...";

        try
        {
            await LoadCoreAsync();
            StatusMessage = Profiles.Count == 0
                ? "No hay perfiles registrados. Importe un perfil en JSON para comenzar."
                : "Perfiles cargados correctamente.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudieron cargar los perfiles: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadCoreAsync()
    {
        var profiles = await _profileManager.GetAllProfilesAsync();
        var active = await _profileManager.GetActiveProfileAsync();

        Profiles.Clear();
        foreach (var profile in profiles)
        {
            Profiles.Add(new ProfileItemViewModel(profile, active?.Id == profile.Id));
        }

        ActiveProfileText = active is null
            ? "Sin perfil activo"
            : $"Perfil activo: «{active.Name}» ({ActiveTypeText(active.Type)})";

        SelectedProfile = Profiles.FirstOrDefault(item => item.IsActive)
                          ?? Profiles.FirstOrDefault();
    }

    private async Task ActivateCoreAsync(ProfileItemViewModel? item)
    {
        if (item is null || IsBusy)
        {
            return;
        }

        IsBusy = true;
        StatusMessage = $"Activando el perfil «{item.Name}»...";

        try
        {
            await _profileManager.SetActiveProfileAsync(item.Id);
            await LoadCoreAsync();
            StatusMessage = $"El perfil «{item.Name}» quedó activo para esta sesión.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo activar el perfil: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ApplyCoreAsync(ProfileItemViewModel? item)
    {
        if (item is null || IsBusy)
        {
            return;
        }

        var confirmed = _userDialogService.Confirm(
            "Aplicar perfil",
            $"Se aplicarán los ajustes del perfil «{item.Name}».\n\n" +
            "SOK creará un respaldo de las claves de registro y de los servicios " +
            "afectados antes de modificarlos.\n\n¿Desea continuar?");

        if (!confirmed)
        {
            return;
        }

        IsBusy = true;
        StatusMessage = $"Aplicando el perfil «{item.Name}» (se crearán respaldos)...";
        LastBackupText = string.Empty;

        try
        {
            var result = await _profileManager.ApplyProfileAsync(item.Id);

            if (result.BackupFilePaths.Count > 0)
            {
                LastBackupText = "Respaldos: " + string.Join(" · ", result.BackupFilePaths);
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
            StatusMessage = $"No se pudo aplicar el perfil: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ExportCoreAsync(ProfileItemViewModel? item)
    {
        if (item is null || IsBusy)
        {
            return;
        }

        var path = _fileDialogService.ShowSaveFileDialog(
            "Exportar perfil",
            "Perfiles SOK (*.json)|*.json|Todos los archivos (*.*)|*.*",
            $"{SanitizeFileName(item.Name)}.json");

        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        IsBusy = true;
        StatusMessage = $"Exportando el perfil «{item.Name}»...";

        try
        {
            await _profileManager.ExportProfileAsync(item.Id, path);
            StatusMessage = $"Perfil exportado en {path}.";
            _userDialogService.ShowInfo("Perfil exportado", $"El perfil se guardó en:\n{path}");
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo exportar el perfil: {ex.Message}";
            _userDialogService.ShowError("Error al exportar", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ImportCoreAsync()
    {
        if (IsBusy)
        {
            return;
        }

        var path = _fileDialogService.ShowOpenFileDialog(
            "Importar perfil",
            "Perfiles SOK (*.json)|*.json|Todos los archivos (*.*)|*.*");

        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        IsBusy = true;
        StatusMessage = "Importando el perfil...";

        try
        {
            var imported = await _profileManager.ImportProfileAsync(path);
            await LoadCoreAsync();
            SelectedProfile = Profiles.FirstOrDefault(item => item.Id == imported.Id);
            StatusMessage = $"Perfil «{imported.Name}» importado correctamente.";
            _userDialogService.ShowInfo(
                "Perfil importado",
                $"Se incorporó el perfil «{imported.Name}» como perfil personalizado.");
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo importar el perfil: {ex.Message}";
            _userDialogService.ShowError("Error al importar", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static string ActiveTypeText(SOK.Domain.Enums.ProfileType type) =>
        type switch
        {
            SOK.Domain.Enums.ProfileType.Gamer => "Gamer",
            SOK.Domain.Enums.ProfileType.Ofimatico => "Ofimático",
            _ => "Personalizado"
        };

    private static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var chars = name
            .Select(character => invalid.Contains(character) || character == ' ' ? '_' : character)
            .ToArray();
        return new string(chars);
    }
}
