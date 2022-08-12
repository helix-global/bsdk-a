using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    [Serializable]
    [XmlType(Namespace = "http://www.w3.org/1999/XSL/Format")]
    [XmlRoot(Namespace = "http://www.w3.org/1999/XSL/Format",ElementName="title")]
    public class Title : ICommonAccessibilityProperties,ICommonAuralProperties
        {
        [XmlAttribute("azimuth")] public String Azimuth { get;set; }
        [XmlAttribute("cue-after")] public String CueAfter { get;set; }
        [XmlAttribute("cue-before")] public String CueBefore { get;set; }
        [XmlAttribute("elevation")] public String Elevation { get;set; }
        [XmlAttribute("pause-after")] public String PauseAfter { get;set; }
        [XmlAttribute("pause-before")] public String PauseBefore { get;set; }
        [XmlAttribute("pitch")] public String Pitch { get;set; }
        [XmlAttribute("pitch-range")] public String PitchRange { get;set; }
        [XmlAttribute("play-during")] public String PlayDuring { get;set; }
        [XmlAttribute("richness")] public String Richness { get;set; }
        [XmlAttribute("role")][DefaultValue("none")] public String Role { get;set; }
        [XmlAttribute("source-document")] public String SourceDocument { get;set; }
        [XmlAttribute("speak")] public String Speak { get;set; }
        [XmlAttribute("speak-header")] public String SpeakHeader { get;set; }
        [XmlAttribute("speak-numeral")] public String SpeakNumeral { get;set; }
        [XmlAttribute("speak-punctuation")] public String SpeakPunctuation { get;set; }
        [XmlAttribute("speech-rate")] public String SpeechRate { get;set; }
        [XmlAttribute("stress")] public String Stress { get;set; }
        [XmlAttribute("voice-family")] public String VoiceFamily { get;set; }
        [XmlAttribute("volume")] public String Volume { get;set; }
        public Title()
            {
            Role = "none";
            }

        #region P:Serializer:XmlSerializer
        private static XmlSerializer serializer;
        public static XmlSerializer Serializer { get {
            serializer = serializer ?? new XmlSerializer(typeof(Title));
            return serializer;
            }}
        #endregion

        public override String Serialize()
            {
            using (var output = new MemoryStream()) {
                Serializer.Serialize(output, this);
                output.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(output)) {
                    return reader.ReadToEnd();
                    }
                }
            }
        }
    }
