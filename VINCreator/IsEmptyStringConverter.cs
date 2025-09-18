using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace VINCreator
{
    /// <summary>
    /// Converter that returns a specified parameter if the input string is not empty, otherwise empty string.
    /// </summary>
    public class IsEmptyStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts the value to the parameter if not empty, else empty string.
        /// </summary>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrEmpty(str))
            {
                return parameter ?? string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}