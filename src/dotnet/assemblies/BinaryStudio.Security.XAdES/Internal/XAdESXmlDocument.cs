using System;
using System.Xml;

namespace BinaryStudio.Security.XAdES.Internal
    {
    internal class XAdESXmlDocument : XAdESXmlObject<XmlDocument>
        {
        public XAdESXmlDocument(XmlDocument source)
            :base(source)
            {
            }

        public XAdESXmlElement CreateElement(String qualifiedName, String namespaceURI)
            {
            return new XAdESXmlElement(Source.CreateElement(qualifiedName, namespaceURI));
            }
        }
    }