using System;
using System.Collections;
using System.Xml;
using System.Security.Cryptography;
using BinaryStudio.Security.Cryptography.Xml.Properties;

namespace BinaryStudio.Security.Cryptography.Xml
{
    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public sealed class EncryptedKey : EncryptedType {
        private String m_recipient;
        private String m_carriedKeyName;
        private ReferenceList m_referenceList;

        public EncryptedKey () {}

        public String Recipient {
            get {
                // an unspecified value for an XmlAttribute is String.Empty
                if (m_recipient == null)
                    m_recipient = String.Empty;
                return m_recipient;
            }
            set {
                m_recipient = value;
                m_cachedXml = null;
            }
        }

       public String CarriedKeyName {
            get { return m_carriedKeyName; }
            set { 
                m_carriedKeyName = value; 
                m_cachedXml = null;
            }
        }

        public ReferenceList ReferenceList {
            get {
                if (m_referenceList == null)
                    m_referenceList = new ReferenceList();
                return m_referenceList;
            }
        }

        public void AddReference (DataReference dataReference) {
            ReferenceList.Add(dataReference);
        }

        public void AddReference (KeyReference keyReference) {
            ReferenceList.Add(keyReference);
        }

        public override void LoadXml (XmlElement value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var nsm = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsm.AddNamespace("enc", EncryptedXml.XmlEncNamespaceUrl);
            nsm.AddNamespace("ds",SignedXml.XmlDsigNamespaceUrl);

            Id = Utils.GetAttribute(value, "Id", EncryptedXml.XmlEncNamespaceUrl);
            Type = Utils.GetAttribute(value, "Type", EncryptedXml.XmlEncNamespaceUrl);
            MimeType = Utils.GetAttribute(value, "MimeType", EncryptedXml.XmlEncNamespaceUrl);
            Encoding = Utils.GetAttribute(value, "Encoding", EncryptedXml.XmlEncNamespaceUrl);
            Recipient = Utils.GetAttribute(value, "Recipient", EncryptedXml.XmlEncNamespaceUrl);

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

            // CarriedKeyName
            var carriedKeyNameNode = value.SelectSingleNode("enc:CarriedKeyName", nsm);
            if (carriedKeyNameNode != null) {
                CarriedKeyName = carriedKeyNameNode.InnerText;
            }

            // ReferenceList
            var referenceListNode = value.SelectSingleNode("enc:ReferenceList", nsm);
            if (referenceListNode != null) {
                // Select the DataReference elements inside the ReferenceList element
                var dataReferenceNodes = referenceListNode.SelectNodes("enc:DataReference", nsm);
                if (dataReferenceNodes != null) {
                    foreach (XmlNode node in dataReferenceNodes) {
                        var dr = new DataReference();
                        dr.LoadXml(node as XmlElement);
                        ReferenceList.Add(dr);
                    }
                }
                // Select the KeyReference elements inside the ReferenceList element
                var keyReferenceNodes = referenceListNode.SelectNodes("enc:KeyReference", nsm);
                if (keyReferenceNodes != null) {
                    foreach (XmlNode node in keyReferenceNodes) {
                        var kr = new KeyReference();
                        kr.LoadXml(node as XmlElement);
                        ReferenceList.Add(kr);
                    }
                }
            }

            // Save away the cached value
            m_cachedXml = value;
        }

        public override XmlElement GetXml () {
            if (CacheValid) return m_cachedXml;

            var document = new XmlDocument {PreserveWhitespace = true};
            return GetXml(document);
        }

        internal XmlElement GetXml (XmlDocument document) {
            // Create the EncryptedKey element
            var encryptedKeyElement = (XmlElement) document.CreateElement("EncryptedKey", EncryptedXml.XmlEncNamespaceUrl);

            // Deal with attributes
            if (!String.IsNullOrEmpty(Id))
                encryptedKeyElement.SetAttribute("Id", Id);
            if (!String.IsNullOrEmpty(Type))
                encryptedKeyElement.SetAttribute("Type", Type);
            if (!String.IsNullOrEmpty(MimeType))
                encryptedKeyElement.SetAttribute("MimeType", MimeType);
            if (!String.IsNullOrEmpty(Encoding))
                encryptedKeyElement.SetAttribute("Encoding", Encoding);
            if (!String.IsNullOrEmpty(Recipient))
                encryptedKeyElement.SetAttribute("Recipient", Recipient);

            // EncryptionMethod
            if (EncryptionMethod != null)
                encryptedKeyElement.AppendChild(EncryptionMethod.GetXml(document));

            // KeyInfo
            if (KeyInfo.Count > 0)
                encryptedKeyElement.AppendChild(KeyInfo.GetXml(document));

            // CipherData
            if (CipherData == null) 
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_MissingCipherData"));
            encryptedKeyElement.AppendChild(CipherData.GetXml(document));

            // EncryptionProperties
            if (EncryptionProperties.Count > 0) {
                var encryptionPropertiesElement = document.CreateElement("EncryptionProperties", EncryptedXml.XmlEncNamespaceUrl);
                for (var index = 0; index < EncryptionProperties.Count; index++) {
                    var ep = EncryptionProperties.Item(index);
                    encryptionPropertiesElement.AppendChild(ep.GetXml(document));
                }
                encryptedKeyElement.AppendChild(encryptionPropertiesElement);
            }

            // ReferenceList
            if (ReferenceList.Count > 0) {
                var referenceListElement = document.CreateElement("ReferenceList", EncryptedXml.XmlEncNamespaceUrl);
                for (var index = 0; index < ReferenceList.Count; index++) {
                    referenceListElement.AppendChild(ReferenceList[index].GetXml(document));
                }
                encryptedKeyElement.AppendChild(referenceListElement);
            }

            // CarriedKeyName
            if (CarriedKeyName != null) {
                var carriedKeyNameElement = (XmlElement) document.CreateElement("CarriedKeyName", EncryptedXml.XmlEncNamespaceUrl);
                var carriedKeyNameText = document.CreateTextNode(CarriedKeyName);
                carriedKeyNameElement.AppendChild(carriedKeyNameText);                
                encryptedKeyElement.AppendChild(carriedKeyNameElement);
            }

            return encryptedKeyElement;
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public sealed class ReferenceList : IList {
        private readonly ArrayList m_references;

        public ReferenceList() {
            m_references = new ArrayList();
        }

        public IEnumerator GetEnumerator() {
            return m_references.GetEnumerator();
        }

        public Int32 Count {
            get { return m_references.Count; }
        }

        public Int32 Add(Object value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!(value is DataReference) && !(value is KeyReference)) 
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_IncorrectObjectType"), nameof(value));

            return m_references.Add(value);
        }

        public void Clear() {
            m_references.Clear();
        }

        public Boolean Contains(Object value) {
            return m_references.Contains(value);
        }

        public Int32 IndexOf(Object value) {
            return m_references.IndexOf(value);
        }

        public void Insert(Int32 index, Object value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!(value is DataReference) && !(value is KeyReference)) 
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_IncorrectObjectType"), nameof(value));

            m_references.Insert(index, value);
        }

        public void Remove(Object value) {
            m_references.Remove(value);
        }

        public void RemoveAt(Int32 index) {
            m_references.RemoveAt(index);
        }

        public EncryptedReference Item(Int32 index) {
            return (EncryptedReference) m_references[index];
        }

        [System.Runtime.CompilerServices.IndexerName ("ItemOf")]
        public EncryptedReference this[Int32 index] {
            get {
                return Item(index);
            }
            set {
                ((IList) this)[index] = value;
            }
        }

        /// <internalonly/>
        Object IList.this[Int32 index] {
            get { return m_references[index]; }
            set {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (!(value is DataReference) && !(value is KeyReference)) 
                    throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_IncorrectObjectType"), nameof(value));

                m_references[index] = value;
            }
        }

        public void CopyTo(Array array, Int32 index) {
            m_references.CopyTo(array, index);
        }

        Boolean IList.IsFixedSize {
            get { return m_references.IsFixedSize; }
        }

        Boolean IList.IsReadOnly {
            get { return m_references.IsReadOnly; }
        }

        public Object SyncRoot {
            get { return m_references.SyncRoot; }
        }

        public Boolean IsSynchronized {
            get { return m_references.IsSynchronized; }
        }
    }
}
