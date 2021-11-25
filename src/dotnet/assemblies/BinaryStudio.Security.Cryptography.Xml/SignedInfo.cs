using System.Security.Cryptography;
using BinaryStudio.Security.Cryptography.Xml.Properties;

namespace BinaryStudio.Security.Cryptography.Xml
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Globalization;

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class SignedInfo : ICollection {
        private String m_id;
        private String m_canonicalizationMethod;
        private String m_signatureMethod;
        private String m_signatureLength;
        private XmlElement m_cachedXml;
        private Transform m_canonicalizationMethodTransform;

        internal SignedXml SignedXml { get; set; }

        public SignedInfo() {
            References = new ArrayList();
        }

        public IEnumerator GetEnumerator() {
            throw new NotSupportedException();
        } 

        public void CopyTo(Array array, Int32 index) {
            throw new NotSupportedException();
        }

        public Int32 Count {
            get { throw new NotSupportedException(); }
        }

        public Boolean IsReadOnly {
            get { throw new NotSupportedException(); }
        }

        public Boolean IsSynchronized {
            get { throw new NotSupportedException(); }
        }

        public Object SyncRoot {
            get { throw new NotSupportedException(); }
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

        public String CanonicalizationMethod {
            get {
                // Default the canonicalization method to C14N
                if (m_canonicalizationMethod == null)
                    return SignedXml.XmlDsigC14NTransformUrl;
                return m_canonicalizationMethod; 
            }
            set {
                m_canonicalizationMethod = value;
                m_cachedXml = null;
            }
        }

        [ComVisible(false)]
        public Transform CanonicalizationMethodObject {
            get {
                if (m_canonicalizationMethodTransform == null) {
                    m_canonicalizationMethodTransform = Utils.CreateFromName<Transform>(CanonicalizationMethod);
                    if (m_canonicalizationMethodTransform == null)
                        throw new CryptographicException(String.Format(CultureInfo.CurrentCulture, SecurityResources.GetResourceString("Cryptography_Xml_CreateTransformFailed"), CanonicalizationMethod));
                    m_canonicalizationMethodTransform.SignedXml = SignedXml;
                    m_canonicalizationMethodTransform.Reference = null;
                }
                return m_canonicalizationMethodTransform;
            }
        }

        public String SignatureMethod {
            get { return m_signatureMethod; }
            set {
                m_signatureMethod = value;
                m_cachedXml = null;
            }
        }

        public String SignatureLength {
            get { return m_signatureLength; }
            set {
                m_signatureLength = value;
                m_cachedXml = null;
            }
        }

        public ArrayList References { get; }

        internal Boolean CacheValid {
            get {
                if (m_cachedXml == null) return false;
                // now check all the references
                foreach (Reference reference in References) {
                    if (!reference.CacheValid) return false;
                }
                return true;
            }
        }

        //
        // public methods
        //

        public XmlElement GetXml() {
            if (CacheValid) return m_cachedXml;

            var document = new XmlDocument();
            document.PreserveWhitespace = true;
            return GetXml(document);
        }

        internal XmlElement GetXml (XmlDocument document) {
            // Create the root element
            var signedInfoElement = document.CreateElement("SignedInfo", SignedXml.XmlDsigNamespaceUrl);
            if (!String.IsNullOrEmpty(m_id))
                signedInfoElement.SetAttribute("Id", m_id);

            // Add the canonicalization method, defaults to SignedXml.XmlDsigNamespaceUrl
            var canonicalizationMethodElement = CanonicalizationMethodObject.GetXml(document, "CanonicalizationMethod");
            signedInfoElement.AppendChild(canonicalizationMethodElement);

            // Add the signature method
            if (String.IsNullOrEmpty(m_signatureMethod))
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_SignatureMethodRequired"));

            var signatureMethodElement = document.CreateElement("SignatureMethod", SignedXml.XmlDsigNamespaceUrl);
            signatureMethodElement.SetAttribute("Algorithm", m_signatureMethod);
            // Add HMACOutputLength tag if we have one
            if (m_signatureLength != null) {
                var hmacLengthElement = document.CreateElement(null, "HMACOutputLength", SignedXml.XmlDsigNamespaceUrl);
                var outputLength = document.CreateTextNode(m_signatureLength);
                hmacLengthElement.AppendChild(outputLength);
                signatureMethodElement.AppendChild(hmacLengthElement);
            }

            signedInfoElement.AppendChild(signatureMethodElement);

            // Add the references
            if (References.Count == 0)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_ReferenceElementRequired"));

            for (var i = 0; i < References.Count; ++i) {
                var reference = (Reference)References[i];
                signedInfoElement.AppendChild(reference.GetXml(document));
            }

            return signedInfoElement;
        }

        public void LoadXml(XmlElement value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            // SignedInfo
            var signedInfoElement = value;
            if (!signedInfoElement.LocalName.Equals("SignedInfo"))
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "SignedInfo");

            var nsm = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsm.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
            var expectedChildNodes = 0;

            // Id attribute -- optional
            m_id = Utils.GetAttribute(signedInfoElement, "Id", SignedXml.XmlDsigNamespaceUrl);
            if (!Utils.VerifyAttributes(signedInfoElement, "Id"))
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "SignedInfo");

            // CanonicalizationMethod -- must be present
            var canonicalizationMethodNodes = signedInfoElement.SelectNodes("ds:CanonicalizationMethod", nsm);
            if (canonicalizationMethodNodes == null || canonicalizationMethodNodes.Count == 0 || (!Utils.GetAllowAdditionalSignatureNodes() && canonicalizationMethodNodes.Count > 1))
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "SignedInfo/CanonicalizationMethod");
            var canonicalizationMethodElement = canonicalizationMethodNodes.Item(0) as XmlElement;
            expectedChildNodes += canonicalizationMethodNodes.Count;
            m_canonicalizationMethod = Utils.GetAttribute(canonicalizationMethodElement, "Algorithm", SignedXml.XmlDsigNamespaceUrl);
            if ((m_canonicalizationMethod == null && !Utils.GetSkipSignatureAttributeEnforcement()) || !Utils.VerifyAttributes(canonicalizationMethodElement, "Algorithm"))
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "SignedInfo/CanonicalizationMethod");
            m_canonicalizationMethodTransform = null;
            if (canonicalizationMethodElement.ChildNodes.Count > 0)
                CanonicalizationMethodObject.LoadInnerXml(canonicalizationMethodElement.ChildNodes);

            // SignatureMethod -- must be present
            var signatureMethodNodes = signedInfoElement.SelectNodes("ds:SignatureMethod", nsm);
            if (signatureMethodNodes == null || signatureMethodNodes.Count == 0 || (!Utils.GetAllowAdditionalSignatureNodes() && signatureMethodNodes.Count > 1))
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "SignedInfo/SignatureMethod");
            var signatureMethodElement = signatureMethodNodes.Item(0) as XmlElement;
            expectedChildNodes += signatureMethodNodes.Count;
            m_signatureMethod = Utils.GetAttribute(signatureMethodElement, "Algorithm", SignedXml.XmlDsigNamespaceUrl);
            if ((m_signatureMethod == null && !Utils.GetSkipSignatureAttributeEnforcement()) || !Utils.VerifyAttributes(signatureMethodElement, "Algorithm"))
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "SignedInfo/SignatureMethod");

            // Now get the output length if we are using a MAC algorithm
            var signatureLengthElement = signatureMethodElement.SelectSingleNode("ds:HMACOutputLength", nsm) as XmlElement;
            if (signatureLengthElement != null) 
                m_signatureLength = signatureLengthElement.InnerXml;

            // flush out any reference that was there
            References.Clear();

            // Reference - 0 or more
            var referenceNodes = signedInfoElement.SelectNodes("ds:Reference", nsm);
            if (referenceNodes != null) {
                if (referenceNodes.Count > Utils.GetMaxReferencesPerSignedInfo()) {
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "SignedInfo/Reference");
                }
                foreach(XmlNode node in referenceNodes) {
                    var referenceElement = node as XmlElement;
                    var reference = new Reference();
                    AddReference(reference);
                    reference.LoadXml(referenceElement);
                }
                expectedChildNodes += referenceNodes.Count;
            }

            // Verify that there aren't any extra nodes that aren't allowed
            if (!Utils.GetAllowAdditionalSignatureNodes() && (signedInfoElement.SelectNodes("*").Count != expectedChildNodes)) {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "SignedInfo");
            }

            // Save away the cached value
            m_cachedXml = signedInfoElement;
        }

        public void AddReference (Reference reference) {
            if (reference == null)
                throw new ArgumentNullException(nameof(reference));

            reference.SignedXml = SignedXml;
            References.Add(reference);
        }
    }
}
