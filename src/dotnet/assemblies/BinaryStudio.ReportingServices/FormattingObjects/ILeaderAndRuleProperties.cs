using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface ILeaderAndRuleProperties
        {
        String LeaderAlignment {get;set;}
        String LeaderLength {get;set;}
        String LeaderPattern {get;set;}
        String LeaderPatternWidth {get;set;}
        String RuleStyle {get;set;}
        String RuleThickness {get;set;}
        }
    }
