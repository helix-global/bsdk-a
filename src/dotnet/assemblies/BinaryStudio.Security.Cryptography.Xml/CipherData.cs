using System;
using System.Xml;
using System.Security.Cryptography;
using BinaryStudio.Security.Cryptography.Xml.Properties;

namespace BinaryStudio.Security.Cryptography.Xml
{
    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public sealed class CipherData {
        private XmlElement m_cachedXml;
        private CipherReference m_cipherReference;
        private Byte[] m_cipherValue;

        public CipherData () {}

        public CipherData (Byte[] cipherValue) {
            CipherValue = cipherValue;
        }

        public CipherData (CipherReference cipherReference) {
            CipherReference = cipherReference;
        }

        private Boolean CacheValid {
            get { 
                return (m_cachedXml != null);
            }
        }

        public CipherReference CipherReference {
            get { return m_cipherReference; }
            set {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (CipherValue != null)
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_CipherValueElementRequired"));

                m_cipherReference = value;
                m_cachedXml = null;
            }
        }

        public Byte[] CipherValue {
            get { return m_cipherValue; }
            set { 
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (CipherReference != null)
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_CipherValueElementRequired"));

                m_cipherValue = (Byte[]) value.Clone(); 
                m_cachedXml = null;
            }
        }

        public XmlElement GetXml () {
            if (CacheValid) return m_cachedXml;

            var document = new XmlDocument {PreserveWhitespace = true};
            return GetXml(document);
        }

        internal XmlElement GetXml (XmlDocument document) {
            // Create the CipherData element
            var cipherDataElement = (XmlElement)document.CreateElement("CipherData", EncryptedXml.XmlEncNamespaceUrl);
            if (CipherValue != null) {
                var cipherValueElement = document.CreateElement("CipherValue", EncryptedXml.XmlEncNamespaceUrl);
                cipherValueElement.AppendChild(document.CreateTextNode(Convert.ToBase64String(CipherValue)));
                cipherDataElement.AppendChild(cipherValueElement);
            } else {
                // No CipherValue specified, see if there is a CipherReference
                if (CipherReference == null)
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_CipherValueElementRequired"));
                cipherDataElement.AppendChild(CipherReference.GetXml(document));
            }
            return cipherDataElement;
        }

        public void LoadXml (XmlElement value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var nsm = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsm.AddNamespace("enc", EncryptedXml.XmlEncNamespaceUrl);

            var cipherValueNode = value.SelectSingleNode("enc:CipherValue", nsm);
            var cipherReferenceNode = value.SelectSingleNode("enc:CipherReference", nsm);
            if (cipherValueNode != null) {
                if (cipherReferenceNode != null) 
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_CipherValueElementRequired"));
                m_cipherValue = Convert.FromBase64String(Utils.DiscardWhiteSpaces(cipherValueNode.InnerText));
            } else if (cipherReferenceNode != null) {
                m_cipherReference = new CipherReference();
                m_cipherReference.LoadXml((XmlElement) cipherReferenceNode);
            } else {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_CipherValueElementRequired"));
            }

            // Save away the cached value
            m_cachedXml = value;
        }
    }
}
