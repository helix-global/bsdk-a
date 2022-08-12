using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BinaryStudio.PlatformUI
    {
    public class MarginFromLeft : IValueConverter
        {
        public MarginFromLeft() {
            Multiplier = 1;
            }

        public Double Multiplier {get;set;}
        public Double Addendum { get;set; }

        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        Object IValueConverter.Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
            if (targetType == typeof(Thickness)) {
                var r = (value is IConvertible e)
                    ? e.ToDouble(null)
                    : 0.0;
                r = Double.IsNaN(r) ? 0.0 : r;
                return new Thickness(Convert(r),0,0,0);
                }
            if (targetType == typeof(Double)) {
                var r = (value is Double)
                    ? (Double)value
                    : 0.0;
                r = Double.IsNaN(r) ? 0.0 : r;
                return Convert(r);
                }
            return value;
            }

        private Double Convert(Double value) {
            return value * Multiplier + Addendum;
            }

        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        Object IValueConverter.ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
            throw new NotImplementedException();
            }
        }
    }