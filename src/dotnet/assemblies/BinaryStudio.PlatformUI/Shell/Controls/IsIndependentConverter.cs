using System;
using System.Globalization;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class IsIndependentConverter : MultiValueConverter<Boolean, Boolean, Boolean, Boolean>
        {
        protected override Boolean Convert(Boolean enableIndependentDocWells, Boolean enableIndependentToolwindows, Boolean hasDocumentGroupContainer, Object parameter, CultureInfo culture)
            {
            if (!enableIndependentToolwindows)
                return enableIndependentDocWells & hasDocumentGroupContainer;
            return true;
            }
        }
    }