using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using SOK.Domain.Enums;

namespace SOK.UI.Converters;

/// <summary>
/// Convierte un ProfileType en la geometría del icono correspondiente.
/// </summary>
public sealed class ProfileTypeToGeometryConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string key = value is ProfileType type
            ? type switch
            {
                ProfileType.Gamer => "IconGamepad",
                ProfileType.Ofimatico => "IconBriefcase",
                _ => "IconUser",
            }
            : "IconUser";

        try
        {
            return System.Windows.Application.Current?.FindResource(key) as Geometry;
        }
        catch
        {
            return null;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
