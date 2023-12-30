using Avalonia.Data.Converters;
using System.Globalization;

namespace KB.AvaloniaCore.Converters;

public class DateTimeToDateTimeOffsetConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }

        DateTime result = DateTime.MinValue;
        if (value is DateTime dateTimeValue)
        {
            result = dateTimeValue;
        }
        else if (value is DateTime?)
        {
            DateTime? nullableDateTime = value as DateTime?;
            result = nullableDateTime.HasValue ? nullableDateTime.Value : DateTime.MinValue;
        }
        else
        {
            throw new ArgumentException($"Value is not a {nameof(DateTime)} or a {nameof(DateTime)}?");
        }

        // DateTimeOffset does not accept DatTime.MinValue
        if (result != DateTime.MinValue)
        {
            return new DateTimeOffset(result);
        }
        else
        {
            return null;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }

        if (value is DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.DateTime;
        }
        else if (value is DateTimeOffset?)
        {
            DateTimeOffset? nullableDateTimeOffset = (DateTimeOffset?)value;
            return nullableDateTimeOffset.HasValue ? nullableDateTimeOffset.Value.DateTime : null;
        }
        else
        {
            throw new ArgumentException($"Value is not a {nameof(DateTimeOffset)} or a {nameof(DateTimeOffset)}?");
        }
    }
}
