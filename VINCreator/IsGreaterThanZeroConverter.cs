using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace VINCreator
{
    /// <summary>
    /// Converter that checks if an integer value is greater than zero.
    /// </summary>
    public class IsGreaterThanZeroConverter : IValueConverter
    {
        /// <summary>
        /// Returns true if the value is an int greater than 0.
        /// </summary>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is int count && count > 0;
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