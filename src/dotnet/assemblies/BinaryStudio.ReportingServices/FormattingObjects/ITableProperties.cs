using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface ITableProperties
        {
        String BorderAfterPrecedence {get;set;}
        String BorderBeforePrecedence {get;set;}
        String BorderCollapse {get;set;}
        String BorderEndPrecedence {get;set;}
        String BorderSeparation {get;set;}
        String BorderStartPrecedence {get;set;}
        String CaptionSide {get;set;}
        String ColumnNumber {get;set;}
        String ColumnWidth {get;set;}
        String EmptyCells {get;set;}
        String EndsRow {get;set;}
        String NumberColumnsRepeated {get;set;}
        String NumberColumnsSpanned {get;set;}
        String NumberRowsSpanned {get;set;}
        String StartsRow {get;set;}
        String TableLayout {get;set;}
        String TableOmitFooterAtBreak {get;set;}
        String TableOmitHeaderAtBreak {get;set;}
        }
    }
