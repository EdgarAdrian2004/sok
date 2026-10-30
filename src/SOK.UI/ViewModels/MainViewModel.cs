using CommunityToolkit.Mvvm.ComponentModel;
using SOK.UI.Navigation;

namespace SOK.UI.ViewModels;

/// <summary>
/// ViewModel de la ventana principal: expone el menú y la vista activa.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private NavigationItem? _selectedNavigationItem;
    private object? _currentView;
    private string _currentTitle = string.Empty;
    private string _currentSubtitle = string.Empty;

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        MenuItems = navigationService.MenuItems;
        SelectedNavigationItem = MenuItems.FirstOrDefault();
    }

    public IReadOnlyList<NavigationItem> MenuItems { get; }

    public NavigationItem? SelectedNavigationItem
    {
        get => _selectedNavigationItem;
        set
        {
            if (SetProperty(ref _selectedNavigationItem, value) && value is not null)
            {
                Navigate(value);
            }
        }
    }

    public object? CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
    }

    public string CurrentTitle
    {
        get => _currentTitle;
        private set => SetProperty(ref _currentTitle, value);
    }

    public string CurrentSubtitle
    {
        get => _currentSubtitle;
        private set => SetProperty(ref _currentSubtitle, value);
    }

    private void Navigate(NavigationItem item)
    {
        CurrentView = _navigationService.GetView(item.Key);
        CurrentTitle = item.Label;
        CurrentSubtitle = item.Description;
    }
}
