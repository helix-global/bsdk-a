using System;
using System.Xml;
using System.Security.Cryptography;
using BinaryStudio.Security.Cryptography.Xml.Properties;

namespace BinaryStudio.Security.Cryptography.Xml
{
    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public abstract class EncryptedReference {
        private String m_uri;
        private String m_referenceType;
        private TransformChain m_transformChain;
        internal XmlElement m_cachedXml;

        protected EncryptedReference () : this (String.Empty, new TransformChain()) {
        }

        protected EncryptedReference (String uri) : this (uri, new TransformChain()) {
        }

        protected EncryptedReference (String uri, TransformChain transformChain) {
            TransformChain = transformChain;
            Uri = uri;
            m_cachedXml = null;
        }

        public String Uri {
            get { return m_uri; }
            set { 
                if (value == null)
                    throw new ArgumentNullException(SecurityResources.GetResourceString("Cryptography_Xml_UriRequired"));
                m_uri = value;
                m_cachedXml = null;
            }
        }

        public TransformChain TransformChain {
            get { 
                if (m_transformChain == null)
                    m_transformChain = new TransformChain();
                return m_transformChain; 
            }
            set {
                m_transformChain = value;
                m_cachedXml = null;
            }
        }

        public void AddTransform (Transform transform) {
            TransformChain.Add(transform);
        }

        protected String ReferenceType {
            get { return m_referenceType; }
            set {
                m_referenceType = value;
                m_cachedXml = null;
            }
        }

        protected internal Boolean CacheValid {
            get {
                return (m_cachedXml != null);
            }
        }

        public virtual XmlElement GetXml () {
            if (CacheValid) return m_cachedXml;

            var document = new XmlDocument {PreserveWhitespace = true};
            return GetXml(document);
        }

        internal XmlElement GetXml (XmlDocument document) {
            if (ReferenceType == null) 
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_ReferenceTypeRequired"));

            // Create the Reference
            var referenceElement = document.CreateElement(ReferenceType, EncryptedXml.XmlEncNamespaceUrl);
            if (!String.IsNullOrEmpty(m_uri))
                referenceElement.SetAttribute("URI", m_uri);

            // Add the transforms to the CipherReference
            if (TransformChain.Count > 0)
                referenceElement.AppendChild(TransformChain.GetXml(document, SignedXml.XmlDsigNamespaceUrl));

            return referenceElement;
        }

        public virtual void LoadXml (XmlElement value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            ReferenceType = value.LocalName;
            Uri = Utils.GetAttribute(value, "URI", EncryptedXml.XmlEncNamespaceUrl);

            // Transforms
            var nsm = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsm.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
            var transformsNode = value.SelectSingleNode("ds:Transforms", nsm);
            if (transformsNode != null)
                TransformChain.LoadXml(transformsNode as XmlElement);

            // cache the Xml
            m_cachedXml = value;
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public sealed class CipherReference : EncryptedReference {
        private Byte[] m_cipherValue;

        public CipherReference () : base () {
            ReferenceType = "CipherReference";
        }

        public CipherReference (String uri) : base(uri) {
            ReferenceType = "CipherReference";
        }

        public CipherReference (String uri, TransformChain transformChain) : base(uri, transformChain) {
            ReferenceType = "CipherReference";
        }

        // This method is used to cache results from resolved cipher references.
        internal Byte[] CipherValue {
            get {
                if (!CacheValid)
                    return null;
                return m_cipherValue;
            }
            set {
                m_cipherValue = value;
            }
        }

        public override XmlElement GetXml () {
            if (CacheValid) return m_cachedXml;

            var document = new XmlDocument {PreserveWhitespace = true};
            return GetXml(document);
        }

        internal new XmlElement GetXml (XmlDocument document) {
            if (ReferenceType == null) 
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_ReferenceTypeRequired"));

            // Create the Reference
            var referenceElement = document.CreateElement(ReferenceType, EncryptedXml.XmlEncNamespaceUrl);
            if (!String.IsNullOrEmpty(Uri))
                referenceElement.SetAttribute("URI", Uri);

            // Add the transforms to the CipherReference
            if (TransformChain.Count > 0) 
                referenceElement.AppendChild(TransformChain.GetXml(document, EncryptedXml.XmlEncNamespaceUrl));

            return referenceElement;
        }

        public override void LoadXml (XmlElement value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            ReferenceType = value.LocalName;
            var uri = Utils.GetAttribute(value, "URI", EncryptedXml.XmlEncNamespaceUrl);
            if (!Utils.GetSkipSignatureAttributeEnforcement() && uri == null) {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UriRequired"));
            }
            Uri = uri;

            // Transforms
            var nsm = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsm.AddNamespace("enc", EncryptedXml.XmlEncNamespaceUrl);
            var transformsNode = value.SelectSingleNode("enc:Transforms", nsm);
            if (transformsNode != null)
                TransformChain.LoadXml(transformsNode as XmlElement);

            // cache the Xml
            m_cachedXml = value;
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public sealed class DataReference : EncryptedReference {
        public DataReference () : base () {
            ReferenceType = "DataReference";
        }

        public DataReference (String uri) : base(uri) {
            ReferenceType = "DataReference";
        }

        public DataReference (String uri, TransformChain transformChain) : base(uri, transformChain) {
            ReferenceType = "DataReference";
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public sealed class KeyReference : EncryptedReference {
        public KeyReference () : base () {
            ReferenceType = "KeyReference";
        }

        public KeyReference (String uri) : base(uri) {
            ReferenceType = "KeyReference";
        }

        public KeyReference (String uri, TransformChain transformChain) : base(uri, transformChain) {
            ReferenceType = "KeyReference";
        }
    }
}
