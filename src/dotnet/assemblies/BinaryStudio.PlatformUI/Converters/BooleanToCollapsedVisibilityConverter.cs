using System;
using System.Globalization;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class BooleanToCollapsedVisibilityConverter : ValueConverter<Boolean, Visibility>
        {
        protected override Visibility Convert(Boolean value, Object parameter, CultureInfo culture)
            {
            return value
                ? Visibility.Visible
                : Visibility.Collapsed;
            }
        }
    }