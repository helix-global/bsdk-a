using System;
using System.Collections;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace BinaryStudio.IO
    {
    public class ExceptionXmlSerializer
        {
        #region M:WriteProperty(XmlWriterAdapter,Boolean,String,Object)
        private static void WriteProperty(XmlWriterAdapter writer, Boolean newline, String propertyname, Object value) {
            if (value != null) {
                writer.WriteStartAttribute(propertyname, newline);
                writer.WriteString(value.ToString());
                writer.WriteEndAttribute();
                }
            }
        #endregion

        private void WriteException(XmlWriterAdapter writer, Exception e) {
            using (writer.WriteStartElement("Exception")) {
                WriteProperty(writer, false, "Type", e.GetType().Name);
                using (writer.WriteStartElement("Message")) {
                    writer.WriteWhitespace(String.Empty);
                    using (writer.CDataScope()) {
                        writer.WriteString(e.Message);
                        }
                    }
                if (e is AggregateException)
                    if (((AggregateException)e).InnerExceptions.Count > 0) {
                        using (writer.WriteStartElement("Exceptions")) {
                            WriteProperty(writer, false, "Count", ((AggregateException)e).InnerExceptions.Count);
                            foreach (var i in ((AggregateException)e).InnerExceptions) {
                                WriteException(writer, i);
                                }
                            }
                        }
                else if (e.InnerException != null) {
                    using (writer.WriteStartElement("Exceptions")) {
                        WriteProperty(writer, false, "Count", 1);
                        WriteException(writer, e.InnerException);
                        }
                    }
                if (e.Data.Count > 0) {
                    using (writer.WriteStartElement("Data")) {
                        WriteProperty(writer, false, "Count", e.Data.Count);
                        var n = e.Data.Keys.OfType<Object>().Select(i => i.ToString()).Max(i => i.Length);
                        foreach (DictionaryEntry i in e.Data) {
                            var key = i.Key.ToString();
                            using (writer.WriteStartElement("KeyValuePair")) {
                                WriteProperty(writer, false,  "Key",  key);
                                var value = i.Value;
                                if (value is IXmlSerializable) {
                                    using (writer.WriteStartElement("Value")) {
                                        ((IXmlSerializable)value).WriteXml(writer.Writer);
                                        }
                                    }
                                else
                                    {
                                    writer.WriteWhitespace(n - key.Length);
                                    WriteProperty(writer, false,  "Value", i.Value);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        private void WriteInternal(XmlWriterAdapter writer, Exception e) {
            }

        public void Write(XmlWriter writer, Exception e)
            {
            var flag = writer.Settings?.NewLineOnAttributes;
            using (var adapter = (flag == true)
                ? (XmlWriterAdapter)new XmlWriterDefaultAdapter(writer)
                : (XmlWriterAdapter)new XmlWellFormedWriterAdapter(writer))
                {
                WriteException(adapter, e);
                }
            }
        }
    }