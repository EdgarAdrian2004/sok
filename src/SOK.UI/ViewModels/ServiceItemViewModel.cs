using CommunityToolkit.Mvvm.ComponentModel;
using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.UI.ViewModels;

/// <summary>
/// ViewModel de una fila del catálogo de servicios. El interruptor
/// «Deshabilitar» notifica al ViewModel padre para que aplique el cambio
/// con respaldo previo.
/// </summary>
public partial class ServiceItemViewModel : ObservableObject
{
    private readonly Func<ServiceItemViewModel, bool, Task> _toggleHandler;
    private bool _suppressToggle;

    [ObservableProperty]
    private bool isDisabled;

    public ServiceItemViewModel(
        ServiceEntry entry,
        Func<ServiceItemViewModel, bool, Task> toggleHandler)
    {
        Entry = entry ?? throw new ArgumentNullException(nameof(entry));
        _toggleHandler = toggleHandler ?? throw new ArgumentNullException(nameof(toggleHandler));

        _suppressToggle = true;
        IsDisabled = entry.Status == ServiceStatus.Deshabilitado;
        _suppressToggle = false;
    }

    public ServiceEntry Entry { get; }

    public string Id => Entry.Id;

    public string DisplayName => Entry.DisplayName;

    public string Description => Entry.Description;

    public ServiceCategory Category => Entry.Category;

    public bool HighRisk => Entry.HighRisk;

    public bool RequiresRestart => Entry.RequiresRestart;

    public bool CanToggle => Entry.Exists;

    public ServiceStatus Status => Entry.Status;

    public string StatusText =>
        Entry.Status switch
        {
            ServiceStatus.Habilitado => "Habilitado",
            ServiceStatus.Deshabilitado => "Deshabilitado",
            ServiceStatus.NoAplicable => "No instalado en este equipo",
            _ => "Estado desconocido"
        };

    public string StartModeText =>
        Entry.StartMode switch
        {
            ServiceStartMode.Automatico => "Inicio automático",
            ServiceStartMode.Manual => "Inicio manual",
            ServiceStartMode.Deshabilitado => "Inicio deshabilitado",
            _ => "Inicio desconocido"
        };

    public string StateText =>
        Entry.State switch
        {
            ServiceState.EnEjecucion => "En ejecución",
            ServiceState.Detenido => "Detenido",
            ServiceState.Pausado => "Pausado",
            ServiceState.Iniciando => "Iniciando",
            ServiceState.Deteniendo => "Deteniendo",
            _ => "Sin datos"
        };

    partial void OnIsDisabledChanged(bool value)
    {
        if (_suppressToggle)
        {
            return;
        }

        _ = _toggleHandler(this, value);
    }

    /// <summary>
    /// Actualiza el estado observado y el interruptor sin disparar el
    /// manejador de cambios.
    /// </summary>
    public void ApplyState(bool disabled)
    {
        _suppressToggle = true;

        Entry.StartMode = disabled ? ServiceStartMode.Deshabilitado : Entry.DefaultStartMode;
        if (disabled)
        {
            Entry.State = ServiceState.Detenido;
        }

        IsDisabled = disabled;
        _suppressToggle = false;

        RaiseStateChanged();
    }

    /// <summary>
    /// Devuelve el interruptor al estado observado (se usa cuando la
    /// operación falla).
    /// </summary>
    public void RevertToggle()
    {
        _suppressToggle = true;
        IsDisabled = Entry.Status == ServiceStatus.Deshabilitado;
        _suppressToggle = false;
    }

    private void RaiseStateChanged()
    {
        OnPropertyChanged(nameof(Status));
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(StartModeText));
        OnPropertyChanged(nameof(StateText));
        OnPropertyChanged(nameof(CanToggle));
    }
}
