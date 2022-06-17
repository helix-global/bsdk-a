using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    [Serializable]
    [XmlType(Namespace = "http://www.w3.org/1999/XSL/Format")]
    [XmlRoot(Namespace = "http://www.w3.org/1999/XSL/Format",ElementName="list-block")]
    public class ListBlock : GenericBlock
        {
        [XmlElement("list-item")] public List<ListItem> ListItem { get;set; }
        public ListBlock()
            {
            ListItem = new List<ListItem>();
            }

        #region P:Serializer:XmlSerializer
        private static XmlSerializer serializer;
        public static XmlSerializer Serializer { get {
            serializer = serializer ?? new XmlSerializer(typeof(ListBlock));
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
