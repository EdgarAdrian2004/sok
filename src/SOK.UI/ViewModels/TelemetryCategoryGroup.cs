using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SOK.Domain.Enums;

namespace SOK.UI.ViewModels;

/// <summary>
/// Agrupación de las claves de telemetría por categoría para mostrarlas
/// en tarjetas independientes.
/// </summary>
public sealed class TelemetryCategoryGroup : ObservableObject
{
    public TelemetryCategoryGroup(
        TelemetryCategory category,
        IReadOnlyList<TelemetryItemViewModel> items)
    {
        Category = category;
        Name = TelemetryCategoryNames.GetName(category);
        Items = new ObservableCollection<TelemetryItemViewModel>(items);
        RefreshSummary();
    }

    public TelemetryCategory Category { get; }

    public string Name { get; }

    public ObservableCollection<TelemetryItemViewModel> Items { get; }

    public string Summary { get; private set; } = string.Empty;

    public void RefreshSummary()
    {
        var disabled = Items.Count(item => item.IsDisabled);
        Summary = $"{disabled} de {Items.Count} deshabilitadas";
        OnPropertyChanged(nameof(Summary));
    }
}
