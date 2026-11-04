using System.Globalization;
using System.Windows.Data;

namespace SOK.UI.Converters;

/// <summary>
/// Convierte un índice base cero en un valor de progreso base uno.
/// </summary>
public sealed class IndexToProgressConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is int i ? i + 1 : 0;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
