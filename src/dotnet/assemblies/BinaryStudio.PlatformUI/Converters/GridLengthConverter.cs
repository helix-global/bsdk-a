﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BinaryStudio.PlatformUI.Converters
    {
    public class GridLengthConverter : IValueConverter
        {
        /// <summary>Converts a value.</summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
            {
            if (value == null) { return null; }
            var type = value.GetType();
            if (type == targetType) { return value; }
            if (targetType == typeof(GridLength)) {
                if (type == typeof(Double)) {
                    return new GridLength((Double)value);
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
        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
            {
            throw new NotImplementedException();
            }
        }
    }