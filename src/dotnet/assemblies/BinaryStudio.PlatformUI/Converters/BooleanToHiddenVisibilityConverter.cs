using System;
using System.Globalization;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class BooleanToHiddenVisibilityConverter : ValueConverter<Boolean, Visibility>
        {
        protected override Visibility Convert(Boolean value, Object parameter, CultureInfo culture)
            {
            var result = Visibility.Hidden;
            if (value)
                {
                result = Visibility.Visible;
                }
            return result;
            }
        }
    }