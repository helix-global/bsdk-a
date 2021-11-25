using System;
using System.Globalization;
using System.Windows.Data;

namespace BinaryStudio.PlatformUI.Converters
    {
    public class Equals : IMultiValueConverter
        {
        Object IMultiValueConverter.Convert(Object[] values, Type targetType, Object parameter, CultureInfo culture)
            {
            if (values.Length < 2) { throw new ArgumentOutOfRangeException(nameof(values)); }
            for (var i = 0; i < values.Length - 1; i++) {
                if (!Equals(values[i], values[i + 1])) { return false; }
                }
            return true;
            }

        Object[] IMultiValueConverter.ConvertBack(Object value, Type[] targetTypes, Object parameter, CultureInfo culture)
            {
            throw new NotImplementedException();
            }
        }
    }