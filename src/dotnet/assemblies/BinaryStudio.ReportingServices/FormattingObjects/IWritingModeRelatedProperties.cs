using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface IWritingModeRelatedProperties
        {
        String Direction {get;set;}
        String GlyphOrientationHorizontal {get;set;}
        String GlyphOrientationVertical {get;set;}
        String TextAltitude {get;set;}
        String TextDepth {get;set;}
        String UnicodeBidi {get;set;}
        String WritingMode {get;set;}
        }
    }
