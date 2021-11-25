using System;
using System.Globalization;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class BooleanOrConverter : MultiValueConverter<Boolean, Boolean, Boolean>
        {
        protected override Boolean Convert(Boolean value1, Boolean value2, Object parameter, CultureInfo culture)
            {
            return value1 | value2;
            }
        }
    }