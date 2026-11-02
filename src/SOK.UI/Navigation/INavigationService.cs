using System.Windows;

namespace SOK.UI.Navigation;

/// <summary>
/// Contrato de navegación entre los módulos de la aplicación.
/// </summary>
public interface INavigationService
{
    IReadOnlyList<NavigationItem> MenuItems { get; }

    /// <summary>
    /// Se produce cuando un ViewModel solicita navegar a otro módulo. La
    /// ventana principal escucha este evento para actualizar el menú.
    /// </summary>
    event EventHandler<string>? NavigationRequested;

    FrameworkElement GetView(string key);

    /// <summary>Solicita la navegación al módulo identificado por <paramref name="key"/>.</summary>
    void RequestNavigation(string key);
}
