using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    [XmlInclude(typeof(BasicLink))]
    [XmlInclude(typeof(BidiOverride))]
    [XmlInclude(typeof(Character))]
    [XmlInclude(typeof(ExternalGraphic))]
    [XmlInclude(typeof(Inline))]
    [XmlInclude(typeof(InlineContainer))]
    [XmlInclude(typeof(InstreamForeignObject))]
    [XmlInclude(typeof(Leader))]
    [XmlInclude(typeof(MultiToggle))]
    [XmlInclude(typeof(PageNumber))]
    [XmlInclude(typeof(PageNumberCitation))]
    [Serializable]
    [XmlType(Namespace = "http://www.w3.org/1999/XSL/Format")]
    [XmlRoot(Namespace = "http://www.w3.org/1999/XSL/Format",ElementName="generic-inline")]
    public abstract class GenericInline
        {
        protected GenericInline()
            {
            }

        public abstract String Serialize();
        }
    }
