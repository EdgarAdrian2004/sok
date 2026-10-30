using System.Windows;

namespace SOK.UI.Navigation;

/// <summary>
/// Contrato de navegación entre los módulos de la aplicación.
/// </summary>
public interface INavigationService
{
    IReadOnlyList<NavigationItem> MenuItems { get; }

    FrameworkElement GetView(string key);
}
