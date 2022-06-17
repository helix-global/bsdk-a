using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface IPropertiesForNumberToStringConversion
        {
        String Format {get;set;}
        String GroupingSeparator {get;set;}
        String GroupingSize {get;set;}
        String LetterValue {get;set;}
        }
    }
