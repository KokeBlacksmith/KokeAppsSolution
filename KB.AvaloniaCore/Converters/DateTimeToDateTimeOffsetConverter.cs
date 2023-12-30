using Avalonia.Data.Converters;
using System.Globalization;

namespace KB.AvaloniaCore.Converters;

public class DateTimeToDateTimeOffsetConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        DateTime dateTime = DateTime.MinValue;
        if (value is DateTime dateTimeValue)
        {
            dateTime = dateTimeValue;
        }
        else if (value is DateTime?)
        {
            DateTime? nullableDateTime = value as DateTime?;
            return nullableDateTime.HasValue ? dateTime = nullableDateTime.Value : DateTime.MinValue;
        }

        // DateTimeOffset does not accept DatTime.MinValue
        if (dateTime != DateTime.MinValue)
        {
            return new DateTimeOffset(dateTime);
        }
        else
        {
            return value;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.DateTime;
        }
        else if (value is DateTimeOffset?)
        {
            DateTimeOffset? nullableDateTimeOffset = value as DateTimeOffset?;
            return nullableDateTimeOffset.HasValue ? nullableDateTimeOffset.Value.DateTime : value;
        }

        return value;
    }
}
