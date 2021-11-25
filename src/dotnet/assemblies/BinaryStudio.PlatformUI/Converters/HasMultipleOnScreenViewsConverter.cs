using System;
using System.Globalization;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class HasMultipleOnScreenViewsConverter : ValueConverter<OnScreenViewCardinality, Boolean>
        {
        protected override Boolean Convert(OnScreenViewCardinality value, Object parameter, CultureInfo culture)
            {
            return value == OnScreenViewCardinality.Many;
            }
        }
    }