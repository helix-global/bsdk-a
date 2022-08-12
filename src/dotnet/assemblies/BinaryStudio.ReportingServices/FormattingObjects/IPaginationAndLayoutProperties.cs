using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface IPaginationAndLayoutProperties
        {
        String BlankOrNotBlank {get;set;}
        String ColumnCount {get;set;}
        String ColumnGap {get;set;}
        String Extent {get;set;}
        String FlowName {get;set;}
        String ForcePageCount {get;set;}
        String InitialPageNumber {get;set;}
        String MasterName {get;set;}
        String MaximumRepeats {get;set;}
        String MediaUsage {get;set;}
        String OddOrEven {get;set;}
        String PageHeight {get;set;}
        String PagePosition {get;set;}
        String PageWidth {get;set;}
        String Precedence {get;set;}
        String RegionName {get;set;}
        }
    }
