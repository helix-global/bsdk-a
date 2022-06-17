using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface IBlockAndLineRelatedProperties
        {
        String HyphenationKeep {get;set;}
        String HyphenationLadderCount {get;set;}
        String LastLineEndIndent {get;set;}
        String LinefeedTreatment {get;set;}
        String LineHeight {get;set;}
        String LineHeightShiftAdjustment {get;set;}
        String LineStackingStrategy {get;set;}
        String SpaceTreatment {get;set;}
        String TextAlign {get;set;}
        String TextAlignLast {get;set;}
        String TextIndent {get;set;}
        String WhiteSpaceCollapse {get;set;}
        String WrapOption {get;set;}
        }
    }
