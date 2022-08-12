using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    [Serializable]
    [XmlType(Namespace = "http://www.w3.org/1999/XSL/Format")]
    [XmlRoot(Namespace = "http://www.w3.org/1999/XSL/Format",ElementName="table-column")]
    public class TableColumn : ICommonBorderPaddingAndBackgroundProperties
        {
        [XmlAttribute("background-attachment")] public String BackgroundAttachment { get;set; }
        [XmlAttribute("background-color")] public String BackgroundColor { get;set; }
        [XmlAttribute("background-image")] public String BackgroundImage { get;set; }
        [XmlAttribute("background-position-horizontal")] public String BackgroundPositionHorizontal { get;set; }
        [XmlAttribute("background-position-vertical")] public String BackgroundPositionVertical { get;set; }
        [XmlAttribute("background-repeat")] public String BackgroundRepeat { get;set; }
        [XmlAttribute("border-after-color")] public String BorderAfterColor { get;set; }
        [XmlAttribute("border-after-style")] public String BorderAfterStyle { get;set; }
        [XmlAttribute("border-after-width")] public String BorderAfterWidth { get;set; }
        [XmlAttribute("border-before-color")] public String BorderBeforeColor { get;set; }
        [XmlAttribute("border-before-style")] public String BorderBeforeStyle { get;set; }
        [XmlAttribute("border-before-width")] public String BorderBeforeWidth { get;set; }
        [XmlAttribute("border-bottom-color")] public String BorderBottomColor { get;set; }
        [XmlAttribute("border-bottom-style")] public String BorderBottomStyle { get;set; }
        [XmlAttribute("border-bottom-width")] public String BorderBottomWidth { get;set; }
        [XmlAttribute("border-end-color")] public String BorderEndColor { get;set; }
        [XmlAttribute("border-end-style")] public String BorderEndStyle { get;set; }
        [XmlAttribute("border-end-width")] public String BorderEndWidth { get;set; }
        [XmlAttribute("border-left-color")] public String BorderLeftColor { get;set; }
        [XmlAttribute("border-left-style")] public String BorderLeftStyle { get;set; }
        [XmlAttribute("border-left-width")] public String BorderLeftWidth { get;set; }
        [XmlAttribute("border-right-color")] public String BorderRightColor { get;set; }
        [XmlAttribute("border-right-style")] public String BorderRightStyle { get;set; }
        [XmlAttribute("border-right-width")] public String BorderRightWidth { get;set; }
        [XmlAttribute("border-start-color")] public String BorderStartColor { get;set; }
        [XmlAttribute("border-start-style")] public String BorderStartStyle { get;set; }
        [XmlAttribute("border-start-width")] public String BorderStartWidth { get;set; }
        [XmlAttribute("border-top-color")] public String BorderTopColor { get;set; }
        [XmlAttribute("border-top-style")] public String BorderTopStyle { get;set; }
        [XmlAttribute("border-top-width")] public String BorderTopWidth { get;set; }
        [XmlAttribute("column-number")] public String ColumnNumber { get;set; }
        [XmlAttribute("column-width")] public String ColumnWidth { get;set; }
        [XmlAttribute("number-columns-repeated")] public String NumberColumnsRepeated { get;set; }
        [XmlAttribute("number-columns-spanned")] public String NumberColumnsSpanned { get;set; }
        [XmlAttribute("padding-after")] public String PaddingAfter { get;set; }
        [XmlAttribute("padding-before")] public String PaddingBefore { get;set; }
        [XmlAttribute("padding-bottom")] public String PaddingBottom { get;set; }
        [XmlAttribute("padding-end")] public String PaddingEnd { get;set; }
        [XmlAttribute("padding-left")] public String PaddingLeft { get;set; }
        [XmlAttribute("padding-right")] public String PaddingRight { get;set; }
        [XmlAttribute("padding-start")] public String PaddingStart { get;set; }
        [XmlAttribute("padding-top")] public String PaddingTop { get;set; }
        [XmlAttribute("visibility")] public String Visibility { get;set; }
        public TableColumn()
            {
            }

        #region P:Serializer:XmlSerializer
        private static XmlSerializer serializer;
        public static XmlSerializer Serializer { get {
            serializer = serializer ?? new XmlSerializer(typeof(TableColumn));
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
