using System;
using System.Xml;
using System.Security.Cryptography;
using BinaryStudio.Security.Cryptography.Xml.Properties;

namespace BinaryStudio.Security.Cryptography.Xml
{
    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public sealed class EncryptedData : EncryptedType {
        public override void LoadXml(XmlElement value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var nsm = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsm.AddNamespace("enc", EncryptedXml.XmlEncNamespaceUrl);
            nsm.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);

            Id = Utils.GetAttribute(value, "Id", EncryptedXml.XmlEncNamespaceUrl);
            Type = Utils.GetAttribute(value, "Type", EncryptedXml.XmlEncNamespaceUrl);
            MimeType = Utils.GetAttribute(value, "MimeType", EncryptedXml.XmlEncNamespaceUrl);
            Encoding = Utils.GetAttribute(value, "Encoding", EncryptedXml.XmlEncNamespaceUrl);

            var encryptionMethodNode = value.SelectSingleNode("enc:EncryptionMethod", nsm);

            // EncryptionMethod
            EncryptionMethod = new EncryptionMethod();
            if (encryptionMethodNode != null)
                EncryptionMethod.LoadXml(encryptionMethodNode as XmlElement);

            // Key Info
            KeyInfo = new KeyInfo();
            var keyInfoNode = value.SelectSingleNode("ds:KeyInfo", nsm);
            if (keyInfoNode != null)
                KeyInfo.LoadXml(keyInfoNode as XmlElement);

            // CipherData
            var cipherDataNode = value.SelectSingleNode("enc:CipherData", nsm);
            if (cipherDataNode == null) 
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_MissingCipherData"));

            CipherData = new CipherData();
            CipherData.LoadXml(cipherDataNode as XmlElement);

            // EncryptionProperties
            var encryptionPropertiesNode = value.SelectSingleNode("enc:EncryptionProperties", nsm);
            if (encryptionPropertiesNode != null) {
                // Select the EncryptionProperty elements inside the EncryptionProperties element
                var encryptionPropertyNodes = encryptionPropertiesNode.SelectNodes("enc:EncryptionProperty", nsm);
                if (encryptionPropertyNodes != null) {
                    foreach (XmlNode node in encryptionPropertyNodes) {
                        var ep = new EncryptionProperty();
                        ep.LoadXml(node as XmlElement);
                        EncryptionProperties.Add(ep);
                    }
                }
            }

            // Save away the cached value
            m_cachedXml = value;
        }

        public override XmlElement GetXml() {
            if (CacheValid) return(m_cachedXml);

            var document = new XmlDocument {PreserveWhitespace = true};
            return GetXml(document);
        }

        internal XmlElement GetXml (XmlDocument document) {
            // Create the EncryptedData element
            var encryptedDataElement = (XmlElement) document.CreateElement("EncryptedData", EncryptedXml.XmlEncNamespaceUrl);

            // Deal with attributes
            if (!String.IsNullOrEmpty(Id))
                encryptedDataElement.SetAttribute("Id", Id);
            if (!String.IsNullOrEmpty(Type))
                encryptedDataElement.SetAttribute("Type", Type);
            if (!String.IsNullOrEmpty(MimeType))
                encryptedDataElement.SetAttribute("MimeType", MimeType);
            if (!String.IsNullOrEmpty(Encoding))
                encryptedDataElement.SetAttribute("Encoding", Encoding);

            // EncryptionMethod
            if (EncryptionMethod != null)
                encryptedDataElement.AppendChild(EncryptionMethod.GetXml(document));

            // KeyInfo
            if (KeyInfo.Count > 0)
                encryptedDataElement.AppendChild(KeyInfo.GetXml(document));

            // CipherData is required.
            if (CipherData == null) 
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_MissingCipherData"));
            encryptedDataElement.AppendChild(CipherData.GetXml(document));

            // EncryptionProperties
            if (EncryptionProperties.Count > 0) {
                var encryptionPropertiesElement = document.CreateElement("EncryptionProperties", EncryptedXml.XmlEncNamespaceUrl);
                for (var index = 0; index < EncryptionProperties.Count; index++) {
                    var ep = EncryptionProperties.Item(index);
                    encryptionPropertiesElement.AppendChild(ep.GetXml(document));
                }
                encryptedDataElement.AppendChild(encryptionPropertiesElement);
            }
            return encryptedDataElement;
        }
    }
}
