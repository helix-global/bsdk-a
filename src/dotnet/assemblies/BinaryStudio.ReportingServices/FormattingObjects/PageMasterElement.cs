using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    [XmlInclude(typeof(RepeatablePageMasterAlternatives))]
    [XmlInclude(typeof(RepeatablePageMasterReference))]
    [XmlInclude(typeof(SinglePageMasterReference))]
    [Serializable]
    [XmlType(Namespace = "http://www.w3.org/1999/XSL/Format")]
    [XmlRoot(Namespace = "http://www.w3.org/1999/XSL/Format",ElementName="page-master-element")]
    public abstract class PageMasterElement
        {
        protected PageMasterElement()
            {
            }

        public abstract String Serialize();
        }
    }
