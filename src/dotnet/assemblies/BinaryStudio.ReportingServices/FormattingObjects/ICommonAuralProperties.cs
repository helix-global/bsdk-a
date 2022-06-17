using System;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    public interface ICommonAuralProperties
        {
        String Azimuth {get;set;}
        String CueAfter {get;set;}
        String CueBefore {get;set;}
        String Elevation {get;set;}
        String PauseAfter {get;set;}
        String PauseBefore {get;set;}
        String Pitch {get;set;}
        String PitchRange {get;set;}
        String PlayDuring {get;set;}
        String Richness {get;set;}
        String Speak {get;set;}
        String SpeakHeader {get;set;}
        String SpeakNumeral {get;set;}
        String SpeakPunctuation {get;set;}
        String SpeechRate {get;set;}
        String Stress {get;set;}
        String VoiceFamily {get;set;}
        String Volume {get;set;}
        }
    }
