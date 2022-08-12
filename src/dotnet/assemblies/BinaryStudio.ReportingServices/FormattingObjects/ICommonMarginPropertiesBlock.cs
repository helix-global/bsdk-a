using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface ICommonMarginPropertiesBlock
        {
        String EndIndent {get;set;}
        String MarginBottom {get;set;}
        String MarginLeft {get;set;}
        String MarginRight {get;set;}
        String MarginTop {get;set;}
        String SpaceAfter {get;set;}
        String SpaceBefore {get;set;}
        String StartIndent {get;set;}
        }
    }
