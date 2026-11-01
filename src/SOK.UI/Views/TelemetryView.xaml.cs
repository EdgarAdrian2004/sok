using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SOK.UI.Views;

public partial class TelemetryView : UserControl
{
    public TelemetryView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <summary>
    /// Garantiza que la vista se muestre desde el principio cada vez que el
    /// usuario entra al módulo, incluso si WPF intentó llevar un elemento a
    /// la vista al crear el contenido.
    /// </summary>
    private void OnLoaded(object sender, RoutedEventArgs e) =>
        Dispatcher.BeginInvoke(
            new Action(() => ContentScroll.ScrollToTop()),
            DispatcherPriority.Loaded);
}
