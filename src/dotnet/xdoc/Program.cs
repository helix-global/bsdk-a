using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace xdoc
    {
    internal class Program
        {
        private static readonly String XmlDocUri = "http://xmldoc.schemas.helix.global";
        private static void Main(String[] args)
            {
            String inputfilename = null;
            String outputfilename = null;
            String lang = null;
            if (args.Length > 0) {
                for (var i = 0; i < args.Length;i++) {
                         if (args[i].StartsWith("input:" )) { inputfilename  = args[i].Substring(6); }
                    else if (args[i].StartsWith("output:")) { outputfilename = args[i].Substring(7); }
                    else if (args[i].StartsWith("lang:"  )) { lang           = args[i].Substring(5); }
                    }
                }
            if (String.Equals(lang, "en-US")) { lang = null; }
            var suffix = String.IsNullOrWhiteSpace(lang)
                ? "en-US"
                : lang;
            outputfilename = String.IsNullOrWhiteSpace(outputfilename)
                ? Path.ChangeExtension(inputfilename, $".{suffix}.xml")
                : outputfilename;
            if (File.Exists(outputfilename)) {
                File.Delete(outputfilename);
                }
            using (var inputstream = File.OpenRead(inputfilename))
            using (var outputstream = File.OpenWrite(outputfilename)) {
                using (var writer = XmlWriter.Create(outputstream, new XmlWriterSettings {
                    Indent = true,
                    IndentChars = " ",
                    Encoding = new UTF8Encoding(false)
                    }))
                    {
                    var document = new XmlDocument();
                    document.Load(inputstream);
                    var nsmgr = new XmlNamespaceManager(document.NameTable);
                    nsmgr.AddNamespace("x", XmlDocUri);
                    Process(writer, nsmgr, lang, document.DocumentElement);
                    }
                }
            }

        private static void CopyAttributes(XmlWriter writer, XmlElement source) {
            foreach (XmlAttribute attribute in source.Attributes) {
                writer.WriteAttributeString(
                    attribute.Prefix,
                    attribute.LocalName,
                    attribute.NamespaceURI,
                    attribute.Value);
                }
            }

        private static void CopyChildNodes(XmlWriter writer, XmlNamespaceManager nsmgr, String lang, IEnumerable<XmlNode> nodes) {
            foreach (var e in nodes) {
                switch (e.NodeType) {
                    case XmlNodeType.Element:
                        {
                        Process(writer, nsmgr, lang, (XmlElement)e);
                        }
                        break;
                    case XmlNodeType.Text:
                        {
                        writer.WriteString(e.InnerText);
                        }
                        break;
                    default:
                        {
                                
                        }
                        break;
                    }
                }        
            }

        private static void Process(XmlWriter writer, XmlNamespaceManager nsmgr, String lang, XmlElement source) {
            switch (source.Name) {
                case "member":
                    {
                    writer.WriteStartElement(source.Name);
                    CopyAttributes(writer, source);
                    var block = (lang != null)
                        ? source.SelectSingleNode($"x:block[@x:lang='{lang}']", nsmgr)
                        : null;
                    if (block != null) {
                        CopyChildNodes(writer, nsmgr, lang, block.ChildNodes.OfType<XmlNode>());
                        }
                    else
                        {
                        CopyChildNodes(writer, nsmgr, lang, source.ChildNodes.
                            OfType<XmlNode>().
                            Where(i => ((i.NamespaceURI != XmlDocUri) || (i.LocalName != "block"))));
                        }
                    writer.WriteEndElement();
                    }
                    break;
                default:
                    {
                    writer.WriteStartElement(source.Name);
                    CopyAttributes(writer, source);
                    CopyChildNodes(writer, nsmgr, lang, source.ChildNodes.OfType<XmlNode>());
                    }
                    writer.WriteEndElement();
                    break;
                }
            }
        }
    }
