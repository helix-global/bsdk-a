using System;
using System.Xml;

namespace BinaryStudio.Security.XAdES.Internal
    {
    internal class XAdESXmlElement : XAdESXmlObject<XmlElement>
        {
        public XAdESXmlElement(XmlElement source)
            :base(source)
            {
            }
        }
    }