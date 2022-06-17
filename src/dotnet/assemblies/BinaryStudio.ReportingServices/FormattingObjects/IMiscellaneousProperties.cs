using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface IMiscellaneousProperties
        {
        String ContentType {get;set;}
        String Id {get;set;}
        String ProvisionalDistanceBetweenStarts {get;set;}
        String ProvisionalLabelSeparation {get;set;}
        String RefId {get;set;}
        String ScoreSpaces {get;set;}
        String Src {get;set;}
        String Visibility {get;set;}
        String ZIndex {get;set;}
        }
    }
