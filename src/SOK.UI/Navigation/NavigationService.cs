using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using SOK.UI.ViewModels;
using SOK.UI.Views;

namespace SOK.UI.Navigation;

/// <summary>
/// Navegación simple basada en un diccionario de vistas. Mantiene una
/// instancia por módulo para no recrear la vista en cada clic.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private readonly Dictionary<string, FrameworkElement> _views = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<NavigationItem> MenuItems { get; } = new List<NavigationItem>
    {
        new("Dashboard", "Panel de control", "Estado del sistema en tiempo real"),
        new("Telemetry", "Telemetría y privacidad", "Control de telemetrías de Windows"),
        new("Services", "Servicios", "Servicios no esenciales y perfiles"),
        new("Memory", "Memoria RAM", "Medición y liberación de memoria"),
        new("Backups", "Respaldos", "Respaldos y restauración"),
        new("Profiles", "Perfiles", "Perfiles de optimización"),
        new("Tutorial", "Tutorial", "Guía paso a paso")
    };

    public FrameworkElement GetView(string key)
    {
        if (_views.TryGetValue(key, out var cached))
        {
            return cached;
        }

        FrameworkElement view = key switch
        {
            "Dashboard" => new DashboardView
            {
                DataContext = Ioc.Default.GetRequiredService<DashboardViewModel>()
            },
            "Telemetry" => new TelemetryView(),
            "Services" => new ServicesView(),
            "Memory" => new MemoryView(),
            "Backups" => new BackupsView(),
            "Profiles" => new ProfilesView(),
            "Tutorial" => new TutorialView(),
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, "Módulo desconocido.")
        };

        _views[key] = view;
        return view;
    }
}
