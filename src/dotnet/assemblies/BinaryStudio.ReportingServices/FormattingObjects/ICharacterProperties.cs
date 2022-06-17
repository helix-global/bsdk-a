using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface ICharacterProperties
        {
        String Character {get;set;}
        String LetterSpacing {get;set;}
        String SuppressAtLineBreak {get;set;}
        String TextDecoration {get;set;}
        String TextShadow {get;set;}
        String TextTransform {get;set;}
        String TreatAsWordSpace {get;set;}
        String WordSpacing {get;set;}
        }
    }
