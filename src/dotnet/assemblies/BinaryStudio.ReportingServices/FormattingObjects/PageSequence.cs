using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    [Serializable]
    [XmlType(Namespace = "http://www.w3.org/1999/XSL/Format")]
    [XmlRoot(Namespace = "http://www.w3.org/1999/XSL/Format",ElementName="page-sequence")]
    public class PageSequence
        {
        [XmlAttribute("country")] public String Country { get;set; }
        [XmlElement("flow")] public Flow Flow { get;set; }
        [XmlAttribute("force-page-count")] public String ForcePageCount { get;set; }
        [XmlAttribute("format")] public String Format { get;set; }
        [XmlAttribute("grouping-separator")] public String GroupingSeparator { get;set; }
        [XmlAttribute("grouping-size")] public String GroupingSize { get;set; }
        [XmlAttribute("id")] public String Id { get;set; }
        [XmlAttribute("initial-page-number")] public String InitialPageNumber { get;set; }
        [XmlAttribute("language")] public String Language { get;set; }
        [XmlAttribute("letter-value")] public String LetterValue { get;set; }
        [XmlAttribute("master-name")] public String MasterName { get;set; }
        [XmlElement("static-content")] public List<StaticContent> StaticContent { get;set; }
        [XmlElement("title")] public Title Title { get;set; }
        public PageSequence()
            {
            Flow = new Flow();
            StaticContent = new List<StaticContent>();
            }

        #region P:Serializer:XmlSerializer
        private static XmlSerializer serializer;
        public static XmlSerializer Serializer { get {
            serializer = serializer ?? new XmlSerializer(typeof(PageSequence));
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
