using System.Globalization;
using Avalonia.Data.Converters;

namespace KB.AvaloniaCore.Converters;

public class MatchValueConverter : IValueConverter
{
    /// <summary>
    ///     Returns true if the value matches the parameter
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch {
            null when parameter == null => true,
            null when parameter != null => false,
            _ => value.Equals(parameter),
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}