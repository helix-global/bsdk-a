using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    [Serializable]
    [XmlType(Namespace = "http://www.w3.org/1999/XSL/Format")]
    [XmlRoot(Namespace = "http://www.w3.org/1999/XSL/Format",ElementName="table")]
    public class Table : GenericBlock,ICommonAccessibilityProperties,ICommonAuralProperties,ICommonBorderPaddingAndBackgroundProperties,ICommonMarginPropertiesBlock,ICommonRelativePositionProperties
        {
        [XmlAttribute("azimuth")] public String Azimuth { get;set; }
        [XmlAttribute("background-attachment")] public String BackgroundAttachment { get;set; }
        [XmlAttribute("background-color")] public String BackgroundColor { get;set; }
        [XmlAttribute("background-image")] public String BackgroundImage { get;set; }
        [XmlAttribute("background-position-horizontal")] public String BackgroundPositionHorizontal { get;set; }
        [XmlAttribute("background-position-vertical")] public String BackgroundPositionVertical { get;set; }
        [XmlAttribute("background-repeat")] public String BackgroundRepeat { get;set; }
        [XmlAttribute("block-progression-dimension")] public String BlockProgressionDimension { get;set; }
        [XmlAttribute("border-after-color")] public String BorderAfterColor { get;set; }
        [XmlAttribute("border-after-precedence")] public String BorderAfterPrecedence { get;set; }
        [XmlAttribute("border-after-style")] public String BorderAfterStyle { get;set; }
        [XmlAttribute("border-after-width")] public String BorderAfterWidth { get;set; }
        [XmlAttribute("border-before-color")] public String BorderBeforeColor { get;set; }
        [XmlAttribute("border-before-precedence")] public String BorderBeforePrecedence { get;set; }
        [XmlAttribute("border-before-style")] public String BorderBeforeStyle { get;set; }
        [XmlAttribute("border-before-width")] public String BorderBeforeWidth { get;set; }
        [XmlAttribute("border-bottom-color")] public String BorderBottomColor { get;set; }
        [XmlAttribute("border-bottom-style")] public String BorderBottomStyle { get;set; }
        [XmlAttribute("border-bottom-width")] public String BorderBottomWidth { get;set; }
        [XmlAttribute("border-collapse")] public String BorderCollapse { get;set; }
        [XmlAttribute("border-end-color")] public String BorderEndColor { get;set; }
        [XmlAttribute("border-end-precedence")] public String BorderEndPrecedence { get;set; }
        [XmlAttribute("border-end-style")] public String BorderEndStyle { get;set; }
        [XmlAttribute("border-end-width")] public String BorderEndWidth { get;set; }
        [XmlAttribute("border-left-color")] public String BorderLeftColor { get;set; }
        [XmlAttribute("border-left-style")] public String BorderLeftStyle { get;set; }
        [XmlAttribute("border-left-width")] public String BorderLeftWidth { get;set; }
        [XmlAttribute("border-right-color")] public String BorderRightColor { get;set; }
        [XmlAttribute("border-right-style")] public String BorderRightStyle { get;set; }
        [XmlAttribute("border-right-width")] public String BorderRightWidth { get;set; }
        [XmlAttribute("border-separation")] public String BorderSeparation { get;set; }
        [XmlAttribute("border-start-color")] public String BorderStartColor { get;set; }
        [XmlAttribute("border-start-precedence")] public String BorderStartPrecedence { get;set; }
        [XmlAttribute("border-start-style")] public String BorderStartStyle { get;set; }
        [XmlAttribute("border-start-width")] public String BorderStartWidth { get;set; }
        [XmlAttribute("border-top-color")] public String BorderTopColor { get;set; }
        [XmlAttribute("border-top-style")] public String BorderTopStyle { get;set; }
        [XmlAttribute("border-top-width")] public String BorderTopWidth { get;set; }
        [XmlAttribute("break-after")] public String BreakAfter { get;set; }
        [XmlAttribute("break-before")] public String BreakBefore { get;set; }
        [XmlAttribute("cue-after")] public String CueAfter { get;set; }
        [XmlAttribute("cue-before")] public String CueBefore { get;set; }
        [XmlAttribute("elevation")] public String Elevation { get;set; }
        [XmlAttribute("end-indent")] public String EndIndent { get;set; }
        [XmlAttribute("height")] public String Height { get;set; }
        [XmlAttribute("id")] public String Id { get;set; }
        [XmlAttribute("inline-progression-dimension")] public String InlineProgressionDimension { get;set; }
        [XmlAttribute("keep-together")] public String KeepTogether { get;set; }
        [XmlAttribute("keep-with-next")] public String KeepWithNext { get;set; }
        [XmlAttribute("keep-with-previous")] public String KeepWithPrevious { get;set; }
        [XmlAttribute("margin-bottom")] public String MarginBottom { get;set; }
        [XmlAttribute("margin-left")] public String MarginLeft { get;set; }
        [XmlAttribute("margin-right")] public String MarginRight { get;set; }
        [XmlAttribute("margin-top")] public String MarginTop { get;set; }
        [XmlAttribute("padding-after")] public String PaddingAfter { get;set; }
        [XmlAttribute("padding-before")] public String PaddingBefore { get;set; }
        [XmlAttribute("padding-bottom")] public String PaddingBottom { get;set; }
        [XmlAttribute("padding-end")] public String PaddingEnd { get;set; }
        [XmlAttribute("padding-left")] public String PaddingLeft { get;set; }
        [XmlAttribute("padding-right")] public String PaddingRight { get;set; }
        [XmlAttribute("padding-start")] public String PaddingStart { get;set; }
        [XmlAttribute("padding-top")] public String PaddingTop { get;set; }
        [XmlAttribute("pause-after")] public String PauseAfter { get;set; }
        [XmlAttribute("pause-before")] public String PauseBefore { get;set; }
        [XmlAttribute("pitch")] public String Pitch { get;set; }
        [XmlAttribute("pitch-range")] public String PitchRange { get;set; }
        [XmlAttribute("play-during")] public String PlayDuring { get;set; }
        [XmlAttribute("relative-position")] public String RelativePosition { get;set; }
        [XmlAttribute("richness")] public String Richness { get;set; }
        [XmlAttribute("role")][DefaultValue("none")] public String Role { get;set; }
        [XmlAttribute("source-document")] public String SourceDocument { get;set; }
        [XmlAttribute("space-after")] public String SpaceAfter { get;set; }
        [XmlAttribute("space-before")] public String SpaceBefore { get;set; }
        [XmlAttribute("speak")] public String Speak { get;set; }
        [XmlAttribute("speak-header")] public String SpeakHeader { get;set; }
        [XmlAttribute("speak-numeral")] public String SpeakNumeral { get;set; }
        [XmlAttribute("speak-punctuation")] public String SpeakPunctuation { get;set; }
        [XmlAttribute("speech-rate")] public String SpeechRate { get;set; }
        [XmlAttribute("start-indent")] public String StartIndent { get;set; }
        [XmlAttribute("stress")] public String Stress { get;set; }
        [XmlElement("table-body")] public List<TableBody> TableBody { get;set; }
        [XmlElement("table-column")] public List<TableColumn> TableColumn { get;set; }
        [XmlElement("table-footer")] public TableFooter TableFooter { get;set; }
        [XmlElement("table-header")] public TableHeader TableHeader { get;set; }
        [XmlAttribute("table-layout")] public String TableLayout { get;set; }
        [XmlAttribute("table-omit-footer-at-break")] public String TableOmitFooterAtBreak { get;set; }
        [XmlAttribute("table-omit-header-at-break")] public String TableOmitHeaderAtBreak { get;set; }
        [XmlAttribute("voice-family")] public String VoiceFamily { get;set; }
        [XmlAttribute("volume")] public String Volume { get;set; }
        [XmlAttribute("width")] public String Width { get;set; }
        [XmlAttribute("writing-mode")] public String WritingMode { get;set; }
        public Table()
            {
            Role = "none";
            TableBody = new List<TableBody>();
            TableColumn = new List<TableColumn>();
            }

        #region P:Serializer:XmlSerializer
        private static XmlSerializer serializer;
        public static XmlSerializer Serializer { get {
            serializer = serializer ?? new XmlSerializer(typeof(Table));
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
