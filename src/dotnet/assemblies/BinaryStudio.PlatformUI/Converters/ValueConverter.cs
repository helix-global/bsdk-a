using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using BinaryStudio.PlatformUI.Properties;

namespace BinaryStudio.PlatformUI
    {
    public class ValueConverter<TSource, TTarget> : IValueConverter
        {
        private static readonly Object DisconnectedSource = typeof(BindingExpressionBase).GetField("DisconnectedItem", BindingFlags.NonPublic|BindingFlags.Static).GetValue(null);

        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
            {
            if (value == DisconnectedSource) { return value; }
            if (!(value is TSource) && (value != null || typeof(TSource).IsValueType)) { throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.Error_ValueNotOfType, typeof(TSource).FullName)); }
            if (!targetType.IsAssignableFrom(typeof(TTarget))) { throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, Resources.Error_TargetNotExtendingType, typeof(TTarget).FullName)); }
            return Convert((TSource)value, parameter, culture);
            }

        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
            {
            if (!(value is TTarget) && (value != null || typeof(TTarget).IsValueType)) { throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.Error_ValueNotOfType, typeof(TTarget).FullName)); }
            if (!targetType.IsAssignableFrom(typeof(TSource))) { throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, Resources.Error_TargetNotExtendingType, typeof(TSource).FullName)); }
            return ConvertBack((TTarget)value, parameter, culture);
            }

        protected virtual TTarget Convert(TSource value, Object parameter, CultureInfo culture)
            {
            throw new NotSupportedException(String.Format(CultureInfo.CurrentCulture, Resources.Error_ConverterFunctionNotDefined, "Convert"));
            }

        protected virtual TSource ConvertBack(TTarget value, Object parameter, CultureInfo culture)
            {
            throw new NotSupportedException(String.Format(CultureInfo.CurrentCulture, Resources.Error_ConverterFunctionNotDefined, "ConvertBack"));
            }
        }
    }