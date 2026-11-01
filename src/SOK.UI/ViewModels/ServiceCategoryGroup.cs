using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SOK.Domain.Enums;

namespace SOK.UI.ViewModels;

/// <summary>
/// Agrupación de los servicios por categoría para mostrarlos en tarjetas
/// independientes.
/// </summary>
public sealed class ServiceCategoryGroup : ObservableObject
{
    public ServiceCategoryGroup(
        ServiceCategory category,
        IReadOnlyList<ServiceItemViewModel> items)
    {
        Category = category;
        Name = ServiceCategoryNames.GetName(category);
        Items = new ObservableCollection<ServiceItemViewModel>(items);
        RefreshSummary();
    }

    public ServiceCategory Category { get; }

    public string Name { get; }

    public ObservableCollection<ServiceItemViewModel> Items { get; }

    public string Summary { get; private set; } = string.Empty;

    public void RefreshSummary()
    {
        var disabled = Items.Count(item => item.IsDisabled);
        Summary = $"{disabled} de {Items.Count} deshabilitados";
        OnPropertyChanged(nameof(Summary));
    }
}
