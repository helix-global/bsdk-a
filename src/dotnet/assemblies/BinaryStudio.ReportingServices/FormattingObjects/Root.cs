using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    [Serializable]
    [XmlType(Namespace = "http://www.w3.org/1999/XSL/Format")]
    [XmlRoot(Namespace = "http://www.w3.org/1999/XSL/Format",ElementName="root")]
    public class Root : ICommonAccessibilityProperties
        {
        [XmlElement("declarations")] public Declarations Declarations { get;set; }
        [XmlElement("layout-master-set")] public LayoutMasterSet LayoutMasterSet { get;set; }
        [XmlAttribute("media-usage")] public String MediaUsage { get;set; }
        [XmlElement("page-sequence")] public List<PageSequence> PageSequence { get;set; }
        [XmlAttribute("role")][DefaultValue("none")] public String Role { get;set; }
        [XmlAttribute("source-document")] public String SourceDocument { get;set; }
        public Root()
            {
            LayoutMasterSet = new LayoutMasterSet();
            PageSequence = new List<PageSequence>();
            Role = "none";
            }

        #region P:Serializer:XmlSerializer
        private static XmlSerializer serializer;
        public static XmlSerializer Serializer { get {
            serializer = serializer ?? new XmlSerializer(typeof(Root));
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
