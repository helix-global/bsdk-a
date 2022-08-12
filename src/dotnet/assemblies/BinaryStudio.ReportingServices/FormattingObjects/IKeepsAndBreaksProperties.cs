using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface IKeepsAndBreaksProperties
        {
        String BreakAfter {get;set;}
        String BreakBefore {get;set;}
        String KeepTogether {get;set;}
        String KeepWithNext {get;set;}
        String KeepWithPrevious {get;set;}
        String Orphans {get;set;}
        String Widows {get;set;}
        }
    }
