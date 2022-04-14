using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace BinaryStudio.PlatformUI
    {
    public class IsNullOrEmpty : IValueConverter
        {
        /// <summary>Converts a value.</summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        Object IValueConverter.Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
            if (value == null) { return true; }
            if (value is String) { return String.IsNullOrEmpty((String)value); }
            if (value is IList) { return ((IList)value).Count == 0; }
            return false;
            }

        /// <summary>Converts a value.</summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        Object IValueConverter.ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
            {
            throw new NotImplementedException();
            }
        }
    }