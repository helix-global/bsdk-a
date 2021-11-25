using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Xml;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using BinaryStudio.Security.Cryptography.Xml.Properties;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Xml
    {
    using X509Certificate = BinaryStudio.Security.Cryptography.Certificates.X509Certificate;
    [HostProtection(MayLeakOnAbort = true)]
    public class SignedXml {
        /// <internalonly/>
        protected Signature m_signature;
        /// <internalonly/>
        protected String m_strSigningKeyName;

        private XmlDocument m_containingDocument;
        private IEnumerator m_keyInfoEnum;
        private X509Certificate2Collection m_x509Collection;
        private IEnumerator m_x509Enum;

        private Boolean[] m_refProcessed;
        private Int32[] m_refLevelCache;

        internal XmlResolver m_xmlResolver;
        internal XmlElement m_context;

        // Built in canonicalization algorithm URIs
        private static IList<String> s_knownCanonicalizationMethods;
        // Built in transform algorithm URIs (excluding canonicalization URIs)
        private static IList<String> s_defaultSafeTransformMethods;

        // additional HMAC Url identifiers
        private const String XmlDsigMoreHMACMD5Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-md5";
        private const String XmlDsigMoreHMACSHA256Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";
        private const String XmlDsigMoreHMACSHA384Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384";
        private const String XmlDsigMoreHMACSHA512Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";
        private const String XmlDsigMoreHMACRIPEMD160Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160";

        // defines the XML encryption processing rules
        private EncryptedXml m_exml;

        //
        // public constant Url identifiers most frequently used within the XML Signature classes
        //

        public const String XmlDsigNamespaceUrl = "http://www.w3.org/2000/09/xmldsig#";
        public const String XmlDsigMinimalCanonicalizationUrl = "http://www.w3.org/2000/09/xmldsig#minimal";
        public const String XmlDsigCanonicalizationUrl = XmlDsigC14NTransformUrl;
        public const String XmlDsigCanonicalizationWithCommentsUrl = XmlDsigC14NWithCommentsTransformUrl;

        public const String XmlDsigSHA1Url = "http://www.w3.org/2000/09/xmldsig#sha1";
        public const String XmlDsigDSAUrl = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
        public const String XmlDsigRSASHA1Url = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        public const String XmlDsigHMACSHA1Url = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";

        public const String XmlDsigSHA256Url = "http://www.w3.org/2001/04/xmlenc#sha256";
        public const String XmlDsigRSASHA256Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

        // Yes, SHA384 is in the xmldsig-more namespace even though all the other SHA variants are in xmlenc. That's the standard.
        public const String XmlDsigSHA384Url = "http://www.w3.org/2001/04/xmldsig-more#sha384";
        public const String XmlDsigRSASHA384Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384";

        public const String XmlDsigSHA512Url = "http://www.w3.org/2001/04/xmlenc#sha512";
        public const String XmlDsigRSASHA512Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";

        internal static readonly String XmlDsigDigestDefault = XmlDsigSHA256Url;
        internal static readonly String XmlDsigRSADefault = XmlDsigRSASHA256Url;

        public const String XmlDsigC14NTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315"; 
        public const String XmlDsigC14NWithCommentsTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments"; 
        public const String XmlDsigExcC14NTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#";
        public const String XmlDsigExcC14NWithCommentsTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#WithComments"; 
        public const String XmlDsigBase64TransformUrl = "http://www.w3.org/2000/09/xmldsig#base64";
        public const String XmlDsigXPathTransformUrl = "http://www.w3.org/TR/1999/REC-xpath-19991116";
        public const String XmlDsigXsltTransformUrl = "http://www.w3.org/TR/1999/REC-xslt-19991116";
        public const String XmlDsigEnvelopedSignatureTransformUrl = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
        public const String XmlDecryptionTransformUrl = "http://www.w3.org/2002/07/decrypt#XML";
        public const String XmlLicenseTransformUrl = "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform";

        private class LocalHashAlgorithm : IHashAlgorithm
            {
            internal readonly HashAlgorithm source;
            public LocalHashAlgorithm(HashAlgorithm source)
                {
                this.source = source;
                }

            public void Dispose()
                {
                
                }

            public Int32 TransformBlock(Byte[] inputBuffer, Int32 inputOffset, Int32 inputCount, Byte[] outputBuffer, Int32 outputOffset)
                {
                return source.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
                }

            public Byte[] TransformFinalBlock(Byte[] inputBuffer, Int32 inputOffset, Int32 inputCount)
                {
                return source.TransformFinalBlock(inputBuffer, inputOffset, inputCount);
                }

            public Int32 InputBlockSize               { get { return source.InputBlockSize;             }}
            public Int32 OutputBlockSize              { get { return source.OutputBlockSize;            }}
            public Int32 HashSize                     { get { return source.HashSize;                   }}
            public Boolean CanTransformMultipleBlocks { get { return source.CanTransformMultipleBlocks; }}
            public Boolean CanReuseTransform          { get { return source.CanReuseTransform;          }}
            public Byte[] Hash { get { return source.Hash; }}

            public void Initialize()
                {
                source.Initialize();
                }

            public Byte[] ComputeHash(Stream inputstream)
                {
                return source.ComputeHash(inputstream);
                }

            public Byte[] Compute(Byte[] bytes)
                {
                return source.ComputeHash(new MemoryStream(bytes));
                }

            public void CreateSignature(Stream signature, KeySpec keyspec)
                {
                throw new NotImplementedException();
                }
            }

        //
        // public constructors
        //

        public SignedXml() {
            Initialize(null);
        }

        public SignedXml(XmlDocument document) {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            Initialize(document.DocumentElement);
        }

        public SignedXml(XmlElement elem) {
            if (elem == null)
                throw new ArgumentNullException(nameof(elem));
            Initialize(elem);
        }

        private void Initialize (XmlElement element) {
            m_containingDocument = (element == null ? null : element.OwnerDocument);
            m_context = element;
            m_signature = new Signature();
            m_signature.SignedXml = this;
            m_signature.SignedInfo = new SignedInfo();
            SigningKey = null;

            SafeCanonicalizationMethods = new Collection<String>(KnownCanonicalizationMethods);
        }

        //
        // public properties
        //

        /// <internalonly/>
        public String SigningKeyName {
            get { return m_strSigningKeyName; }
            set { m_strSigningKeyName = value; }
        }

        [ComVisible(false)]
        public XmlResolver Resolver {
            // This property only has a setter. The rationale for this is that we don't have a good value
            // to return when it has not been explicitely set, as we are using XmlSecureResolver by default
            set { 
                m_xmlResolver = value;
                ResolverSet = true;
            }
        }

        internal Boolean ResolverSet { get; private set; }

        public Func<SignedXml, Boolean> SignatureFormatValidator { get; set; } = DefaultSignatureFormatValidator;

        public Collection<String> SafeCanonicalizationMethods { get; private set; }

        public AsymmetricAlgorithm SigningKey { get; set; }

        [ComVisible(false)]
        public EncryptedXml EncryptedXml {
            get {
                if (m_exml == null)
                    m_exml = new EncryptedXml(m_containingDocument); // default processing rules
                return m_exml; 
            }
            set { m_exml = value; }
        }

        public Signature Signature {
            get { return m_signature; }
        }

        public SignedInfo SignedInfo {
            get { return m_signature.SignedInfo; }
        }

        public String SignatureMethod  {
            get { return m_signature.SignedInfo.SignatureMethod; }
        }

        public String SignatureLength {
            get { return m_signature.SignedInfo.SignatureLength; }
        }

        public Byte[] SignatureValue {
            get { return m_signature.SignatureValue; }
        }

        public KeyInfo KeyInfo {
            get { return m_signature.KeyInfo; }
            set { m_signature.KeyInfo = value; }
        }

        public XmlElement GetXml() {
            // If we have a document context, then return a signature element in this context
            if (m_containingDocument != null)
                return m_signature.GetXml(m_containingDocument);
            else
                return m_signature.GetXml();
        }

        public void LoadXml(XmlElement value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            m_signature.LoadXml(value);

            if (m_context == null) {
                m_context = value;
            }

            bCacheValid = false;
        }

        //
        // public methods
        //

        public void AddReference(Reference reference) {
            m_signature.SignedInfo.AddReference(reference);
        }

        public void AddObject(DataObject dataObject) {
            m_signature.AddObject(dataObject);
        }

        public Boolean CheckSignature() {
            AsymmetricAlgorithm signingKey;
            return CheckSignatureReturningKey(out signingKey);
        }

        public Boolean CheckSignatureReturningKey(out AsymmetricAlgorithm signingKey) {
            SignedXmlDebugLog.LogBeginSignatureVerification(this, m_context);

            signingKey = null;
            var bRet = false;
            AsymmetricAlgorithm key = null;

            if (!CheckSignatureFormat()) {
                return false;
            }

            do {
                key = GetPublicKey();
                if (key != null) {
                    bRet = CheckSignature(key);
                    SignedXmlDebugLog.LogVerificationResult(this, key, bRet);
                }
            } while (key != null && bRet == false);

            signingKey = key;
            return bRet;
        }

        public Boolean CheckSignature(AsymmetricAlgorithm key) {
            if (!CheckSignatureFormat()) {
                return false;
            }

            if (!CheckSignedInfo(key)) {
                SignedXmlDebugLog.LogVerificationFailure(this, SecurityResources.GetResourceString("Log_VerificationFailed_SignedInfo"));
                return false;
            }

            // Now is the time to go through all the references and see if their DigestValues are good
            if (!CheckDigestedReferences()) {
                SignedXmlDebugLog.LogVerificationFailure(this, SecurityResources.GetResourceString("Log_VerificationFailed_References"));
                return false;
            }

            SignedXmlDebugLog.LogVerificationResult(this, key, true);
            return true;
        }

        public Boolean CheckSignature(KeyedHashAlgorithm macAlg) {
            if (!CheckSignatureFormat()) {
                return false;
            }

            if (!CheckSignedInfo(macAlg)) {
                SignedXmlDebugLog.LogVerificationFailure(this, SecurityResources.GetResourceString("Log_VerificationFailed_SignedInfo"));
                return false;
            }

            if (!CheckDigestedReferences()) {
                SignedXmlDebugLog.LogVerificationFailure(this, SecurityResources.GetResourceString("Log_VerificationFailed_References"));
                return false;
            }

            SignedXmlDebugLog.LogVerificationResult(this, macAlg, true);
            return true;
        }

        [ComVisible(false)]
        [SecuritySafeCritical]
        public Boolean CheckSignature(X509Certificate2 certificate, Boolean verifySignatureOnly) {
            if (!verifySignatureOnly) {
                // Check key usages to make sure it is good for signing.
                foreach (var extension in certificate.Extensions) {
                    if (String.Compare(extension.Oid.Value, ObjectIdentifiers.szOID_KEY_USAGE, StringComparison.OrdinalIgnoreCase) == 0) {
                        var keyUsage = new X509KeyUsageExtension();
                        keyUsage.CopyFrom(extension);
                        SignedXmlDebugLog.LogVerifyKeyUsage(this, certificate, keyUsage);

                        var validKeyUsage = (keyUsage.KeyUsages & X509KeyUsageFlags.DigitalSignature) != 0 ||
                                             (keyUsage.KeyUsages & X509KeyUsageFlags.NonRepudiation) != 0;

                        if (!validKeyUsage) {
                            SignedXmlDebugLog.LogVerificationFailure(this, SecurityResources.GetResourceString("Log_VerificationFailed_X509KeyUsage"));
                            return false;
                        }
                        break;
                    }
                }

                // Do the chain verification to make sure the certificate is valid.
                var chain = new X509Chain();
                chain.ChainPolicy.ExtraStore.AddRange(BuildBagOfCerts());
                var chainVerified = chain.Build(certificate);
                SignedXmlDebugLog.LogVerifyX509Chain(this, chain, certificate);

                if (!chainVerified) {
                    SignedXmlDebugLog.LogVerificationFailure(this, SecurityResources.GetResourceString("Log_VerificationFailed_X509Chain"));
                    return false;
                }
            }

            if (!CheckSignature(certificate.PublicKey.Key)) {
                return false;
            }

            SignedXmlDebugLog.LogVerificationResult(this, certificate, true);
            return true;
        }

        public void ComputeSignature() {
            SignedXmlDebugLog.LogBeginSignatureComputation(this, m_context);

            BuildDigestedReferences();

            // Load the key
            var key = SigningKey;

            if ((key == null) && (SigningCertificate == null))
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_LoadKeyFailed"));

            // Check the signature algorithm associated with the key so that we can accordingly set the signature method
            if (SignedInfo.SignatureMethod == null) {
                if (key is DSA) {
                    SignedInfo.SignatureMethod = XmlDsigDSAUrl;
                } else if (key is RSA) {
                    // Default to RSA-SHA256 or RSA-SHA1 depending on context switch
                    if (SignedInfo.SignatureMethod == null) 
                        SignedInfo.SignatureMethod = XmlDsigRSADefault;
                } else
                    {
                    //throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_CreatedKeyFailed"));
                    SignedInfo.SignatureMethod = key.SignatureAlgorithm;
                    }
                }

            // See if there is a signature description class defined in the Config file
            var signatureDescription = Utils.CreateFromName<SignatureDescription>(SignedInfo.SignatureMethod);
            if (signatureDescription == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_SignatureDescriptionNotCreated"));
            var hashAlg = new LocalHashAlgorithm(signatureDescription.CreateDigest());
            if (hashAlg == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_CreateHashAlgorithmFailed"));
            var hashvalue = GetC14NDigest(hashAlg);
            var asymmetricSignatureFormatter = signatureDescription.CreateFormatter(key ?? new CertificateAsymmetricAlgorithm(SigningCertificate));
            SignedXmlDebugLog.LogSigning(this, key, signatureDescription, hashAlg, asymmetricSignatureFormatter);
            m_signature.SignatureValue = asymmetricSignatureFormatter.CreateSignature(hashAlg.source);
            //    }
            //else
            //    {
            //    using (var context = new CryptographicContext(SigningCertificate.SignatureAlgorithm, CryptographicContextFlags.CRYPT_SILENT|CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
            //        using (var signing = new CryptographicContext(context, SigningCertificate, CRYPT_ACQUIRE_FLAGS.CRYPT_ACQUIRE_NONE)) {
            //            CryptHashAlgorithm
            //            }
            //        }
            //    }
            }

        public void ComputeSignature(KeyedHashAlgorithm macAlg) {
            if (macAlg == null)
                throw new ArgumentNullException(nameof(macAlg));

            var hash = macAlg as HMAC;
            if(hash == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_SignatureMethodKeyMismatch"));

            Int32 signatureLength;
            if (m_signature.SignedInfo.SignatureLength == null)
                signatureLength = hash.HashSize;
            else
                signatureLength = Convert.ToInt32(m_signature.SignedInfo.SignatureLength, null);
            // signatureLength should be less than hash size
            if (signatureLength < 0 || signatureLength > hash.HashSize)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidSignatureLength"));
            if (signatureLength % 8 != 0)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidSignatureLength2"));

            BuildDigestedReferences();
            switch (hash.HashName) {
            case "SHA1":
                SignedInfo.SignatureMethod = XmlDsigHMACSHA1Url;
                break;
            case "SHA256":
                SignedInfo.SignatureMethod = XmlDsigMoreHMACSHA256Url;
                break;
            case "SHA384":
                SignedInfo.SignatureMethod = XmlDsigMoreHMACSHA384Url;
                break;
            case "SHA512":
                SignedInfo.SignatureMethod = XmlDsigMoreHMACSHA512Url;
                break;
            case "MD5":
                SignedInfo.SignatureMethod = XmlDsigMoreHMACMD5Url;
                break;
            case "RIPEMD160":
                SignedInfo.SignatureMethod = XmlDsigMoreHMACRIPEMD160Url;
                break;
            default:
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_SignatureMethodKeyMismatch"));
            }

            var hashValue = GetC14NDigest(new LocalHashAlgorithm(hash));

            SignedXmlDebugLog.LogSigning(this, hash);
            m_signature.SignatureValue = new Byte[signatureLength / 8];
            Buffer.BlockCopy(hashValue, 0, m_signature.SignatureValue, 0, signatureLength / 8);
        }

        //
        // virtual methods
        //

        protected virtual AsymmetricAlgorithm GetPublicKey() {
            if (KeyInfo == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_KeyInfoRequired"));

            if (m_x509Enum != null) {
                var key = GetNextCertificatePublicKey();
                if (key != null)
                    return key;
            }

            if (m_keyInfoEnum == null)
                m_keyInfoEnum = KeyInfo.GetEnumerator();

            // In our implementation, we move to the next KeyInfo clause which is an RSAKeyValue, DSAKeyValue or KeyInfoX509Data
            while (m_keyInfoEnum.MoveNext()) {
                var rsaKeyValue = m_keyInfoEnum.Current as RSAKeyValue;
                if (rsaKeyValue != null) 
                    return rsaKeyValue.Key;

                var dsaKeyValue = m_keyInfoEnum.Current as DSAKeyValue;
                if (dsaKeyValue != null) 
                    return dsaKeyValue.Key;

                var x509Data = m_keyInfoEnum.Current as KeyInfoX509Data;
                if (x509Data != null) {
                    m_x509Collection = Utils.BuildBagOfCerts (x509Data, CertUsageType.Verification);
                    if (m_x509Collection.Count > 0) {
                        m_x509Enum = m_x509Collection.GetEnumerator();
                        var key = GetNextCertificatePublicKey();
                        if (key != null)
                            return key;
                    }
                }
            }

            return null;
        }

        private X509Certificate2Collection BuildBagOfCerts () {
            var collection = new X509Certificate2Collection();
            if (KeyInfo != null) {
                foreach (KeyInfoClause clause in KeyInfo) {
                    var x509Data = clause as KeyInfoX509Data;
                    if (x509Data != null) 
                        collection.AddRange(Utils.BuildBagOfCerts(x509Data, CertUsageType.Verification));
                }
            }

            return collection;
        }

        private AsymmetricAlgorithm GetNextCertificatePublicKey() {
            while (m_x509Enum.MoveNext()) {
                var certificate = (X509Certificate2) m_x509Enum.Current;
                if (certificate != null)
                    {
                    return new CertificateAsymmetricAlgorithm(
                        new X509Certificate(certificate.Handle));
                    }
                }
            return null;
            }

        public virtual XmlElement GetIdElement (XmlDocument document, String idValue) {
            return DefaultGetIdElement(document, idValue);
        }

        internal static XmlElement DefaultGetIdElement(XmlDocument document, String idValue) {
            if (document == null)
                return null;

            if (Utils.RequireNCNameIdentifier()) {
                try {
                    XmlConvert.VerifyNCName(idValue);
                } catch (XmlException) {
                    // Identifiers are required to be an NCName
                    //   (xml:id version 1.0, part 4, paragraph 2, bullet 1)
                    //
                    // If it isn't an NCName, it isn't allowed to match.
                    return null;
                }
            }

            // Get the element with idValue
            var elem = document.GetElementById(idValue);

            if (elem != null) {
                if (!Utils.AllowAmbiguousReferenceTargets()) {
                    // Have to check for duplicate ID values from the DTD.

                    var docClone = (XmlDocument)document.CloneNode(true);
                    var cloneElem = docClone.GetElementById(idValue);

                    // If it's null here we want to know about it, because it means that
                    // GetElementById failed to work across the cloning, and our uniqueness
                    // test is invalid.
                    System.Diagnostics.Debug.Assert(cloneElem != null);

                    // Guard against null anyways
                    if (cloneElem != null) {
                        cloneElem.Attributes.RemoveAll();

                        var cloneElem2 = docClone.GetElementById(idValue);

                        if (cloneElem2 != null) {
                            throw new CryptographicException(
                                SecurityResources.GetResourceString("Cryptography_Xml_InvalidReference"));
                        }
                    }
                }

                return elem;
            }

            elem = GetSingleReferenceTarget(document, "Id", idValue);
            if (elem != null)
                return elem;
            elem = GetSingleReferenceTarget(document, "id", idValue);
            if (elem != null)
                return elem;
            elem = GetSingleReferenceTarget(document, "ID", idValue);

            return elem;
        }

        //
        // private methods
        //

        private Boolean bCacheValid;
        private Byte[] _digestedSignedInfo;

        private static Boolean DefaultSignatureFormatValidator(SignedXml signedXml) {
            // Reject the signature if it uses a truncated HMAC
            if (signedXml.DoesSignatureUseTruncatedHmac()) {
                return false;
            }

            // Reject the signature if it uses a canonicalization algorithm other than
            // one of the ones explicitly allowed
            if (!signedXml.DoesSignatureUseSafeCanonicalizationMethod()) {
                return false;
            }

            // Otherwise accept it
            return true;
        }

        // Validation function to see if the current signature is signed with a truncated HMAC - one which
        // has a signature length of fewer bits than the whole HMAC output.
        private Boolean DoesSignatureUseTruncatedHmac() {
            // If we're not using the SignatureLength property, then we're not truncating the signature length
            if (SignedInfo.SignatureLength == null) {
                return false;
            }

            // See if we're signed witn an HMAC algorithm
            var hmac = Utils.CreateFromName<HMAC>(SignatureMethod);
            if (hmac == null) {
                // We aren't signed with an HMAC algorithm, so we cannot have a truncated HMAC
                return false;
            }

            // Figure out how many bits the signature is using
            if (!Int32.TryParse(SignedInfo.SignatureLength, out var actualSignatureSize)) {
                // If the value wasn't a valid integer, then we'll conservatively reject it all together
                return true;
            }

            // Make sure the full HMAC signature size is the same size that was specified in the XML
            // signature.  If the actual signature size is not exactly the same as the full HMAC size, then
            // reject the signature.
            return actualSignatureSize != hmac.HashSize;
        }

        // Validation function to see if the signature uses a canonicalization algorithm from our list
        // of approved algorithm URIs.
        private Boolean DoesSignatureUseSafeCanonicalizationMethod() {
            foreach (var safeAlgorithm in SafeCanonicalizationMethods) {
                if (String.Equals(safeAlgorithm, SignedInfo.CanonicalizationMethod, StringComparison.OrdinalIgnoreCase)) {
                    return true;
                }
            }

            SignedXmlDebugLog.LogUnsafeCanonicalizationMethod(this, SignedInfo.CanonicalizationMethod, SafeCanonicalizationMethods);
            return false;
        }

        private Boolean ReferenceUsesSafeTransformMethods(Reference reference)
        {
            var transformChain = reference.TransformChain;
            var transformCount = transformChain.Count;

            for (var i = 0; i < transformCount; i++) {
                var transform = transformChain[i];

                if (!IsSafeTransform(transform.Algorithm)) {
                    return false;
                }
            }

            return true;
        }

        private Boolean IsSafeTransform(String transformAlgorithm)
        {
            // All canonicalization algorithms are valid transform algorithms.
            foreach (var safeAlgorithm in SafeCanonicalizationMethods) {
                if (String.Equals(safeAlgorithm, transformAlgorithm, StringComparison.OrdinalIgnoreCase)) {
                    return true;
                }
            }

            foreach (var safeAlgorithm in DefaultSafeTransformMethods) {
                if (String.Equals(safeAlgorithm, transformAlgorithm, StringComparison.OrdinalIgnoreCase)) {
                    return true;
                }
            }

            SignedXmlDebugLog.LogUnsafeTransformMethod(
                this,
                transformAlgorithm,
                SafeCanonicalizationMethods,
                DefaultSafeTransformMethods);

            return false;
        }

        // Get a list of the built in canonicalization algorithms, as well as any that the machine admin has
        // added to the valid set.
        private static IList<String> KnownCanonicalizationMethods
        {
            get {
                if (s_knownCanonicalizationMethods == null) {
                    // Start with the list that the machine admin added, if any
                    var safeAlgorithms = ReadAdditionalSafeCanonicalizationMethods();

                    // And add the built in algorithms as well
                    safeAlgorithms.Add(XmlDsigC14NTransformUrl);
                    safeAlgorithms.Add(XmlDsigC14NWithCommentsTransformUrl);
                    safeAlgorithms.Add(XmlDsigExcC14NTransformUrl);
                    safeAlgorithms.Add(XmlDsigExcC14NWithCommentsTransformUrl);

                    s_knownCanonicalizationMethods = safeAlgorithms;
                }

                return s_knownCanonicalizationMethods;
            }
        }

        private static IList<String> DefaultSafeTransformMethods
        {
            get {
                if (s_defaultSafeTransformMethods == null) {
                    var safeAlgorithms = ReadAdditionalSafeTransformMethods();

                    // And the built in algorithms

                    // KnownCanonicalizationMethods don't need to be added here, because
                    // the validator will automatically accept those.
                    //
                    // xmldsig 6.6.1:
                    //     Any canonicalization algorithm that can be used for
                    //     CanonicalizationMethod can be used as a Transform.
                    safeAlgorithms.Add(XmlDsigEnvelopedSignatureTransformUrl);
                    safeAlgorithms.Add(XmlDsigBase64TransformUrl);
                    safeAlgorithms.Add(XmlLicenseTransformUrl);
                    safeAlgorithms.Add(XmlDecryptionTransformUrl);

                    s_defaultSafeTransformMethods = safeAlgorithms;
                }

                return s_defaultSafeTransformMethods;
            }
        }

        // Allow machine admins to add additional canonicalization algorithms that should be considered valid when
        // validating XML signatuers by supplying a list in the 
        // HKLM\Software\Microsoft\.NETFramework\Security\SafeCanonicalizationMethods
        // key.  Each REG_SZ entry in this key will be considered a canonicalziation algorithm URI that should be
        // allowed by SignedXml instances on this machine.
        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        private static List<String> ReadAdditionalSafeCanonicalizationMethods() {
            return ReadFxSecurityStringValues("SafeCanonicalizationMethods");
        }

        // Allow machine admins to add additional transform algorithms that should be considered valid when
        // validating XML signatuers by supplying a list in the 
        // HKLM\Software\Microsoft\.NETFramework\Security\SafeTransformMethods
        // key.  Each REG_SZ entry in this key will be considered a transform algorithm URI that should be
        // allowed by SignedXml instances on this machine.
        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        private static List<String> ReadAdditionalSafeTransformMethods() {
            return ReadFxSecurityStringValues("SafeTransformMethods");
        }

        private static List<String> ReadFxSecurityStringValues(String subkey) {
            var values = new List<String>();

            try {
                using (var stringListKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework\Security\" + subkey, false)) {
                    if (stringListKey != null) {
                        foreach (var value in stringListKey.GetValueNames()) {
                            if (stringListKey.GetValueKind(value) == RegistryValueKind.String) {
                                var stringValue = stringListKey.GetValue(value) as String;
                                if (!String.IsNullOrWhiteSpace(stringValue)) {
                                    values.Add(stringValue);
                                }
                            }
                        }
                    }
                }
            }
            catch (SecurityException) { /* we could not open the key - that's fine, we can proceed with no additional values */ }
            return values;
        }

        private Byte[] GetC14NDigest (IHashAlgorithm hash) {
            if (!bCacheValid || !SignedInfo.CacheValid) {
                var baseUri = (m_containingDocument == null ? null : m_containingDocument.BaseURI);
                var resolver = (ResolverSet ? m_xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), baseUri));
                var doc = Utils.PreProcessElementInput(SignedInfo.GetXml(), resolver, baseUri);

                // Add non default namespaces in scope
                var namespaces = (m_context == null ? null : Utils.GetPropagatedAttributes(m_context));
                SignedXmlDebugLog.LogNamespacePropagation(this, namespaces);
                Utils.AddNamespaces(doc.DocumentElement, namespaces);

                var c14nMethodTransform = SignedInfo.CanonicalizationMethodObject;
                c14nMethodTransform.Resolver = resolver;
                c14nMethodTransform.BaseURI = baseUri;

                SignedXmlDebugLog.LogBeginCanonicalization(this, c14nMethodTransform);
                c14nMethodTransform.LoadInput(doc);
                SignedXmlDebugLog.LogCanonicalizedOutput(this, c14nMethodTransform);
                _digestedSignedInfo = c14nMethodTransform.GetDigestedOutput(hash);

                bCacheValid = true;
            }
            return _digestedSignedInfo;
        }

        private Int32 GetReferenceLevel (Int32 index, ArrayList references) {
            if (m_refProcessed[index]) return m_refLevelCache[index];
            m_refProcessed[index] = true;
            var reference = (Reference) references[index];
            if (reference.Uri == null || reference.Uri.Length == 0 || (reference.Uri.Length > 0 && reference.Uri[0] != '#')) {
                m_refLevelCache[index] = 0;
                return 0;
            }
            if (reference.Uri.Length > 0 && reference.Uri[0] == '#') {
                var idref = Utils.ExtractIdFromLocalUri(reference.Uri);
                if (idref == "xpointer(/)") {
                    m_refLevelCache[index] = 0;
                    return 0;
                }
                // If this is pointing to another reference
                for (var j=0; j < references.Count; ++j) {
                    if (((Reference)references[j]).Id == idref) {
                        m_refLevelCache[index] = GetReferenceLevel(j, references) + 1;
                        return (m_refLevelCache[index]);
                    }
                }
                // Then the reference points to an object tag
                m_refLevelCache[index] = 0;
                return 0;
            }
            // Malformed reference
            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidReference"));
        }

        private class ReferenceLevelSortOrder : IComparer {
            public ReferenceLevelSortOrder() {}

            public ArrayList References { get; set; }

            public Int32 Compare(Object a, Object b) {
                var referenceA = a as Reference;
                var referenceB = b as Reference;

                // Get the indexes
                var iIndexA = 0;
                var iIndexB = 0;
                var i = 0;
                foreach (Reference reference in References) {
                    if (reference == referenceA) iIndexA = i;
                    if (reference == referenceB) iIndexB = i;
                    i++;
                }

                var iLevelA = referenceA.SignedXml.GetReferenceLevel(iIndexA, References);
                var iLevelB = referenceB.SignedXml.GetReferenceLevel(iIndexB, References);
                return iLevelA.CompareTo(iLevelB);
            }
        }

        private void BuildDigestedReferences() {
            // Default the DigestMethod and Canonicalization
            var references = SignedInfo.References;
            // Reset the cache
            m_refProcessed = new Boolean[references.Count];
            m_refLevelCache = new Int32[references.Count];

            var sortOrder = new ReferenceLevelSortOrder();
            sortOrder.References = references;
            // Don't alter the order of the references array list
            var sortedReferences = new ArrayList();
            foreach (Reference reference in references) {
                sortedReferences.Add(reference);
            }
            sortedReferences.Sort(sortOrder);

            var nodeList = new CanonicalXmlNodeList();
            foreach (DataObject obj in m_signature.ObjectList) {
                nodeList.Add(obj.GetXml());
            }
            foreach (Reference reference in sortedReferences) {
                // If no DigestMethod has yet been set, default it to SHA256 or SHA1 depending on context switch
                if (reference.DigestMethod == null)
                    reference.DigestMethod = XmlDsigDigestDefault;

                SignedXmlDebugLog.LogSigningReference(this, reference);

                reference.UpdateHashValue(m_containingDocument, nodeList);
                // If this reference has an Id attribute, add it
                if (reference.Id != null)
                    nodeList.Add(reference.GetXml());
            }
        }

        private Boolean CheckDigestedReferences () {
            var references = m_signature.SignedInfo.References;
            for (var i = 0; i < references.Count; ++i) {
                var digestedReference = (Reference) references[i];

                if (!ReferenceUsesSafeTransformMethods(digestedReference)) {
                    return false;
                }

                SignedXmlDebugLog.LogVerifyReference(this, digestedReference);
                Byte[] calculatedHash = null;
                try {
                    calculatedHash = digestedReference.CalculateHashValue(m_containingDocument, m_signature.ReferencedItems);
                }
                catch (CryptoSignedXmlRecursionException) {
                    SignedXmlDebugLog.LogSignedXmlRecursionLimit(this, digestedReference);
                    return false;
                }
                // Compare both hashes
                SignedXmlDebugLog.LogVerifyReferenceHash(this, digestedReference, calculatedHash, digestedReference.DigestValue);

                if (!CryptographicEquals(calculatedHash, digestedReference.DigestValue, calculatedHash.Length)) {
                    return false;
                }
            }

            return true;
        }

        // Methods _must_ be marked both No Inlining and No Optimization to be fully opted out of optimization.
        // This is because if a candidate method is inlined, its method level attributes, including the NoOptimization
        // attribute, are lost. 
        // This method makes no attempt to disguise the length of either of its inputs. It is assumed the attacker has 
        // knowledge of the algorithms used, and thus the output length. Length is difficult to properly blind in modern CPUs.
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static Boolean CryptographicEquals(Byte[] a, Byte[] b, Int32 count) {
            System.Diagnostics.Debug.Assert(a != null);
            System.Diagnostics.Debug.Assert(b != null);
            System.Diagnostics.Debug.Assert(count > 0);

            var result = 0;

            // Short cut if the lengths are not right
            if (a.Length < count || b.Length < count)
                return false;

            unchecked {
                // This routine was changed to accept the count as an input, so the total length is already
                // in a register.
                //
                // If this method is being copied to require an exact length match (a la
                // CryptographicOperations.FixedTimeEquals(ReadOnlySpan, ReadOnlySpan) then
                // one of the lengths should be saved to a local here, or the non-optimized call in the
                // for loop is noticable.

                for (var i = 0; i < count; i++)
                    // We use subtraction here instead of XOR because the XOR algorithm gets ever so
                    // slightly faster as more and more differences pile up.
                    // This cannot overflow more than once (and back to 0) because bytes are 1 byte
                    // in length, and result is 4 bytes. The OR propagates all set bytes, so the differences
                    // can't add up and overflow a second time.
                    result = result | (a[i] - b[i]);
            }

            return (0 == result);
        }

        // If we have a signature format validation callback, check to see if this signature's format (not
        // the signautre itself) is valid according to the validator.  A return value of true indicates that
        // the signature format is acceptable, false means that the format is not valid.
        private Boolean CheckSignatureFormat() {
            if (SignatureFormatValidator == null) {
                // No format validator means that we default to accepting the signature.  (This is
                // effectively compatibility mode with v3.5).
                return true;
            }
            
            SignedXmlDebugLog.LogBeginCheckSignatureFormat(this, SignatureFormatValidator);

            var formatValid = SignatureFormatValidator(this);
            SignedXmlDebugLog.LogFormatValidationResult(this, formatValid);
            return formatValid;
        }

        private Boolean CheckSignedInfo (AsymmetricAlgorithm key) {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            SignedXmlDebugLog.LogBeginCheckSignedInfo(this, m_signature.SignedInfo);

            var signatureDescription = Utils.CreateFromName<SignatureDescription>(SignatureMethod);
            if (signatureDescription == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_SignatureDescriptionNotCreated"));

            // Let's see if the key corresponds with the SignatureMethod 
            var ta = Type.GetType(signatureDescription.KeyAlgorithm);
            if (!IsKeyTheCorrectAlgorithm(key, ta))
                return false;

            var hashAlgorithm = new LocalHashAlgorithm(signatureDescription.CreateDigest());
            if (hashAlgorithm == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_CreateHashAlgorithmFailed"));
            var hashval = GetC14NDigest(hashAlgorithm);

            var asymmetricSignatureDeformatter = signatureDescription.CreateDeformatter(key);
            SignedXmlDebugLog.LogVerifySignedInfo(this,
                                                  key,
                                                  signatureDescription,
                                                  hashAlgorithm,
                                                  asymmetricSignatureDeformatter,
                                                  hashval,
                                                  m_signature.SignatureValue);
            return asymmetricSignatureDeformatter.VerifySignature(hashval, m_signature.SignatureValue);
        }

        private Boolean CheckSignedInfo (KeyedHashAlgorithm macAlg) {
            if (macAlg == null)
                throw new ArgumentNullException(nameof(macAlg));

            SignedXmlDebugLog.LogBeginCheckSignedInfo(this, m_signature.SignedInfo);

            Int32 signatureLength;
            if (m_signature.SignedInfo.SignatureLength == null)
                signatureLength = macAlg.HashSize;
            else 
                signatureLength = Convert.ToInt32(m_signature.SignedInfo.SignatureLength, null);

            // signatureLength should be less than hash size
            if (signatureLength < 0 || signatureLength > macAlg.HashSize)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidSignatureLength"));
            if (signatureLength % 8 != 0)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidSignatureLength2"));
            if (m_signature.SignatureValue == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_SignatureValueRequired"));
            if (m_signature.SignatureValue.Length != signatureLength / 8)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidSignatureLength"));

            // Calculate the hash
            var hashValue = GetC14NDigest(new LocalHashAlgorithm(macAlg));
            SignedXmlDebugLog.LogVerifySignedInfo(this, macAlg, hashValue, m_signature.SignatureValue);
            for (var i=0; i<m_signature.SignatureValue.Length; i++) {
                if (m_signature.SignatureValue[i] != hashValue[i]) return false;
            }
            return true; 
        }

        private static XmlElement GetSingleReferenceTarget(XmlDocument document, String idAttributeName, String idValue) {
            // idValue has already been tested as an NCName (unless overridden for compatibility), so there's no
            // escaping that needs to be done here.
            var xPath = "//*[@" + idAttributeName + "=\"" + idValue + "\"]";

            if (Utils.AllowAmbiguousReferenceTargets()) {
                return document.SelectSingleNode(xPath) as XmlElement;
            }

            // http://www.w3.org/TR/xmldsig-core/#sec-ReferenceProcessingModel says that for the form URI="#chapter1":
            //
            //   Identifies a node-set containing the element with ID attribute value 'chapter1' ...
            //
            // Note that it uses the singular. Therefore, if the match is ambiguous, we should consider the document invalid.
            //
            // In this case, we'll treat it the same as having found nothing across all fallbacks (but shortcut so that we don't
            // fall into a trap of finding a secondary element which wasn't the originally signed one).

            var nodeList = document.SelectNodes(xPath);

            if (nodeList == null || nodeList.Count == 0) {
                return null;
            }

            if (nodeList.Count == 1) {
                return nodeList[0] as XmlElement;
            }

            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidReference"));
        }

        private static Boolean IsKeyTheCorrectAlgorithm(AsymmetricAlgorithm key, Type expectedType)
        {
            var actualType = key.GetType();

            if (actualType == expectedType)
                return true;

            // This check exists solely for compatibility with 4.6. Normally, we would expect "expectedType" to be the superclass type and
            // the actualType to be the subclass.
            if (expectedType.IsSubclassOf(actualType))
                return true;

            //
            // "expectedType" comes from the KeyAlgorithm property of a SignatureDescription. The BCL SignatureDescription classes have historically 
            // denoted provider-specific implementations ("RSACryptoServiceProvider") rather than the base class for the algorithm ("RSA"). We could
            // change those (at the risk of creating other compat problems) but we have no control over third party SignatureDescriptions.
            //
            // So, in the absence of a better approach, walk up the parent hierarchy until we find the ancestor that's a direct subclass of
            // AsymmetricAlgorithm and treat that as the algorithm identifier.
            //
            while (expectedType != null && expectedType.BaseType != typeof(AsymmetricAlgorithm))
            {
                expectedType = expectedType.BaseType;
            }

            if (expectedType == null)
                return false;   // SignatureDescription specified something that isn't even a subclass of AsymmetricAlgorithm. For compatibility with 4.6, return false rather throw.

            if (actualType.IsSubclassOf(expectedType))
                return true;

            return false;
        }

        public IX509Certificate SigningCertificate {get;set; }
    }
}
