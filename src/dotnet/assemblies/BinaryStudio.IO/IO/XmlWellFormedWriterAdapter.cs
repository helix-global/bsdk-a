using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace BinaryStudio.IO
    {
    public class XmlWellFormedWriterAdapter : XmlWriterAdapter
        {
        private Boolean firstattribute = true;
        private readonly FieldInfo identlevelfi;
        private readonly Object    nestedwriter;
        private String elementname;
        private Boolean InCData;
        private XmlWriterAdapterFlags CDataFlags;

        public TextWriter TextWriter { get; }

        public XmlWellFormedWriterAdapter(XmlWriter writer)
            : base(writer)
            {
            var writerfi = Writer.GetType().GetField("writer", BindingFlags.Instance | BindingFlags.NonPublic);
            if (writerfi != null) {
                nestedwriter = writerfi.GetValue(Writer);
                if (nestedwriter != null) {
                    identlevelfi = nestedwriter.GetType().GetField("indentLevel", BindingFlags.Instance | BindingFlags.NonPublic);
                    var wfi = nestedwriter.GetType().GetField("writer", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (wfi != null) {
                        TextWriter = (TextWriter)wfi.GetValue(nestedwriter);
                        }
                    }
                }
            var settings = writer.Settings;
            IndentChars = (settings != null)
                ? settings.IndentChars
                : "  ";
            }

        public Int32 IndentLevel { get {
            return (identlevelfi != null)
                ? (Int32)identlevelfi.GetValue(nestedwriter)
                : 0;
            }}
        public String IndentChars { get; }

        #region M:WriteStartElement(String,String,String)
        /// <summary>When overridden in a derived class, writes the specified start tag and associates it with the given namespace and prefix.</summary>
        /// <param name="prefix">The namespace prefix of the element. </param>
        /// <param name="localname">The local name of the element. </param>
        /// <param name="ns">The namespace URI to associate with the element. </param>
        /// <exception cref="T:System.InvalidOperationException">The writer is closed. </exception>
        /// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections. </exception>
        public override void WriteStartElement(String prefix, String localname, String ns)
            {
            elementname = localname;
            base.WriteStartElement(prefix, localname, ns);
            }
        #endregion
        #region M:WriteWhitespace(Int32)
        public override void WriteWhitespace(Int32 length) {
            if (length > 0) {
                var writer = TextWriter;
                if (writer != null) {
                    Writer.Flush();
                    writer.Write(new String(' ', length));
                    }
                }
            }
        #endregion
        #region M:WriteStartAttribute(String,Boolean)
        public override void WriteStartAttribute(String localname, Boolean newline) {
            if (newline) {
                var writer = TextWriter;
                if (writer != null) {
                    Writer.Flush();
                    writer.WriteLine();
                    writer.Write(new String(' ', IndentLevel*IndentChars.Length + elementname.Length - 1));
                    }
                }
            base.WriteStartAttribute(localname, newline);
            }
        #endregion

        public override void WriteLine()
            {
            var writer = TextWriter;
            if (writer != null) {
                Writer.Flush();
                writer.WriteLine();
                writer.Write(new String(' ', IndentLevel*IndentChars.Length));
                }
            }

        protected override void WriteStartCData() {
            InCData = true;
            var writer = TextWriter;
            if (writer != null) {
                Writer.Flush();
                writer.WriteLine();
                writer.Write(new String(' ', IndentLevel*IndentChars.Length));
                writer.Write("<![CDATA[");
                }
            }

        protected override void WriteEndCData() {
            InCData = false;
            var writer = TextWriter;
            if (writer != null) {
                Writer.Flush();
                writer.Write("]]>");
                writer.WriteLine();
                writer.Write(new String(' ', (IndentLevel - 1)*IndentChars.Length));
                }
            }
        }
    }