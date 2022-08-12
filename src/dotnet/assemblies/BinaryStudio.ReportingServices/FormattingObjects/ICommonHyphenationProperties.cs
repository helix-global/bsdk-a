using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface ICommonHyphenationProperties
        {
        String Country {get;set;}
        String Hyphenate {get;set;}
        String HyphenationCharacter {get;set;}
        String HyphenationPushCharacterCount {get;set;}
        String HyphenationRemainCharacterCount {get;set;}
        String Language {get;set;}
        String Script {get;set;}
        }
    }
