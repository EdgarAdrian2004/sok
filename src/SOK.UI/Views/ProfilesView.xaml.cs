using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SOK.UI.Views;

public partial class ProfilesView : UserControl
{
    public ProfilesView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <summary>
    /// Garantiza que la vista se muestre desde el principio cada vez que el
    /// usuario entra al módulo.
    /// </summary>
    private void OnLoaded(object sender, RoutedEventArgs e) =>
        Dispatcher.BeginInvoke(
            new Action(() => ContentScroll.ScrollToTop()),
            DispatcherPriority.Loaded);
}
