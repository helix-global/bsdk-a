using System;
using System.Collections.Generic;
using System.Xml;

namespace BinaryStudio.PlatformUI.Shell
    {
    public interface ICustomXmlSerializer
        {
        Boolean ExcludeLocalizable { get; set; }

        Boolean ExcludeOptional { get; set; }

        Object Content { get; }

        void WriteXmlAttributes(XmlWriter writer);

        IEnumerable<KeyValuePair<String, Object>> GetNonContentPropertyValues();
        }
    }