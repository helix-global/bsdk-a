using System;
using System.Globalization;
using System.Windows.Data;

namespace BinaryStudio.PlatformUI.Converters
    {
    using EDateTimeConverter = System.ComponentModel.DateTimeConverter;
    public class DateTimeConverter : EDateTimeConverter,IValueConverter
        {
        /// <summary>Converts a value.</summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
            if (value is DateTime datetime) {
                if (targetType == typeof(String)) {
                    var formatspec = parameter as String;
                    return String.IsNullOrWhiteSpace(formatspec)
                        ? ConvertTo(null, culture, value, targetType)
                        : datetime.ToString(formatspec);
                    }
                }
            return value;
            }

        /// <summary>Converts a value.</summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
            if (value is String stringvalue) {
                if (targetType == typeof(DateTime)) {
                    var formatspec = parameter as String;
                    return String.IsNullOrWhiteSpace(formatspec)
                        ? ConvertFrom(null, culture, value)
                        : DateTime.ParseExact(stringvalue, formatspec, culture);
                    }
                }
            return value;
            }
        }
    }