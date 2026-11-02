using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SOK.Application.Abstractions;
using SOK.Application.Models;
using SOK.Domain.Abstractions;
using SOK.UI.Navigation;

namespace SOK.UI.ViewModels;

/// <summary>
/// ViewModel del tutorial integrado: carga los pasos en Markdown, controla
/// la navegación entre ellos, abre el módulo relacionado y guarda la marca
/// de tutorial completado en la configuración.
/// </summary>
public partial class TutorialViewModel : ObservableObject
{
    /// <summary>Clave de configuración que evita repetir el tutorial al iniciar.</summary>
    public const string TutorialCompletedKey = "TutorialCompleted";

    private readonly ITutorialService _tutorialService;
    private readonly ISettingsRepository _settingsRepository;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private int currentIndex;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string statusMessage = "Cargando la guía paso a paso...";

    public TutorialViewModel(
        ITutorialService tutorialService,
        ISettingsRepository settingsRepository,
        INavigationService navigationService)
    {
        _tutorialService = tutorialService ?? throw new ArgumentNullException(nameof(tutorialService));
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        InitializationTask = LoadAsync();
    }

    /// <summary>Tarea de carga inicial; se expone para las pruebas.</summary>
    internal Task InitializationTask { get; }

    public ObservableCollection<TutorialStep> Steps { get; } = new();

    public TutorialStep? CurrentStep =>
        CurrentIndex >= 0 && CurrentIndex < Steps.Count
            ? Steps[CurrentIndex]
            : null;

    public string ProgressText =>
        Steps.Count == 0
            ? "Sin pasos disponibles"
            : $"Paso {CurrentIndex + 1} de {Steps.Count}";

    public bool CanGoPrevious => !IsBusy && CurrentIndex > 0;

    public bool CanGoNext => !IsBusy && CurrentIndex >= 0 && CurrentIndex < Steps.Count - 1;

    public bool IsLastStep => Steps.Count > 0 && CurrentIndex == Steps.Count - 1;

    public bool IsNotLastStep => !IsLastStep;

    public bool HasModule => !string.IsNullOrWhiteSpace(CurrentStep?.ModuleKey);

    public string ModuleButtonText =>
        CurrentStep?.ModuleKey switch
        {
            "Dashboard" => "Ir al panel de control",
            "Telemetry" => "Ir a Telemetría",
            "Services" => "Ir a Servicios",
            "Memory" => "Ir a Memoria",
            "Backups" => "Ir a Respaldos",
            "Profiles" => "Ir a Perfiles",
            _ => "Ir al módulo"
        };

    [RelayCommand(CanExecute = nameof(CanGoPrevious))]
    private void Previous() => CurrentIndex--;

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private void Next() => CurrentIndex++;

    [RelayCommand(CanExecute = nameof(HasModule))]
    private void GoToModule()
    {
        if (CurrentStep?.ModuleKey is { Length: > 0 } key)
        {
            _navigationService.RequestNavigation(key);
        }
    }

    [RelayCommand]
    private Task SkipAsync() =>
        FinishAsync("Tutorial omitido. Puede volver a abrirlo desde el menú lateral.");

    [RelayCommand]
    private Task CompleteAsync() =>
        FinishAsync("Tutorial completado. ¡Gracias por usar SOK!");

    partial void OnCurrentIndexChanged(int value)
    {
        RaiseStepProperties();
        NotifyCommands();
    }

    partial void OnIsBusyChanged(bool value)
    {
        RaiseStepProperties();
        NotifyCommands();
    }

    private async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        StatusMessage = "Cargando la guía paso a paso...";

        try
        {
            var steps = await _tutorialService.GetStepsAsync();

            Steps.Clear();
            foreach (var step in steps)
            {
                Steps.Add(step);
            }

            CurrentIndex = Steps.Count > 0 ? 0 : -1;
            StatusMessage = Steps.Count == 0
                ? "No se encontraron los pasos del tutorial."
                : "Recorra la guía con los botones Anterior y Siguiente.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo cargar el tutorial: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            RaiseStepProperties();
            NotifyCommands();
        }
    }

    private async Task FinishAsync(string message)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;

        try
        {
            await _settingsRepository.SetAsync(TutorialCompletedKey, "true");
            StatusMessage = message;
            _navigationService.RequestNavigation("Dashboard");
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo guardar el estado del tutorial: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void RaiseStepProperties()
    {
        OnPropertyChanged(nameof(CurrentStep));
        OnPropertyChanged(nameof(ProgressText));
        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));
        OnPropertyChanged(nameof(IsLastStep));
        OnPropertyChanged(nameof(IsNotLastStep));
        OnPropertyChanged(nameof(HasModule));
        OnPropertyChanged(nameof(ModuleButtonText));
    }

    private void NotifyCommands()
    {
        PreviousCommand.NotifyCanExecuteChanged();
        NextCommand.NotifyCanExecuteChanged();
        GoToModuleCommand.NotifyCanExecuteChanged();
    }
}
