using System;
using System.Xml;

namespace BinaryStudio.Security.Cryptography.Xml
{
    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class DataObject {
        private String m_id;
        private String m_mimeType;
        private String m_encoding;
        private CanonicalXmlNodeList m_elData;
        private XmlElement m_cachedXml;

        //
        // public constructors
        //

        public DataObject () {
            m_cachedXml = null;
            m_elData = new CanonicalXmlNodeList();
        }

        public DataObject (String id, String mimeType, String encoding, XmlElement data) {
            if (data == null) 
                throw new ArgumentNullException(nameof(data));

            m_id = id;
            m_mimeType = mimeType;
            m_encoding = encoding;
            m_elData = new CanonicalXmlNodeList {data};
            m_cachedXml = null;
        }

        //
        // public properties
        //

        public String Id {
            get { return m_id; }
            set { 
                m_id = value;
                m_cachedXml = null;
            }
        }

        public String MimeType {
            get { return m_mimeType; }
            set {
                m_mimeType = value; 
                m_cachedXml = null;
            }
        }

        public String Encoding {
            get { return m_encoding; }
            set {
                m_encoding = value;
                m_cachedXml = null;
            }
        }

        public XmlNodeList Data {
            get { return m_elData; }
            set {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                // Reset the node list
                m_elData = new CanonicalXmlNodeList();
                foreach (XmlNode node in value) {
                    m_elData.Add(node);
                }
                m_cachedXml = null;
            }
        }

        private Boolean CacheValid {
            get { 
                return(m_cachedXml != null);
            }
        }

        //
        // public methods
        //

        public XmlElement GetXml() {
            if (CacheValid) return(m_cachedXml);

            var document = new XmlDocument {PreserveWhitespace = true};
            return GetXml(document);
        }

        internal XmlElement GetXml (XmlDocument document) {
            var objectElement = document.CreateElement("Object", SignedXml.XmlDsigNamespaceUrl);

            if (!String.IsNullOrEmpty(m_id))
                objectElement.SetAttribute("Id", m_id);
            if (!String.IsNullOrEmpty(m_mimeType))
                objectElement.SetAttribute("MimeType", m_mimeType);
            if (!String.IsNullOrEmpty(m_encoding))
                objectElement.SetAttribute("Encoding", m_encoding);

            if (m_elData != null) {
                foreach (XmlNode node in m_elData) {
                    objectElement.AppendChild(document.ImportNode(node, true));
                }
            }

            return objectElement;
        }

        public void LoadXml (XmlElement value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            m_id = Utils.GetAttribute(value, "Id", SignedXml.XmlDsigNamespaceUrl);
            m_mimeType = Utils.GetAttribute(value, "MimeType", SignedXml.XmlDsigNamespaceUrl);
            m_encoding = Utils.GetAttribute(value, "Encoding", SignedXml.XmlDsigNamespaceUrl);

            foreach (XmlNode node in value.ChildNodes) {
                m_elData.Add(node);
            }

            // Save away the cached value
            m_cachedXml = value;
        }
    }
}
