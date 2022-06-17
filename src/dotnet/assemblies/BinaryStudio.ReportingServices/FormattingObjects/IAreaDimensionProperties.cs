using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface IAreaDimensionProperties
        {
        String BlockProgressionDimension {get;set;}
        String ContentHeight {get;set;}
        String ContentWidth {get;set;}
        String Height {get;set;}
        String InlineProgressionDimension {get;set;}
        String MaxHeight {get;set;}
        String MaxWidth {get;set;}
        String MinHeight {get;set;}
        String MinWidth {get;set;}
        String Scaling {get;set;}
        String ScalingMethod {get;set;}
        String Width {get;set;}
        }
    }
