using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface ICommonFontProperties
        {
        String FontFamily {get;set;}
        String FontSelectionStrategy {get;set;}
        String FontSize {get;set;}
        String FontSizeAdjust {get;set;}
        String FontStretch {get;set;}
        String FontStyle {get;set;}
        String FontVariant {get;set;}
        String FontWeight {get;set;}
        }
    }
