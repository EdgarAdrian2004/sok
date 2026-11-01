using CommunityToolkit.Mvvm.ComponentModel;
using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.UI.ViewModels;

/// <summary>
/// ViewModel de una fila del catálogo de telemetría. El interruptor
/// «Deshabilitar» notifica al ViewModel padre para que aplique el cambio
/// con respaldo previo.
/// </summary>
public partial class TelemetryItemViewModel : ObservableObject
{
    private readonly Func<TelemetryItemViewModel, bool, Task> _toggleHandler;
    private bool _suppressToggle;

    [ObservableProperty]
    private bool isDisabled;

    public TelemetryItemViewModel(
        TelemetryKey key,
        Func<TelemetryItemViewModel, bool, Task> toggleHandler)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        _toggleHandler = toggleHandler ?? throw new ArgumentNullException(nameof(toggleHandler));

        _suppressToggle = true;
        IsDisabled = key.Status == TelemetryStatus.Deshabilitado;
        _suppressToggle = false;
    }

    public TelemetryKey Key { get; }

    public string Id => Key.Id;

    public string DisplayName => Key.DisplayName;

    public string Description => Key.Description;

    public bool RequiresRestart => Key.RequiresRestart;

    public TelemetryStatus Status => Key.Status;

    public string StatusText =>
        Key.Status switch
        {
            TelemetryStatus.Habilitado => "Habilitada",
            TelemetryStatus.Deshabilitado => "Deshabilitada",
            TelemetryStatus.NoAplicable => "Sin valor en este equipo",
            _ => "Estado desconocido"
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
    public void ApplyState(TelemetryStatus status)
    {
        _suppressToggle = true;
        Key.Status = status;
        IsDisabled = status == TelemetryStatus.Deshabilitado;
        _suppressToggle = false;

        OnPropertyChanged(nameof(Status));
        OnPropertyChanged(nameof(StatusText));
    }

    /// <summary>
    /// Devuelve el interruptor al estado observado (se usa cuando la
    /// operación falla).
    /// </summary>
    public void RevertToggle()
    {
        _suppressToggle = true;
        IsDisabled = Key.Status == TelemetryStatus.Deshabilitado;
        _suppressToggle = false;
    }
}
