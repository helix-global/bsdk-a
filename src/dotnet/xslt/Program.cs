using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace xslt
    {
    internal class Program
        {
        private static void Main(string[] args)
            {
            String stylesheet = null;
            String inputfilename = null;
            String outputfilename = null;
            if (args.Length > 0) {
                for (var i = 0; i < args.Length;) {
                    switch (args[i]) {
                        case "-t":
                                {
                                if (i < args.Length - 1) {
                                    stylesheet = args[i + 1];
                                    i += 2;
                                    }
                                }
                            break;
                        case "-i":
                                {
                                if (i < args.Length - 1) {
                                    inputfilename = args[i + 1];
                                    i += 2;
                                    }
                                }
                            break;
                        case "-o":
                                {
                                if (i < args.Length - 1) {
                                    outputfilename = args[i + 1];
                                    i += 2;
                                    }
                                }
                            break;
                        }
                    }
                }
            var xslt = new XslCompiledTransform();
            xslt.Load(stylesheet);
            if (String.IsNullOrWhiteSpace(outputfilename)) {
                using (var writer = new XmlTextWriter(Console.Out)) {
                    writer.Formatting=Formatting.Indented;
                    xslt.Transform(new XPathDocument(inputfilename), writer);
                    }
                }
            else
                {
                using (var output = File.OpenWrite(outputfilename)) {
                    using (var writer = XmlWriter.Create(output, new XmlWriterSettings {
                        Encoding = Encoding.UTF8,
                        Indent = true,
                        IndentChars = " "
                        })) {
                        xslt.Transform(new XPathDocument(inputfilename), writer);
                        }
                    }
                }
            }
        }
    }
