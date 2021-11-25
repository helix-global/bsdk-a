using System.Security.Cryptography;
using BinaryStudio.Security.Cryptography.Xml.Properties;

namespace BinaryStudio.Security.Cryptography.Xml
{
    using System;
    using System.Collections;
    using System.Xml;

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class Signature {
        private SignedInfo m_signedInfo;
        private String m_signatureValueId;
        private KeyInfo m_keyInfo;

        internal SignedXml SignedXml { get; set; }

        //
        // public constructors
        //

        public Signature() {
            ObjectList = new ArrayList();
            ReferencedItems = new CanonicalXmlNodeList();
        }

        //
        // public properties
        //

        public String Id { get; set; }

        public SignedInfo SignedInfo {
            get { return m_signedInfo; }
            set { 
                m_signedInfo = value;
                if (SignedXml != null && m_signedInfo != null)
                    m_signedInfo.SignedXml = SignedXml;
            }
        }

        public Byte[] SignatureValue { get; set; }

        public KeyInfo KeyInfo {
            get {
                if (m_keyInfo == null)
                    m_keyInfo = new KeyInfo();
                return m_keyInfo;
            }
            set { m_keyInfo = value; }
        }

        public IList ObjectList { get; set; }

        internal CanonicalXmlNodeList ReferencedItems { get; }

        //
        // public methods
        //

        public XmlElement GetXml() {
            var document = new XmlDocument();
            document.PreserveWhitespace = true;
            return GetXml(document);
        }

        internal XmlElement GetXml (XmlDocument document) {
            // Create the Signature
            var signatureElement = (XmlElement)document.CreateElement("Signature", SignedXml.XmlDsigNamespaceUrl);
            if (!String.IsNullOrEmpty(Id))
                signatureElement.SetAttribute("Id", Id);

            // Add the SignedInfo
            if (m_signedInfo == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_SignedInfoRequired"));

            signatureElement.AppendChild(m_signedInfo.GetXml(document));

            // Add the SignatureValue
            if (SignatureValue == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_SignatureValueRequired"));

            var signatureValueElement = document.CreateElement("SignatureValue", SignedXml.XmlDsigNamespaceUrl);
            signatureValueElement.AppendChild(document.CreateTextNode(Convert.ToBase64String(SignatureValue)));
            if (!String.IsNullOrEmpty(m_signatureValueId))
                signatureValueElement.SetAttribute("Id", m_signatureValueId);
            signatureElement.AppendChild(signatureValueElement);

            // Add the KeyInfo
            if (KeyInfo.Count > 0)
                signatureElement.AppendChild(KeyInfo.GetXml(document));

            // Add the Objects
            foreach (var obj in ObjectList) {
                var dataObj = obj as DataObject;
                if (dataObj != null) {
                    signatureElement.AppendChild(dataObj.GetXml(document));
                }
            }

            return signatureElement;
        }

        public void LoadXml(XmlElement value) {
             // Make sure we don't get passed null
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            // Signature
            var signatureElement = value;
            if (!signatureElement.LocalName.Equals("Signature"))
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "Signature");

            // Attributes
            Id = Utils.GetAttribute(signatureElement, "Id", SignedXml.XmlDsigNamespaceUrl);
            if (!Utils.VerifyAttributes(signatureElement, "Id"))
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "Signature");

            var nsm = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsm.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
            var expectedChildNodes = 0;

            // SignedInfo
            var signedInfoNodes = signatureElement.SelectNodes("ds:SignedInfo", nsm);
            if (signedInfoNodes == null || signedInfoNodes.Count == 0 || (!Utils.GetAllowAdditionalSignatureNodes() && signedInfoNodes.Count > 1))
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"),"SignedInfo");
            var signedInfoElement = signedInfoNodes[0] as XmlElement;
            expectedChildNodes += signedInfoNodes.Count;

            SignedInfo = new SignedInfo();
            SignedInfo.LoadXml(signedInfoElement);

            // SignatureValue
            var signatureValueNodes = signatureElement.SelectNodes("ds:SignatureValue", nsm);
            if (signatureValueNodes == null || signatureValueNodes.Count == 0 || (!Utils.GetAllowAdditionalSignatureNodes() && signatureValueNodes.Count > 1))
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"),"SignatureValue");
            var signatureValueElement = signatureValueNodes[0] as XmlElement;
            expectedChildNodes += signatureValueNodes.Count;
            SignatureValue = Convert.FromBase64String(Utils.DiscardWhiteSpaces(signatureValueElement.InnerText));
            m_signatureValueId = Utils.GetAttribute(signatureValueElement, "Id", SignedXml.XmlDsigNamespaceUrl);
            if (!Utils.VerifyAttributes(signatureValueElement, "Id"))
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "SignatureValue");

            // KeyInfo - optional single element
            var keyInfoNodes = signatureElement.SelectNodes("ds:KeyInfo", nsm);
            m_keyInfo = new KeyInfo();
            if (keyInfoNodes != null) {
                if (!Utils.GetAllowAdditionalSignatureNodes() && keyInfoNodes.Count > 1) {
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "KeyInfo");
                }
                foreach(XmlNode node in keyInfoNodes) {
                    var keyInfoElement = node as XmlElement;
                    if (keyInfoElement != null)
                        m_keyInfo.LoadXml(keyInfoElement);
                }
                expectedChildNodes += keyInfoNodes.Count;
            }

            // Object - zero or more elements allowed
            var objectNodes = signatureElement.SelectNodes("ds:Object", nsm);
            ObjectList.Clear();
            if (objectNodes != null) {
                foreach(XmlNode node in objectNodes) {
                    var objectElement = node as XmlElement;
                    if (objectElement != null) {
                        var dataObj = new DataObject();
                        dataObj.LoadXml(objectElement);
                        ObjectList.Add(dataObj);
                    }
                }
                expectedChildNodes += objectNodes.Count;
            }

            // Select all elements that have Id attributes
            var nodeList = signatureElement.SelectNodes("//*[@Id]", nsm);
            if (nodeList != null) {
                foreach (XmlNode node in nodeList) {
                    ReferencedItems.Add(node);
                }
            }

            // Verify that there aren't any extra nodes that aren't allowed
            if (!Utils.GetAllowAdditionalSignatureNodes() && (signatureElement.SelectNodes("*").Count != expectedChildNodes)) {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "Signature");
            }
        }

        public void AddObject(DataObject dataObject) {
            ObjectList.Add(dataObject);
        }
    }
}

