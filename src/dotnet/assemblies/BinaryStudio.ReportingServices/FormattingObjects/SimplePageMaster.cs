using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    [Serializable]
    [XmlType(Namespace = "http://www.w3.org/1999/XSL/Format")]
    [XmlRoot(Namespace = "http://www.w3.org/1999/XSL/Format",ElementName="simple-page-master")]
    public class SimplePageMaster
        {
        [XmlElement("region-after")] public RegionAfter RegionAfter { get;set; }
        [XmlElement("region-before")] public RegionBefore RegionBefore { get;set; }
        [XmlElement("region-body")] public RegionBody RegionBody { get;set; }
        [XmlElement("region-end")] public RegionEnd RegionEnd { get;set; }
        [XmlElement("region-start")] public RegionStart RegionStart { get;set; }
        public SimplePageMaster()
            {
            RegionBody = new RegionBody();
            }

        #region P:Serializer:XmlSerializer
        private static XmlSerializer serializer;
        public static XmlSerializer Serializer { get {
            serializer = serializer ?? new XmlSerializer(typeof(SimplePageMaster));
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
