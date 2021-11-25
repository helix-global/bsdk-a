using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace BinaryStudio.PlatformUI
    {
    public class ToString : IValueConverter
        {
        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        Object IValueConverter.Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
            var format = (parameter != null)
                ? parameter.ToString()
                : null;
            if (value == null) { return null; }
            if (value is ProcessPriorityClass) {
                var source = (ProcessPriorityClass)value;
                switch (source) {
                    case ProcessPriorityClass.AboveNormal: { return "above normal"; }
                    case ProcessPriorityClass.BelowNormal: { return "below normal"; }
                    case ProcessPriorityClass.High:        { return "high";         }
                    case ProcessPriorityClass.Idle:        { return "idle";         }
                    case ProcessPriorityClass.Normal:      { return "normal";       }
                    case ProcessPriorityClass.RealTime:    { return "realtime";     }
                    }
                return "N/A";
                }
            if (value is TimeSpan) {
                var source = (TimeSpan)value;
                return source.ToString(format);
                }
            return value.ToString();
            }

        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        Object IValueConverter.ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
            {
            return value;
            }
        }
    }