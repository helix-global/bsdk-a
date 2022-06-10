using System;

namespace shell
    {
    internal class XmlTextDocumentSource : DocumentSource
        {
        public XmlTextDocumentSource(Object source)
            :base(source, DesiredDocumentType.XmlFile)
            {
            }
        }
    }