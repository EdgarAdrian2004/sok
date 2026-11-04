using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SOK.UI.Converters;

/// <summary>
/// Convierte (orden del paso, índice actual) en el pincel de fondo del stepper.
/// </summary>
public sealed class StepStateToBrushConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || values[0] is not int order || values[1] is not int current)
        {
            return System.Windows.Application.Current?.FindResource("SurfaceSubtleBrush") as Brush
                   ?? Brushes.Transparent;
        }

        var index = order - 1;
        if (index < current)
        {
            return System.Windows.Application.Current?.FindResource("SuccessBrush") as Brush
                   ?? Brushes.Green;
        }
        if (index == current)
        {
            return System.Windows.Application.Current?.FindResource("AccentBrush") as Brush
                   ?? Brushes.Blue;
        }
        return System.Windows.Application.Current?.FindResource("SurfaceSubtleBrush") as Brush
               ?? Brushes.LightGray;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Convierte (orden del paso, índice actual) en el texto del indicador:
/// marca de verificación si está completado, número en caso contrario.
/// </summary>
public sealed class StepStateToTextConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || values[0] is not int order || values[1] is not int current)
        {
            return "?";
        }
        if (order - 1 < current)
        {
            return "\u2713";
        }
        return order.ToString("00", culture);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
