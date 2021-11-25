using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using BinaryStudio.PlatformUI.Properties;

namespace BinaryStudio.PlatformUI
    {
    public class PriorityClass : IValueConverter
        {
        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        Object IValueConverter.Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
            if (value is ProcessPriorityClass) {
                var source = (ProcessPriorityClass)value;
                switch (source) {
                    case ProcessPriorityClass.AboveNormal: { return Resources.ProcessPriorityClass_AboveNormal;  }
                    case ProcessPriorityClass.BelowNormal: { return Resources.ProcessPriorityClass_BelowNormal;  }
                    case ProcessPriorityClass.High:        { return Resources.ProcessPriorityClass_High;         }
                    case ProcessPriorityClass.Idle:        { return Resources.ProcessPriorityClass_Idle;         }
                    case ProcessPriorityClass.Normal:      { return Resources.ProcessPriorityClass_Normal;       }
                    case ProcessPriorityClass.RealTime:    { return Resources.ProcessPriorityClass_RealTime;     }
                    }
                return Resources.ProcessPriorityClass_NA;
                }
            return value;
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