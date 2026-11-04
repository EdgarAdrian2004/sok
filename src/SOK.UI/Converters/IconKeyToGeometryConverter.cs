using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SOK.UI.Converters;

/// <summary>
/// Convierte la clave de un icono (por ejemplo "IconDashboard") en la
/// geometría correspondiente definida en Styles/Icons.xaml.
/// </summary>
public sealed class IconKeyToGeometryConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string key && !string.IsNullOrWhiteSpace(key))
        {
            try
            {
                return System.Windows.Application.Current?.FindResource(key) as Geometry;
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
