using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface IAreaAlignmentProperties
        {
        String AlignmentAdjust {get;set;}
        String AlignmentBaseline {get;set;}
        String BaselineShift {get;set;}
        String DisplayAlign {get;set;}
        String DominantBaseline {get;set;}
        String RelativeAlign {get;set;}
        }
    }
