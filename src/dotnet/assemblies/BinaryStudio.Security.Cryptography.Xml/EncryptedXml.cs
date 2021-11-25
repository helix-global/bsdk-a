using BinaryStudio.Security.Cryptography.Xml.Properties;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Xml;

namespace BinaryStudio.Security.Cryptography.Xml
{
    /// <summary>
    /// This exception helps catch the signed XML recursion limit error.
    /// This is being caught in the SignedXml class while computing the
    /// hash. ComputeHash can throw different kind of exceptions.
    /// This unique exception helps catch the recursion limit issue.
    /// </summary>
    [Serializable]
    internal class CryptoSignedXmlRecursionException : XmlException {
        public CryptoSignedXmlRecursionException() : base() { }
        public CryptoSignedXmlRecursionException(String message) : base(message) { }
        public CryptoSignedXmlRecursionException(String message, Exception inner) : base(message, inner) { }
        // A constructor is needed for serialization when an 
        // exception propagates from a remoting server to the client.  
        protected CryptoSignedXmlRecursionException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) { }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class EncryptedXml {

        //
        // public constant Url identifiers used within the XML Encryption classes
        //

        public const String XmlEncNamespaceUrl = "http://www.w3.org/2001/04/xmlenc#";
        public const String XmlEncElementUrl = "http://www.w3.org/2001/04/xmlenc#Element";
        public const String XmlEncElementContentUrl = "http://www.w3.org/2001/04/xmlenc#Content";
        public const String XmlEncEncryptedKeyUrl = "http://www.w3.org/2001/04/xmlenc#EncryptedKey";

        //
        // Symmetric Block Encryption
        //

        public const String XmlEncDESUrl = "http://www.w3.org/2001/04/xmlenc#des-cbc";
        public const String XmlEncTripleDESUrl = "http://www.w3.org/2001/04/xmlenc#tripledes-cbc";
        public const String XmlEncAES128Url = "http://www.w3.org/2001/04/xmlenc#aes128-cbc";
        public const String XmlEncAES256Url = "http://www.w3.org/2001/04/xmlenc#aes256-cbc";
        public const String XmlEncAES192Url = "http://www.w3.org/2001/04/xmlenc#aes192-cbc";

        //
        // Key Transport
        //

        public const String XmlEncRSA15Url = "http://www.w3.org/2001/04/xmlenc#rsa-1_5";
        public const String XmlEncRSAOAEPUrl = "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p";

        //
        // Symmetric Key Wrap
        //

        public const String XmlEncTripleDESKeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-tripledes";
        public const String XmlEncAES128KeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-aes128";
        public const String XmlEncAES256KeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-aes256";
        public const String XmlEncAES192KeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-aes192";

        //
        // Message Digest
        //

        public const String XmlEncSHA256Url = "http://www.w3.org/2001/04/xmlenc#sha256";
        public const String XmlEncSHA512Url = "http://www.w3.org/2001/04/xmlenc#sha512";

        //
        // private members
        //

        private XmlDocument m_document;

        // hash table defining the key name mapping
        private const Int32 m_capacity = 4; // 4 is a reasonable capacity for
                                          // the key name mapping hash table
        private Hashtable m_keyNameMapping;
        private String m_recipient;
        private Int32 m_xmlDsigSearchDepthCounter;

        //
        // public constructors
        //
        public EncryptedXml () : this (new XmlDocument()) {}

        public EncryptedXml (XmlDocument document) : this (document, null) {}

        public EncryptedXml (XmlDocument document, Evidence evidence) {
            m_document = document;
            DocumentEvidence = evidence;
            Resolver = null;
            // set the default padding to ISO-10126
            Padding = PaddingMode.ISO10126;
            // set the default cipher mode to CBC
            Mode = CipherMode.CBC;
            // By default the encoding is going to be UTF8
            Encoding = Encoding.UTF8;
            m_keyNameMapping = new Hashtable(m_capacity);
            XmlDSigSearchDepth = Utils.GetXmlDsigSearchDepth();
        }

        /// <summary>
        /// This mentod validates the m_xmlDsigSearchDepthCounter counter
        /// if the counter is over the limit defined by admin or developer.
        /// </summary>
        /// <returns>returns true if the limit has reached otherwise false</returns>
        private Boolean IsOverXmlDsigRecursionLimit() {
            if (m_xmlDsigSearchDepthCounter > XmlDSigSearchDepth) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets / Sets the max limit for recursive search of encryption key in signed XML
        /// </summary>
        public Int32 XmlDSigSearchDepth { get; set; }

        //
        // public properties
        //

        // The evidence of the document being loaded: will be used to resolve external URIs
        public Evidence DocumentEvidence { get; set; }

        // The resolver to use for external entities
        public XmlResolver Resolver { get; set; }

        // The padding to be used. XML Encryption uses ISO 10126
        // but it's nice to provide a way to extend this to include other forms of paddings
        public PaddingMode Padding { get; set; }

        // The cipher mode to be used. XML Encryption uses CBC padding
        // but it's nice to provide a way to extend this to include other cipher modes
        public CipherMode Mode { get; set; }

        // The encoding of the XML document
        public Encoding Encoding { get; set; }

        // This is used to specify the EncryptedKey elements that should be considered
        // when an EncyptedData references an EncryptedKey using a CarriedKeyName and Recipient
        public String Recipient {
            get {
                // an unspecified value for an XmlAttribute is String.Empty
                if (m_recipient == null)
                    m_recipient = String.Empty;
                return m_recipient;
            }
            set { m_recipient = value; }
        }

        //
        // private methods
        //

        private Byte[] GetCipherValue (CipherData cipherData) {
            if (cipherData == null)
                throw new ArgumentNullException(nameof(cipherData));

            WebResponse response = null;
            Stream inputStream = null;

            if (cipherData.CipherValue != null) {
                return cipherData.CipherValue;
            } else if (cipherData.CipherReference != null) {
                if (cipherData.CipherReference.CipherValue != null)
                    return cipherData.CipherReference.CipherValue;
                Stream decInputStream = null;
                // See if the CipherReference is a local URI
                if (!Utils.GetLeaveCipherValueUnchecked() && cipherData.CipherReference.Uri == null){
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UriNotSupported"));
                }
                if (cipherData.CipherReference.Uri.Length == 0) {
                    // self referenced Uri
                    var baseUri = (m_document == null ? null : m_document.BaseURI);
                    var tc = cipherData.CipherReference.TransformChain;
                    if (!Utils.GetLeaveCipherValueUnchecked() && tc == null) {
                        throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UriNotSupported"));
                    }
                    decInputStream = tc.TransformToOctetStream(m_document, Resolver, baseUri);
                } else if (cipherData.CipherReference.Uri[0] == '#') {
                    var idref = Utils.ExtractIdFromLocalUri(cipherData.CipherReference.Uri);
                    // Serialize 
                    if (Utils.GetLeaveCipherValueUnchecked()) {
                        inputStream = new MemoryStream(Encoding.GetBytes(GetIdElement(m_document, idref).OuterXml));
                    }
                    else {
                        var idElem = GetIdElement(m_document, idref);
                        if (idElem == null || idElem.OuterXml == null) {
                            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UriNotSupported"));
                        }
                        inputStream = new MemoryStream(Encoding.GetBytes(idElem.OuterXml));
                    }

                    var baseUri = (m_document == null ? null : m_document.BaseURI);
                    var tc = cipherData.CipherReference.TransformChain;
                    if (!Utils.GetLeaveCipherValueUnchecked() && tc == null) {
                        throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UriNotSupported"));
                    }
                    decInputStream = tc.TransformToOctetStream(inputStream, Resolver, baseUri);
                } else {
                    DownloadCipherValue(cipherData, out inputStream, out decInputStream, out response);
                }
                // read the output stream into a memory stream
                Byte[] cipherValue;
                using (var ms = new MemoryStream()) {
                    Utils.Pump(decInputStream, ms);
                    cipherValue = ms.ToArray();
                    // Close the stream and return
                    if (response != null)
                        response.Close();
                    if (inputStream != null)
                        inputStream.Close();
                    decInputStream.Close();
                }

                // cache the cipher value for Perf reasons in case we call this routine twice
                cipherData.CipherReference.CipherValue = cipherValue;
                return cipherValue;
            }

            // Throw a CryptographicException if we were unable to retrieve the cipher data.
            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_MissingCipherData"));
        }

        private void DownloadCipherValue (CipherData cipherData, out Stream inputStream, out Stream decInputStream, out WebResponse response) {
            // maybe a network stream, make sure we allow just what is needed!!
            var ps = SecurityManager.GetStandardSandbox(DocumentEvidence);
            ps.PermitOnly();
            var request = WebRequest.Create(cipherData.CipherReference.Uri);
            if (request == null) 
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UriNotResolved"), cipherData.CipherReference.Uri);
            response = request.GetResponse();
            if (response == null) 
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UriNotResolved"), cipherData.CipherReference.Uri);
            inputStream = response.GetResponseStream();
            if (inputStream == null) 
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UriNotResolved"), cipherData.CipherReference.Uri);
            var tc = cipherData.CipherReference.TransformChain;
            decInputStream = tc.TransformToOctetStream(inputStream, Resolver, cipherData.CipherReference.Uri);
        }

        //
        // public virtual methods
        //

        // This describes how the application wants to associate id references to elements
        public virtual XmlElement GetIdElement (XmlDocument document, String idValue) {
            return SignedXml.DefaultGetIdElement(document, idValue);
        }

        // default behaviour is to look for the IV in the CipherValue
        public virtual Byte[] GetDecryptionIV (EncryptedData encryptedData, String symmetricAlgorithmUri) {
            if (encryptedData == null)
                throw new ArgumentNullException(nameof(encryptedData));

            var initBytesSize = 0;
            // If the Uri is not provided by the application, try to get it from the EncryptionMethod
            if (symmetricAlgorithmUri == null) {
                if (encryptedData.EncryptionMethod == null)
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_MissingAlgorithm"));
                symmetricAlgorithmUri = encryptedData.EncryptionMethod.KeyAlgorithm;
            }
            switch (symmetricAlgorithmUri) {
            case XmlEncDESUrl:
            case XmlEncTripleDESUrl:
                initBytesSize = 8;
                break;
            case XmlEncAES128Url:
            case XmlEncAES192Url:
            case XmlEncAES256Url:
                initBytesSize = 16;
                break;
            default:
                // The Uri is not supported.
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UriNotSupported"));
            }
            var IV = new Byte[initBytesSize];
            var cipherValue = GetCipherValue(encryptedData.CipherData);
            Buffer.BlockCopy(cipherValue, 0, IV, 0, IV.Length);
            return IV;
        }

        // default behaviour is to look for keys defined by an EncryptedKey clause
        // either directly or through a KeyInfoRetrievalMethod, and key names in the key mapping
        public virtual SymmetricAlgorithm GetDecryptionKey (EncryptedData encryptedData, String symmetricAlgorithmUri) {
            if (encryptedData == null)
                throw new ArgumentNullException(nameof(encryptedData));

            if (encryptedData.KeyInfo == null)
                return null;
            var keyInfoEnum = encryptedData.KeyInfo.GetEnumerator();
            KeyInfoRetrievalMethod kiRetrievalMethod;
            KeyInfoName kiName;
            KeyInfoEncryptedKey kiEncKey;
            EncryptedKey ek = null;

            while (keyInfoEnum.MoveNext()) {
                kiName = keyInfoEnum.Current as KeyInfoName;
                if (kiName != null) {
                    // Get the decryption key from the key mapping
                    var keyName = kiName.Value;
                    if ((SymmetricAlgorithm) m_keyNameMapping[keyName] != null) 
                        return (SymmetricAlgorithm) m_keyNameMapping[keyName];
                    // try to get it from a CarriedKeyName
                    var nsm = new XmlNamespaceManager(m_document.NameTable);
                    nsm.AddNamespace("enc", XmlEncNamespaceUrl);
                    var encryptedKeyList = m_document.SelectNodes("//enc:EncryptedKey", nsm);
                    if (encryptedKeyList != null) {
                        foreach (XmlNode encryptedKeyNode in encryptedKeyList) {
                            var encryptedKeyElement = encryptedKeyNode as XmlElement;
                            var ek1 = new EncryptedKey();
                            ek1.LoadXml(encryptedKeyElement);
                            if (ek1.CarriedKeyName == keyName && ek1.Recipient == Recipient) {
                                ek = ek1;
                                break;
                            }
                        }
                    }
                    break;
                }
                kiRetrievalMethod = keyInfoEnum.Current as KeyInfoRetrievalMethod;
                if (kiRetrievalMethod != null) { 
                    var idref = Utils.ExtractIdFromLocalUri(kiRetrievalMethod.Uri);
                    ek = new EncryptedKey();
                    ek.LoadXml(GetIdElement(m_document, idref));
                    break;
                }
                kiEncKey = keyInfoEnum.Current as KeyInfoEncryptedKey;
                if (kiEncKey != null) {
                    ek = kiEncKey.EncryptedKey;
                    break;
                }
            }

            // if we have an EncryptedKey, decrypt to get the symmetric key
            if (ek != null) {
                // now process the EncryptedKey, loop recursively 
                // If the Uri is not provided by the application, try to get it from the EncryptionMethod
                if (symmetricAlgorithmUri == null) {
                    if (encryptedData.EncryptionMethod == null)
                        throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_MissingAlgorithm"));
                    symmetricAlgorithmUri = encryptedData.EncryptionMethod.KeyAlgorithm;
                }
                var key = DecryptEncryptedKey(ek);
                if (key == null)
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_MissingDecryptionKey"));

                var symAlg = Utils.CreateFromName<SymmetricAlgorithm>(symmetricAlgorithmUri);
                if (symAlg == null) {
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_MissingAlgorithm"));
                }
                symAlg.Key = key;
                return symAlg;
            }
            return null;
        }

        // Try to decrypt the EncryptedKey given the key mapping
        public virtual Byte[] DecryptEncryptedKey (EncryptedKey encryptedKey) {
            if (encryptedKey == null)
                throw new ArgumentNullException(nameof(encryptedKey));
            if (encryptedKey.KeyInfo == null)
                return null;

            var keyInfoEnum = encryptedKey.KeyInfo.GetEnumerator();
            KeyInfoName kiName;
            KeyInfoX509Data kiX509Data;
            KeyInfoRetrievalMethod kiRetrievalMethod;
            KeyInfoEncryptedKey kiEncKey;
            EncryptedKey ek = null;
            var fOAEP = false;

            while (keyInfoEnum.MoveNext()) {
                kiName = keyInfoEnum.Current as KeyInfoName;
                if (kiName != null) {
                    // Get the decryption key from the key mapping
                    var keyName = kiName.Value;
                    var kek = m_keyNameMapping[keyName];
                    if (kek != null) {
                        if (!Utils.GetLeaveCipherValueUnchecked() && (encryptedKey.CipherData == null || encryptedKey.CipherData.CipherValue == null)) {
                            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_MissingAlgorithm"));
                        }
                        // kek is either a SymmetricAlgorithm or an RSA key, otherwise, we wouldn't be able to insert it in the hash table
                        if (kek is SymmetricAlgorithm)
                            return DecryptKey(encryptedKey.CipherData.CipherValue, (SymmetricAlgorithm) kek);

                        // kek is an RSA key: get fOAEP from the algorithm, default to false
                        fOAEP = (encryptedKey.EncryptionMethod != null && encryptedKey.EncryptionMethod.KeyAlgorithm == XmlEncRSAOAEPUrl);
                        return DecryptKey(encryptedKey.CipherData.CipherValue, (RSA) kek, fOAEP);
                    }
                    break;
                }
                kiX509Data = keyInfoEnum.Current as KeyInfoX509Data;
                if (kiX509Data != null) {
                    var collection = Utils.BuildBagOfCerts(kiX509Data, CertUsageType.Decryption);
                    foreach (var certificate in collection) {
                        var privateKey = certificate.PrivateKey as RSA;
                        if (privateKey != null) {
                            if (!Utils.GetLeaveCipherValueUnchecked() && (encryptedKey.CipherData == null || encryptedKey.CipherData.CipherValue == null)) {
                                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_MissingAlgorithm"));
                            }
                            fOAEP = (encryptedKey.EncryptionMethod != null && encryptedKey.EncryptionMethod.KeyAlgorithm == XmlEncRSAOAEPUrl);
                            return DecryptKey(encryptedKey.CipherData.CipherValue, privateKey, fOAEP);
                        }
                    }
                    break;
                }
                kiRetrievalMethod = keyInfoEnum.Current as KeyInfoRetrievalMethod;
                if (kiRetrievalMethod != null) {
                    var idref = Utils.ExtractIdFromLocalUri(kiRetrievalMethod.Uri);
                    ek = new EncryptedKey();
                    ek.LoadXml(GetIdElement(m_document, idref));
                    try {
                        //Following checks if XML dsig processing is in loop and within the limit defined by machine
                        // admin or developer. Once the recursion depth crosses the defined limit it will throw exception.
                        m_xmlDsigSearchDepthCounter++;
                        if (IsOverXmlDsigRecursionLimit()) {
                            //Throw exception once recursion limit is hit. 
                            throw new CryptoSignedXmlRecursionException();
                        }
                        else {
                            return DecryptEncryptedKey(ek);
                        }
                    }
                    finally {
                        m_xmlDsigSearchDepthCounter--;
                    }
                }
                kiEncKey = keyInfoEnum.Current as KeyInfoEncryptedKey;
                if (kiEncKey != null) {
                    ek = kiEncKey.EncryptedKey;
                    // recursively process EncryptedKey elements
                    var encryptionKey = DecryptEncryptedKey(ek);
                    if (encryptionKey != null) {
                        // this is a symmetric algorithm for sure
                        var symAlg = Utils.CreateFromName<SymmetricAlgorithm>(encryptedKey.EncryptionMethod.KeyAlgorithm);
                        if (symAlg == null) {
                            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_MissingAlgorithm"));
                        }
                        symAlg.Key = encryptionKey;
                        if (!Utils.GetLeaveCipherValueUnchecked()  && (encryptedKey.CipherData == null || encryptedKey.CipherData.CipherValue == null)) {
                            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_MissingAlgorithm"));
                        }
                        return DecryptKey(encryptedKey.CipherData.CipherValue, symAlg);
                    }
                }
            }
            return null;
        }

        //
        // public methods
        //

        // defines a key name mapping. Default behaviour is to require the key object
        // to be an RSA key or a SymmetricAlgorithm
        public void AddKeyNameMapping (String keyName, Object keyObject) {
            if (keyName == null)
                throw new ArgumentNullException(nameof(keyName));
            if (keyObject == null)
                throw new ArgumentNullException(nameof(keyObject));

            if (!(keyObject is SymmetricAlgorithm) && !(keyObject is RSA)) 
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_NotSupportedCryptographicTransform"));
            m_keyNameMapping.Add(keyName, keyObject);
        }

        public void ClearKeyNameMappings () {
            m_keyNameMapping.Clear();
        }

        // Encrypts the given element with the certificate specified. The certificate is added as
        // an X509Data KeyInfo to an EncryptedKey (AES session key) generated randomly.
        public EncryptedData Encrypt (XmlElement inputElement, X509Certificate2 certificate) {
            if (inputElement == null)
                throw new ArgumentNullException(nameof(inputElement));
            if (certificate == null)
                throw new ArgumentNullException(nameof(certificate));

            var rsaPublicKey = certificate.PublicKey.Key as RSA;
            if (rsaPublicKey == null)
                throw new NotSupportedException(SecurityResources.GetResourceString("NotSupported_KeyAlgorithm"));
    
            // Create the EncryptedData object, using an AES-256 session key by default.
            var ed = new EncryptedData();
            ed.Type = XmlEncElementUrl;
            ed.EncryptionMethod = new EncryptionMethod(XmlEncAES256Url);

            // Include the certificate in the EncryptedKey KeyInfo.
            var ek = new EncryptedKey();
            ek.EncryptionMethod = new EncryptionMethod(XmlEncRSA15Url);
            ek.KeyInfo.AddClause(new KeyInfoX509Data(certificate));

            // Create a random AES session key and encrypt it with the public key associated with the certificate.
            using (var aes = Aes.Create()) {
                ek.CipherData.CipherValue = EncryptKey(aes.Key, rsaPublicKey, false);

                // Encrypt the input element with the random session key that we've created above.
                var kek = new KeyInfoEncryptedKey(ek);
                ed.KeyInfo.AddClause(kek);
                ed.CipherData.CipherValue = EncryptData(inputElement, aes, false);
            }

            return ed;
        }

        // Encrypts the given element with the key name specified. A corresponding key name mapping 
        // has to be defined before calling this method. The key name is added as
        // a KeyNameInfo KeyInfo to an EncryptedKey (AES session key) generated randomly.
        public EncryptedData Encrypt (XmlElement inputElement, String keyName) {
            if (inputElement == null)
                throw new ArgumentNullException(nameof(inputElement));
            if (keyName == null)
                throw new ArgumentNullException(nameof(keyName));

            Object encryptionKey = null;
            if (m_keyNameMapping != null)
                encryptionKey = m_keyNameMapping[keyName];

            if (encryptionKey == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_MissingEncryptionKey"));

            // kek is either a SymmetricAlgorithm or an RSA key, otherwise, we wouldn't be able to insert it in the hash table
            var symKey = encryptionKey as SymmetricAlgorithm;
            var rsa = encryptionKey as RSA;

            // Create the EncryptedData object, using an AES-256 session key by default.
            var ed = new EncryptedData();
            ed.Type = XmlEncElementUrl;
            ed.EncryptionMethod = new EncryptionMethod(XmlEncAES256Url);

            // Include the key name in the EncryptedKey KeyInfo.
            String encryptionMethod = null;
            if (symKey == null) {
                encryptionMethod = XmlEncRSA15Url;
            } else if (symKey is TripleDES) {
                // CMS Triple DES Key Wrap
                encryptionMethod = XmlEncTripleDESKeyWrapUrl;
            } else if (symKey is Rijndael || symKey is Aes) {
                // FIPS AES Key Wrap
                switch (symKey.KeySize) {
                case 128:
                    encryptionMethod = XmlEncAES128KeyWrapUrl;
                    break;
                case 192:
                    encryptionMethod = XmlEncAES192KeyWrapUrl;
                    break;
                case 256:
                    encryptionMethod = XmlEncAES256KeyWrapUrl;
                    break;
                }
            } else {
                // throw an exception if the transform is not in the previous categories
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_NotSupportedCryptographicTransform"));
            }
            var ek = new EncryptedKey();
            ek.EncryptionMethod = new EncryptionMethod(encryptionMethod);
            ek.KeyInfo.AddClause(new KeyInfoName(keyName));

            // Create a random AES session key and encrypt it with the public key associated with the certificate.
            using (var aes = Aes.Create()) {
                ek.CipherData.CipherValue = symKey == null ?
                    EncryptKey(aes.Key, rsa, false) :
                    EncryptKey(aes.Key, symKey);

                // Encrypt the input element with the random session key that we've created above.
                var kek = new KeyInfoEncryptedKey(ek);
                ed.KeyInfo.AddClause(kek);
                ed.CipherData.CipherValue = EncryptData(inputElement, aes, false);
            }

            return ed;
        }

        // decrypts the document using the defined key mapping in GetDecryptionKey
        // The behaviour of this method can be extended because GetDecryptionKey is virtual
        // the document is decrypted in place
        public void DecryptDocument () {
            // Look for all EncryptedData elements and decrypt them
            var nsm = new XmlNamespaceManager(m_document.NameTable);
            nsm.AddNamespace("enc", XmlEncNamespaceUrl);
            var encryptedDataList = m_document.SelectNodes("//enc:EncryptedData", nsm);
            if (encryptedDataList != null) {
                foreach (XmlNode encryptedDataNode in encryptedDataList) {
                    var encryptedDataElement = encryptedDataNode as XmlElement;
                    var ed = new EncryptedData();
                    ed.LoadXml(encryptedDataElement);
                    var symAlg = GetDecryptionKey(ed, null);
                    if (symAlg == null)
                        throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_MissingDecryptionKey"));
                    var decrypted = DecryptData(ed, symAlg);
                    ReplaceData(encryptedDataElement, decrypted);
                }
            }
        }

        // encrypts the supplied arbitrary data
        public Byte[] EncryptData (Byte[] plaintext, SymmetricAlgorithm symmetricAlgorithm) {
            if (plaintext == null)
                throw new ArgumentNullException(nameof(plaintext));
            if (symmetricAlgorithm == null)
                throw new ArgumentNullException(nameof(symmetricAlgorithm));

            // save the original symmetric algorithm
            var origMode = symmetricAlgorithm.Mode;
            var origPadding = symmetricAlgorithm.Padding;

            Byte[] cipher = null;
            try {
                symmetricAlgorithm.Mode = Mode;
                symmetricAlgorithm.Padding = Padding;

                var enc = symmetricAlgorithm.CreateEncryptor();
                cipher = enc.TransformFinalBlock(plaintext, 0, plaintext.Length);
            } finally {
                // now restore the original symmetric algorithm
                symmetricAlgorithm.Mode = origMode;
                symmetricAlgorithm.Padding = origPadding;
            }

            Byte[] output = null;
            if (Mode == CipherMode.ECB) {
                output = cipher;
            } else {
                var IV = symmetricAlgorithm.IV;
                output = new Byte[cipher.Length + IV.Length];
                Buffer.BlockCopy(IV, 0, output, 0, IV.Length);
                Buffer.BlockCopy(cipher, 0, output, IV.Length, cipher.Length);
            }
            return output;
        }

        // encrypts the supplied input element
        public Byte[] EncryptData (XmlElement inputElement, SymmetricAlgorithm symmetricAlgorithm, Boolean content) {
            if (inputElement == null)
                throw new ArgumentNullException(nameof(inputElement));
            if (symmetricAlgorithm == null)
                throw new ArgumentNullException(nameof(symmetricAlgorithm));

            var plainText = (content ? Encoding.GetBytes(inputElement.InnerXml) : Encoding.GetBytes(inputElement.OuterXml));
            return EncryptData(plainText, symmetricAlgorithm);
        }

        // decrypts the supplied EncryptedData
        public Byte[] DecryptData (EncryptedData encryptedData, SymmetricAlgorithm symmetricAlgorithm) {
            if (encryptedData == null)
                throw new ArgumentNullException(nameof(encryptedData));
            if (symmetricAlgorithm == null)
                throw new ArgumentNullException(nameof(symmetricAlgorithm));

            // get the cipher value and decrypt
            var cipherValue = GetCipherValue(encryptedData.CipherData);

            // save the original symmetric algorithm
            var origMode = symmetricAlgorithm.Mode;
            var origPadding = symmetricAlgorithm.Padding;
            var origIV = symmetricAlgorithm.IV;

            // read the IV from cipherValue
            Byte[] decryptionIV = null;
            if (Mode != CipherMode.ECB)
                decryptionIV = GetDecryptionIV(encryptedData, null);

            Byte[] output = null;
            try {
                var lengthIV = 0;
                if (decryptionIV != null) {
                    symmetricAlgorithm.IV = decryptionIV;
                    lengthIV = decryptionIV.Length;
                }
                symmetricAlgorithm.Mode = Mode;
                symmetricAlgorithm.Padding = Padding;

                var dec = symmetricAlgorithm.CreateDecryptor();
                output = dec.TransformFinalBlock(cipherValue, lengthIV, cipherValue.Length - lengthIV);
            } finally {
                // now restore the original symmetric algorithm
                symmetricAlgorithm.Mode = origMode;
                symmetricAlgorithm.Padding = origPadding;
                symmetricAlgorithm.IV = origIV;
            }

            return output;
        }

        // This method replaces an EncryptedData element with the decrypted sequence of bytes
        public void ReplaceData (XmlElement inputElement, Byte[] decryptedData) {
            if (inputElement == null)
                throw new ArgumentNullException (nameof(inputElement));
            if (decryptedData == null)
                throw new ArgumentNullException (nameof(decryptedData));

            var parent = inputElement.ParentNode;
            if (parent.NodeType == XmlNodeType.Document) {
                // We're replacing the root element, but we can't just wholesale replace the owner
                // document's InnerXml, since we need to preserve any other top-level XML elements (such as
                // comments or the XML entity declaration.  Instead, create a new document with the
                // decrypted XML, import it into the existing document, and replace just the root element.
                var importDocument = new XmlDocument();
                importDocument.PreserveWhitespace = true;
                var decryptedString = Encoding.GetString(decryptedData);
                using (var sr = new StringReader(decryptedString))
                {
                    using (var xr = XmlReader.Create(sr, Utils.GetSecureXmlReaderSettings(Resolver)))
                    {
                        importDocument.Load(xr);
                    }
                }

                var importedNode = inputElement.OwnerDocument.ImportNode(importDocument.DocumentElement, true);

                parent.RemoveChild(inputElement);
                parent.AppendChild(importedNode);
            } else {
                XmlNode dummy = parent.OwnerDocument.CreateElement(parent.Prefix, parent.LocalName, parent.NamespaceURI);

                try {
                    parent.AppendChild(dummy);

                    // Replace the children of the dummy node with the sequence of bytes passed in.
                    // The string will be parsed into DOM objects in the context of the parent of the EncryptedData element.
                    dummy.InnerXml = Encoding.GetString(decryptedData);

                    // Move the children of the dummy node up to the parent.
                    var child = dummy.FirstChild;
                    var sibling = inputElement.NextSibling;

                    XmlNode nextChild = null;
                    while (child != null) {
                        nextChild = child.NextSibling;
                        parent.InsertBefore(child, sibling);
                        child = nextChild;
                    }
                }
                finally {
                    // Remove the dummy element.
                    parent.RemoveChild(dummy);
                }

                // Remove the EncryptedData element
                parent.RemoveChild(inputElement);
            }
        }

        //
        // public static methods
        //

        // replaces the inputElement with the provided EncryptedData
        public static void ReplaceElement (XmlElement inputElement, EncryptedData encryptedData, Boolean content) {
            if (inputElement == null)
                throw new ArgumentNullException(nameof(inputElement));
            if (encryptedData == null)
                throw new ArgumentNullException(nameof(encryptedData));

            // First, get the XML representation of the EncryptedData object
            var elemED = encryptedData.GetXml(inputElement.OwnerDocument);
            switch (content) {
            case true:
                // remove all children of the input element
                Utils.RemoveAllChildren(inputElement);
                // then append the encrypted data as a child of the input element
                inputElement.AppendChild(elemED);
                break;
            case false:
                var parentNode = inputElement.ParentNode;
                // remove the input element from the containing document
                parentNode.ReplaceChild(elemED, inputElement);
                break;
            }
        }

        // wraps the supplied input key data using the provided symmetric algorithm
        public static Byte[] EncryptKey (Byte[] keyData, SymmetricAlgorithm symmetricAlgorithm) {
            if (keyData == null)
                throw new ArgumentNullException(nameof(keyData));
            if (symmetricAlgorithm == null)
                throw new ArgumentNullException(nameof(symmetricAlgorithm));

            if (symmetricAlgorithm is TripleDES) {
                // CMS Triple DES Key Wrap
                return SymmetricKeyWrap.TripleDESKeyWrapEncrypt(symmetricAlgorithm.Key, keyData);
            } else if (symmetricAlgorithm is Rijndael || symmetricAlgorithm is Aes) {
                // FIPS AES Key Wrap
                return SymmetricKeyWrap.AESKeyWrapEncrypt(symmetricAlgorithm.Key, keyData);
            }
            // throw an exception if the transform is not in the previous categories
            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_NotSupportedCryptographicTransform"));
        }


        // encrypts the supplied input key data using an RSA key and specifies whether we want to use OAEP 
        // padding or PKCS#1 v1.5 padding as described in the PKCS specification
        public static Byte[] EncryptKey (Byte[] keyData, RSA rsa, Boolean useOAEP) {
            if (keyData == null)
                throw new ArgumentNullException(nameof(keyData));
            if (rsa == null)
                throw new ArgumentNullException(nameof(rsa));

            if (useOAEP) {
                var rsaFormatter = new RSAOAEPKeyExchangeFormatter(rsa);
                return rsaFormatter.CreateKeyExchange(keyData);
            } else {
                var rsaFormatter = new RSAPKCS1KeyExchangeFormatter(rsa);
                return rsaFormatter.CreateKeyExchange(keyData);
            }
        }

        // decrypts the supplied wrapped key using the provided symmetric algorithm
        public static Byte[] DecryptKey (Byte[] keyData, SymmetricAlgorithm symmetricAlgorithm) {
            if (keyData == null)
                throw new ArgumentNullException(nameof(keyData));
            if (symmetricAlgorithm == null)
                throw new ArgumentNullException(nameof(symmetricAlgorithm));

            if (symmetricAlgorithm is TripleDES) {
                // CMS Triple DES Key Wrap
                return SymmetricKeyWrap.TripleDESKeyWrapDecrypt(symmetricAlgorithm.Key, keyData);
            } else if (symmetricAlgorithm is Rijndael || symmetricAlgorithm is Aes) {
                // FIPS AES Key Wrap
                return SymmetricKeyWrap.AESKeyWrapDecrypt(symmetricAlgorithm.Key, keyData);
            }
            // throw an exception if the transform is not in the previous categories
            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_NotSupportedCryptographicTransform"));
        }

        // decrypts the supplied data using an RSA key and specifies whether we want to use OAEP 
        // padding or PKCS#1 v1.5 padding as described in the PKCS specification
        public static Byte[] DecryptKey (Byte[] keyData, RSA rsa, Boolean useOAEP) {
            if (keyData == null)
                throw new ArgumentNullException(nameof(keyData));
            if (rsa == null)
                throw new ArgumentNullException(nameof(rsa));

            if (useOAEP) {
                var rsaDeformatter = new RSAOAEPKeyExchangeDeformatter(rsa);
                return rsaDeformatter.DecryptKeyExchange(keyData);
            } else {
                var rsaDeformatter = new RSAPKCS1KeyExchangeDeformatter(rsa);
                return rsaDeformatter.DecryptKeyExchange(keyData);
            }
        }
    }
}
