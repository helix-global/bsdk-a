using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace BinaryStudio.ReportingServices.FormattingObjects
    {
    [XmlInclude(typeof(Block))]
    [XmlInclude(typeof(BlockContainer))]
    [XmlInclude(typeof(ListBlock))]
    [XmlInclude(typeof(Table))]
    [XmlInclude(typeof(TableAndCaption))]
    [Serializable]
    [XmlType(Namespace = "http://www.w3.org/1999/XSL/Format")]
    [XmlRoot(Namespace = "http://www.w3.org/1999/XSL/Format",ElementName="generic-block")]
    public abstract class GenericBlock
        {
        protected GenericBlock()
            {
            }

        public abstract String Serialize();
        }
    }
