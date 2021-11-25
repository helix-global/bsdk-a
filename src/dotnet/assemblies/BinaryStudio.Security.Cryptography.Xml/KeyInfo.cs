using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Security;
using BinaryStudio.Security.Cryptography.Xml.Properties;

namespace BinaryStudio.Security.Cryptography.Xml
{
    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class KeyInfo : IEnumerable {
        private ArrayList m_KeyInfoClauses;

        //
        // public constructors
        //

        public KeyInfo() {
            m_KeyInfoClauses = new ArrayList();
        }

        //
        // public properties
        //

        public String Id { get; set; }

        public XmlElement GetXml() {
            var xmlDocument = new XmlDocument {PreserveWhitespace = true};
            return GetXml(xmlDocument);
        }

        internal XmlElement GetXml (XmlDocument xmlDocument) {
            // Create the KeyInfo element itself
            var keyInfoElement = xmlDocument.CreateElement("KeyInfo", SignedXml.XmlDsigNamespaceUrl);
            if (!String.IsNullOrEmpty(Id)) {
                keyInfoElement.SetAttribute("Id", Id);
            }

            // Add all the clauses that go underneath it
            for (var i = 0; i < m_KeyInfoClauses.Count; ++i) {
                var xmlElement = ((KeyInfoClause) m_KeyInfoClauses[i]).GetXml(xmlDocument);
                if (xmlElement != null) {
                    keyInfoElement.AppendChild(xmlElement);
                }
            }
            return keyInfoElement;
        }

        public void LoadXml(XmlElement value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var keyInfoElement = value;
            Id = Utils.GetAttribute(keyInfoElement, "Id", SignedXml.XmlDsigNamespaceUrl);
            if (!Utils.VerifyAttributes(keyInfoElement, "Id"))
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "KeyInfo");

            var child = keyInfoElement.FirstChild;
            while (child != null) {
                if (child is XmlElement elem) {
                    // Create the right type of KeyInfoClause; we use a combination of the namespace and tag name (local name)
                    var kicString = elem.NamespaceURI + " " + elem.LocalName;
                    // Special-case handling for KeyValue -- we have to go one level deeper
                    if (kicString == "http://www.w3.org/2000/09/xmldsig# KeyValue") {
                        if (!Utils.VerifyAttributes(elem, (String[])null)) {
                            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "KeyInfo/KeyValue");
                        }

                        var nodeList2 = elem.ChildNodes;
                        foreach (XmlNode node2 in nodeList2) {
                            var elem2 = node2 as XmlElement;
                            if (elem2 != null) {
                                kicString += "/" + elem2.LocalName;
                                break;
                            }
                        }
                    }
                    var keyInfoClause = Utils.CreateFromName<KeyInfoClause>(kicString);
                    // if we don't know what kind of KeyInfoClause we're looking at, use a generic KeyInfoNode:
                    if (keyInfoClause == null)
                        keyInfoClause = new KeyInfoNode();

                    // Ask the create clause to fill itself with the corresponding XML
                    keyInfoClause.LoadXml(elem);
                    // Add it to our list of KeyInfoClauses
                    AddClause(keyInfoClause);
                }
                child = child.NextSibling;
            }
        }

        public Int32 Count {
            get { return m_KeyInfoClauses.Count; }
        }

        //
        // public constructors
        //

        public void AddClause(KeyInfoClause clause) {
            m_KeyInfoClauses.Add(clause);
        }

        public IEnumerator GetEnumerator() {
            return m_KeyInfoClauses.GetEnumerator();
        }

        public IEnumerator GetEnumerator(Type requestedObjectType) {
            var requestedList = new ArrayList();

            Object tempObj;
            var tempEnum = m_KeyInfoClauses.GetEnumerator();

            while(tempEnum.MoveNext()) {
                tempObj = tempEnum.Current;
                if (requestedObjectType.Equals(tempObj.GetType()))
                    requestedList.Add(tempObj);
            }

            return requestedList.GetEnumerator();
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public abstract class KeyInfoClause {
        //
        // protected constructors
        //

        protected KeyInfoClause () {}

        //
        // public methods
        //

        public abstract XmlElement GetXml();
        internal virtual XmlElement GetXml (XmlDocument xmlDocument) {
            var keyInfo = GetXml();
            return (XmlElement) xmlDocument.ImportNode(keyInfo, true);
        }

        public abstract void LoadXml(XmlElement element);
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class KeyInfoName : KeyInfoClause {
        //
        // public constructors
        //

        public KeyInfoName () : this (null) {}

        public KeyInfoName (String keyName) {
            Value = keyName;
        }

        //
        // public properties
        //

        public String Value { get; set; }

        //
        // public methods
        //

        public override XmlElement GetXml() {
            var xmlDocument = new XmlDocument {PreserveWhitespace = true};
            return GetXml(xmlDocument);
        }

        internal override XmlElement GetXml (XmlDocument xmlDocument) {
            var nameElement = xmlDocument.CreateElement("KeyName", SignedXml.XmlDsigNamespaceUrl);
            nameElement.AppendChild(xmlDocument.CreateTextNode(Value));
            return nameElement;
        }

        public override void LoadXml(XmlElement value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            var nameElement = value;
            Value = nameElement.InnerText.Trim();
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class DSAKeyValue : KeyInfoClause {
        //
        // public constructors
        //

        public DSAKeyValue () {
            Key = DSA.Create();
        }

        public DSAKeyValue (DSA key) {
            Key = key;
        }

        //
        // public properties
        //

        public DSA Key { get; set; }

        //
        // public methods
        //

        public override XmlElement GetXml() {
            var xmlDocument = new XmlDocument {PreserveWhitespace = true};
            return GetXml(xmlDocument);
        }

        internal override XmlElement GetXml (XmlDocument xmlDocument) {
            var dsaParams = Key.ExportParameters(false);

            var keyValueElement = xmlDocument.CreateElement("KeyValue", SignedXml.XmlDsigNamespaceUrl);
            var dsaKeyValueElement = xmlDocument.CreateElement("DSAKeyValue", SignedXml.XmlDsigNamespaceUrl);

            var pElement = xmlDocument.CreateElement("P", SignedXml.XmlDsigNamespaceUrl);
            pElement.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(dsaParams.P)));
            dsaKeyValueElement.AppendChild(pElement);

            var qElement = xmlDocument.CreateElement("Q", SignedXml.XmlDsigNamespaceUrl);
            qElement.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(dsaParams.Q)));
            dsaKeyValueElement.AppendChild(qElement);

            var gElement = xmlDocument.CreateElement("G", SignedXml.XmlDsigNamespaceUrl);
            gElement.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(dsaParams.G)));
            dsaKeyValueElement.AppendChild(gElement);

            var yElement = xmlDocument.CreateElement("Y", SignedXml.XmlDsigNamespaceUrl);
            yElement.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(dsaParams.Y)));
            dsaKeyValueElement.AppendChild(yElement);

            // Add optional components if present
            if (dsaParams.J != null) {
                var jElement = xmlDocument.CreateElement("J", SignedXml.XmlDsigNamespaceUrl);
                jElement.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(dsaParams.J)));
                dsaKeyValueElement.AppendChild(jElement);
            }

            if (dsaParams.Seed != null) {  // note we assume counter is correct if Seed is present
                var seedElement = xmlDocument.CreateElement("Seed", SignedXml.XmlDsigNamespaceUrl);
                seedElement.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(dsaParams.Seed)));
                dsaKeyValueElement.AppendChild(seedElement);

                var counterElement = xmlDocument.CreateElement("PgenCounter", SignedXml.XmlDsigNamespaceUrl);
                counterElement.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(Utils.ConvertIntToByteArray(dsaParams.Counter))));
                dsaKeyValueElement.AppendChild(counterElement);
            }

            keyValueElement.AppendChild(dsaKeyValueElement);

            return keyValueElement;
        }

        public override void LoadXml(XmlElement value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            // Get the XML string 
            Key.FromXmlString(value.OuterXml);
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class RSAKeyValue : KeyInfoClause {
        //
        // public constructors
        //

        public RSAKeyValue () {
            Key = RSA.Create();
        }

        public RSAKeyValue (RSA key) {
            Key = key;
        }

        //
        // public properties
        //

        public RSA Key { get; set; }

        //
        // public methods
        //

        public override XmlElement GetXml() {
            var xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            return GetXml(xmlDocument);
        }

        internal override XmlElement GetXml (XmlDocument xmlDocument) {
            var rsaParams = Key.ExportParameters(false);

            var keyValueElement = xmlDocument.CreateElement("KeyValue", SignedXml.XmlDsigNamespaceUrl);
            var rsaKeyValueElement = xmlDocument.CreateElement("RSAKeyValue", SignedXml.XmlDsigNamespaceUrl);

            var modulusElement = xmlDocument.CreateElement("Modulus", SignedXml.XmlDsigNamespaceUrl);
            modulusElement.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(rsaParams.Modulus)));
            rsaKeyValueElement.AppendChild(modulusElement);

            var exponentElement = xmlDocument.CreateElement("Exponent", SignedXml.XmlDsigNamespaceUrl);
            exponentElement.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(rsaParams.Exponent)));
            rsaKeyValueElement.AppendChild(exponentElement);

            keyValueElement.AppendChild(rsaKeyValueElement);

            return keyValueElement;
        }

        public override void LoadXml(XmlElement value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            // Get the XML string 
            Key.FromXmlString(value.OuterXml);
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class KeyInfoRetrievalMethod : KeyInfoClause {
        //
        // public constructors
        //

        public KeyInfoRetrievalMethod () {}

        public KeyInfoRetrievalMethod (String strUri) {
            Uri = strUri;
        }

        public KeyInfoRetrievalMethod (String strUri, String typeName) {
            Uri = strUri;
            Type = typeName;
        }

        //
        // public properties
        //

        public String Uri { get; set; }

        [ComVisible(false)]
        public String Type { get; set; }

        public override XmlElement GetXml() {
            var xmlDocument =  new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            return GetXml(xmlDocument);
        }

        internal override XmlElement GetXml (XmlDocument xmlDocument) {
            // Create the actual element
            var retrievalMethodElement = xmlDocument.CreateElement("RetrievalMethod",SignedXml.XmlDsigNamespaceUrl);

            if (!String.IsNullOrEmpty(Uri))
                retrievalMethodElement.SetAttribute("URI", Uri);
            if (!String.IsNullOrEmpty(Type))
                retrievalMethodElement.SetAttribute("Type", Type);

            return retrievalMethodElement;
        }

        public override void LoadXml (XmlElement value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var retrievalMethodElement = value;
            Uri = Utils.GetAttribute(value, "URI", SignedXml.XmlDsigNamespaceUrl);
            Type = Utils.GetAttribute(value, "Type", SignedXml.XmlDsigNamespaceUrl);
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class KeyInfoEncryptedKey : KeyInfoClause {
        public KeyInfoEncryptedKey () {}
        
        public KeyInfoEncryptedKey (EncryptedKey encryptedKey) {
            EncryptedKey = encryptedKey;
        }

        public EncryptedKey EncryptedKey { get; set; }

        public override XmlElement GetXml() {
            if (EncryptedKey == null) 
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "KeyInfoEncryptedKey");
            return EncryptedKey.GetXml();
        }

        internal override XmlElement GetXml (XmlDocument xmlDocument) {
            if (EncryptedKey == null) 
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "KeyInfoEncryptedKey");
            return EncryptedKey.GetXml(xmlDocument);
        }

        public override void LoadXml(XmlElement value) {
            EncryptedKey = new EncryptedKey();
            EncryptedKey.LoadXml(value);
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public struct X509IssuerSerial {
        internal X509IssuerSerial (String issuerName, String serialNumber) {
            if (issuerName == null || issuerName.Length == 0)
                throw new ArgumentException(SecurityResources.GetResourceString("Arg_EmptyOrNullString"), nameof(issuerName));
            if (serialNumber == null || serialNumber.Length == 0)
                throw new ArgumentException(SecurityResources.GetResourceString("Arg_EmptyOrNullString"), nameof(serialNumber));
            IssuerName = issuerName;
            SerialNumber = serialNumber;
        }

        
        public String IssuerName { get; set; }
        public String SerialNumber { get; set; }
        }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class KeyInfoX509Data : KeyInfoClause {
        internal const Int32 CERT_E_CHAINING              = unchecked((Int32) 0x800B010A);

        // An array of certificates representing the certificate chain 
        // An array of issuer serial structs
        // An array of SKIs
        // An array of subject names
        // A raw byte data representing a certificate revocation list

        internal static Boolean IsSelfSigned (X509Chain chain) {
            var elements = chain.ChainElements;
            if (elements.Count != 1)
                return false;
            var certificate = elements[0].Certificate;
            if (String.Compare(certificate.SubjectName.Name, certificate.IssuerName.Name, StringComparison.OrdinalIgnoreCase) == 0)
                return true;
            return false;
        }

        //
        // public constructors
        //

        public KeyInfoX509Data () {}

        public KeyInfoX509Data (Byte[] rgbCert) {
            var certificate = new X509Certificate2(rgbCert);
            AddCertificate(certificate);
        }

        public KeyInfoX509Data (X509Certificate cert) {
            AddCertificate(cert);
        }

        [SecuritySafeCritical]
        public KeyInfoX509Data (X509Certificate cert, X509IncludeOption includeOption) {
            if (cert == null)
                throw new ArgumentNullException(nameof(cert));

            var certificate = new X509Certificate2(cert);
            X509ChainElementCollection elements;
            X509Chain chain = null;
            switch (includeOption) {
            case X509IncludeOption.ExcludeRoot:
                // Build the certificate chain
                chain = new X509Chain();
                chain.Build(certificate);

                // Can't honor the option if we only have a partial chain.
                if ((chain.ChainStatus.Length > 0) && 
                    ((chain.ChainStatus[0].Status & X509ChainStatusFlags.PartialChain) == X509ChainStatusFlags.PartialChain))
                    throw new CryptographicException(CERT_E_CHAINING);

                elements = (X509ChainElementCollection) chain.ChainElements;
                for (var index = 0; index < (IsSelfSigned(chain) ? 1 : elements.Count - 1); index++) {
                    AddCertificate(elements[index].Certificate);
                }
                break;
            case X509IncludeOption.EndCertOnly:
                AddCertificate(certificate);
                break;
            case X509IncludeOption.WholeChain:
                // Build the certificate chain
                chain = new X509Chain();
                chain.Build(certificate);

                // Can't honor the option if we only have a partial chain.
                if ((chain.ChainStatus.Length > 0) && 
                    ((chain.ChainStatus[0].Status & X509ChainStatusFlags.PartialChain) == X509ChainStatusFlags.PartialChain))
                    throw new CryptographicException(CERT_E_CHAINING);

                elements = (X509ChainElementCollection) chain.ChainElements;
                foreach (var element in elements) {
                    AddCertificate(element.Certificate);
                }
                break;
            }
        }

        //
        // public properties
        //

        public ArrayList Certificates { get; private set; }

        public void AddCertificate (X509Certificate certificate) {
            if (certificate == null)
                throw new ArgumentNullException(nameof(certificate));

            if (Certificates == null)
                Certificates = new ArrayList();

            var x509 = new X509Certificate2(certificate);
            Certificates.Add(x509);
        }

        public ArrayList SubjectKeyIds { get; private set; }

        public void AddSubjectKeyId(Byte[] subjectKeyId) {
            if (SubjectKeyIds == null)
                SubjectKeyIds = new ArrayList();
            SubjectKeyIds.Add(subjectKeyId);
        }

        internal static Byte HexToByte (Char val) {
            if (val <= '9' && val >= '0')
                return (Byte) (val - '0');
            else if (val >= 'a' && val <= 'f')
                return (Byte) ((val - 'a') + 10);
            else if (val >= 'A' && val <= 'F')
                return (Byte) ((val - 'A') + 10);
            else
                return 0xFF;
        }

        internal static Byte[] DecodeHexString (String s) {
            var hexString = Utils.DiscardWhiteSpaces(s);
            var cbHex = (UInt32) hexString.Length / 2;
            var hex = new Byte[cbHex];
            var i = 0;
            for (var index = 0; index < cbHex; index++) {
                hex[index] = (Byte) ((HexToByte(hexString[i]) << 4) | HexToByte(hexString[i+1]));
                i += 2;
            }
            return hex;
        }

        [ComVisible(false)]
        public void AddSubjectKeyId(String subjectKeyId) {
            if (SubjectKeyIds == null)
                SubjectKeyIds = new ArrayList();
            SubjectKeyIds.Add(DecodeHexString(subjectKeyId));
        }

        public ArrayList SubjectNames { get; private set; }

        public void AddSubjectName(String subjectName) {
            if (SubjectNames == null)
                SubjectNames = new ArrayList();
            SubjectNames.Add(subjectName);
        }

        public ArrayList IssuerSerials { get; private set; }

        public void AddIssuerSerial(String issuerName, String serialNumber) {
            var h = new BigInt();
            h.FromHexadecimal(serialNumber);
            if (IssuerSerials == null)
                IssuerSerials = new ArrayList();
            IssuerSerials.Add(new X509IssuerSerial(issuerName, h.ToDecimal()));
        }

        // When we load an X509Data from Xml, we know the serial number is in decimal representation.
        internal void InternalAddIssuerSerial(String issuerName, String serialNumber) {
            if (IssuerSerials == null)
                IssuerSerials = new ArrayList();
            IssuerSerials.Add(new X509IssuerSerial(issuerName, serialNumber));
        }

        public Byte[] CRL { get; set; }

        //
        // private methods
        //

        private void Clear() {
            CRL = null;
            if (SubjectKeyIds != null) SubjectKeyIds.Clear();
            if (SubjectNames != null) SubjectNames.Clear();
            if (IssuerSerials != null) IssuerSerials.Clear();
            if (Certificates != null) Certificates.Clear();
        }

        //
        // public methods
        //

        public override XmlElement GetXml() {
            var xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            return GetXml(xmlDocument);
        }

        internal override XmlElement GetXml (XmlDocument xmlDocument) {
            var x509DataElement = xmlDocument.CreateElement("X509Data", SignedXml.XmlDsigNamespaceUrl);

            if (IssuerSerials != null) {
                foreach(X509IssuerSerial issuerSerial in IssuerSerials) {
                    var issuerSerialElement = xmlDocument.CreateElement("X509IssuerSerial", SignedXml.XmlDsigNamespaceUrl);
                    var issuerNameElement = xmlDocument.CreateElement("X509IssuerName", SignedXml.XmlDsigNamespaceUrl);
                    issuerNameElement.AppendChild(xmlDocument.CreateTextNode(issuerSerial.IssuerName));
                    issuerSerialElement.AppendChild(issuerNameElement);
                    var serialNumberElement = xmlDocument.CreateElement("X509SerialNumber", SignedXml.XmlDsigNamespaceUrl);
                    serialNumberElement.AppendChild(xmlDocument.CreateTextNode(issuerSerial.SerialNumber));
                    issuerSerialElement.AppendChild(serialNumberElement);
                    x509DataElement.AppendChild(issuerSerialElement);
                }
            }

            if (SubjectKeyIds != null) {
                foreach(Byte[] subjectKeyId in SubjectKeyIds) {
                    var subjectKeyIdElement = xmlDocument.CreateElement("X509SKI", SignedXml.XmlDsigNamespaceUrl);
                    subjectKeyIdElement.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(subjectKeyId)));
                    x509DataElement.AppendChild(subjectKeyIdElement);
                }
            }

            if (SubjectNames != null) {
                foreach(String subjectName in SubjectNames) {
                    var subjectNameElement = xmlDocument.CreateElement("X509SubjectName", SignedXml.XmlDsigNamespaceUrl);
                    subjectNameElement.AppendChild(xmlDocument.CreateTextNode(subjectName));
                    x509DataElement.AppendChild(subjectNameElement);
                }
            }

            if (Certificates != null) {
                foreach(X509Certificate certificate in Certificates) {
                    var x509Element = xmlDocument.CreateElement("X509Certificate", SignedXml.XmlDsigNamespaceUrl);
                    x509Element.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(certificate.GetRawCertData())));
                    x509DataElement.AppendChild(x509Element);
                }
            }

            if (CRL != null) {
                var crlElement = xmlDocument.CreateElement("X509CRL", SignedXml.XmlDsigNamespaceUrl);
                crlElement.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(CRL)));
                x509DataElement.AppendChild(crlElement);
            }

            return x509DataElement;
        }

        public override void LoadXml(XmlElement element) {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var nsm = new XmlNamespaceManager(element.OwnerDocument.NameTable);
            nsm.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);

            var x509IssuerSerialNodes = element.SelectNodes("ds:X509IssuerSerial", nsm);
            var x509SKINodes = element.SelectNodes("ds:X509SKI", nsm);
            var x509SubjectNameNodes = element.SelectNodes("ds:X509SubjectName", nsm);
            var x509CertificateNodes = element.SelectNodes("ds:X509Certificate", nsm);
            var x509CRLNodes = element.SelectNodes("ds:X509CRL", nsm);

            if ((x509CRLNodes.Count == 0 && x509IssuerSerialNodes.Count == 0 && x509SKINodes.Count == 0
                    && x509SubjectNameNodes.Count == 0 && x509CertificateNodes.Count == 0)) // Bad X509Data tag, or Empty tag
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "X509Data");

            // Flush anything in the lists
            Clear();

            if (x509CRLNodes.Count != 0)
                CRL = Convert.FromBase64String(Utils.DiscardWhiteSpaces(x509CRLNodes.Item(0).InnerText));

            foreach (XmlNode issuerSerialNode in x509IssuerSerialNodes) {
                var x509IssuerNameNode = issuerSerialNode.SelectSingleNode("ds:X509IssuerName", nsm);
                var x509SerialNumberNode = issuerSerialNode.SelectSingleNode("ds:X509SerialNumber", nsm);
                if (x509IssuerNameNode == null || x509SerialNumberNode == null)
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "IssuerSerial");
                InternalAddIssuerSerial(x509IssuerNameNode.InnerText.Trim(), x509SerialNumberNode.InnerText.Trim());
            }

            foreach (XmlNode node in x509SKINodes) {
                AddSubjectKeyId(Convert.FromBase64String(Utils.DiscardWhiteSpaces(node.InnerText)));
            }

            foreach (XmlNode node in x509SubjectNameNodes) {
                AddSubjectName(node.InnerText.Trim());
            }

            foreach (XmlNode node in x509CertificateNodes) {
                AddCertificate(new X509Certificate2(Convert.FromBase64String(Utils.DiscardWhiteSpaces(node.InnerText))));
            }
        }
    }

    // This is for generic, unknown nodes
    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class KeyInfoNode : KeyInfoClause {
        //
        // public constructors
        //

        public KeyInfoNode() {}

        public KeyInfoNode(XmlElement node) {
            Value = node;
        }

        //
        // public properties
        //

        public XmlElement Value { get; set; }

        //
        // public methods
        //

        public override XmlElement GetXml() {
            var xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            return GetXml(xmlDocument);
        }

        internal override XmlElement GetXml (XmlDocument xmlDocument) {
            return xmlDocument.ImportNode(Value, true) as XmlElement;
        }

        public override void LoadXml(XmlElement value) {
            Value = value; 
        }
    }
}
