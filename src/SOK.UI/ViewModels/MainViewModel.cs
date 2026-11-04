using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SOK.UI.Navigation;
using SOK.UI.Services;

/// <summary>
/// ViewModel de la ventana principal: expone el menú y la vista activa.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly ThemeService _themeService;
    private NavigationItem? _selectedNavigationItem;
    private object? _currentView;
    private string _currentTitle = string.Empty;
    private string _currentSubtitle = string.Empty;

    public MainViewModel(INavigationService navigationService, ThemeService themeService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _navigationService.NavigationRequested += OnNavigationRequested;
        MenuItems = navigationService.MenuItems;
        SelectedNavigationItem = MenuItems.FirstOrDefault();
    }

    public ThemeService Theme => _themeService;

    [RelayCommand]
    private void ToggleTheme() => _themeService.Toggle();

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

    private void OnNavigationRequested(object? sender, string key)
    {
        var item = MenuItems.FirstOrDefault(
            candidate => string.Equals(candidate.Key, key, StringComparison.OrdinalIgnoreCase));

        if (item is not null)
        {
            SelectedNavigationItem = item;
        }
    }
}
