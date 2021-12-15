using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.Security.Cryptography.Certificates;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    public class CryptographicContext : CryptographicObject, ICryptographicContext
        {
        public const String XmlDsigSHA1                      = "http://www.w3.org/2000/09/xmldsig#sha1";
        public const String XmlDsigDSA                       = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
        public const String XmlDsigRSASHA1                   = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        public const String XmlDsigHMACSHA1                  = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";
        public const String XmlDsigSHA256                    = "http://www.w3.org/2001/04/xmlenc#sha256";
        public const String XmlDsigRSASHA256                 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        public const String XmlDsigSHA384                    = "http://www.w3.org/2001/04/xmldsig-more#sha384";
        public const String XmlDsigRSASHA384                 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384";
        public const String XmlDsigSHA512                    = "http://www.w3.org/2001/04/xmlenc#sha512";
        public const String XmlDsigRSASHA512                 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";
        public const String URI_GOST_CIPHER	                 = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gost28147";
        public const String	URI_GOST_DIGEST	                 = "http://www.w3.org/2001/04/xmldsig-more#gostr3411";
        public const String	URI_GOST_HMAC_GOSTR3411	         = "http://www.w3.org/2001/04/xmldsig-more#hmac-gostr3411";
        public const String	URI_GOST_SIGN                    = "http://www.w3.org/2001/04/xmldsig-more#gostr34102001-gostr3411";
        public const String	URI_GOST_TRANSPORT               = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:transport-gost2001";
        public const String	URI_GOST_TRANSPORT_GOST_2012_256 = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:transport-gost2012-256";
        public const String	URI_GOST_TRANSPORT_GOST_2012_512 = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:transport-gost2012-512";
        public const String	URN_GOST_DIGEST                  = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr3411";
        public const String	URN_GOST_DIGEST_2012_256         = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34112012-256";
        public const String	URN_GOST_DIGEST_2012_512         = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34112012-512";
        public const String	URN_GOST_HMAC_GOSTR3411          = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:hmac-gostr3411";
        public const String	URN_GOST_SIGN                    = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34102001-gostr3411";
        public const String	URN_GOST_SIGN_2012_256           = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34102012-gostr34112012-256";
        public const String	URN_GOST_SIGN_2012_512           = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34102012-gostr34112012-512";

        public override IntPtr Handle { get; }
        protected internal override ILogger Logger { get; }
        private readonly ICryptographicContext UnderlyingObject;

        /// <summary>
        /// Constructs <see cref="CryptographicContext"/> using specified <paramref name="providertype"/> and flags.
        /// </summary>
        /// <param name="providertype">Type of provider (<see cref="CRYPT_PROVIDER_TYPE"/>).</param>
        /// <param name="flags">Provider flags.</param>
        public CryptographicContext(CRYPT_PROVIDER_TYPE providertype, CryptographicContextFlags flags) {
            switch (providertype) {
                case CRYPT_PROVIDER_TYPE.OPENSSL: UnderlyingObject = new OpenSSLCryptographicContext(flags); break;
                default: UnderlyingObject = new SCryptographicContext(providertype, flags); break;
                }
            }

        public IHashAlgorithm CreateHashAlgorithm(Oid algid)
            {
            return UnderlyingObject.CreateHashAlgorithm(algid);
            }

        public void CreateMessageSignature(Stream inputstream, Stream output, IList<IX509Certificate> certificates, CryptographicMessageFlags flags, RequestSecureStringEventHandler requesthandler)
            {
            UnderlyingObject.CreateMessageSignature(inputstream, output, certificates, flags, requesthandler);
            }

        public Boolean VerifyCertificateSignature(out Exception e, IX509Certificate subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags)
            {
            return UnderlyingObject.VerifyCertificateSignature(out e, subject, issuer, flags);
            }

        public void EncryptMessageBER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream)
            {
            UnderlyingObject.EncryptMessageBER(algid, recipients, inputstream, outputstream);
            }

        public void EncryptMessageDER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream)
            {
            UnderlyingObject.EncryptMessageDER(algid, recipients, inputstream, outputstream);
            }

        public void VerifyAttachedMessageSignature(Stream input, Stream output, out IList<IX509Certificate> certificates, IX509CertificateResolver finder)
            {
            UnderlyingObject.VerifyAttachedMessageSignature(
                input, output, out certificates, finder);
            }

        public void VerifyDetachedMessageSignature(Stream input, Stream inputdata, out IList<IX509Certificate> certificates, IX509CertificateResolver finder)
            {
            UnderlyingObject.VerifyDetachedMessageSignature(
                input, inputdata, out certificates, finder);
            }

        static CryptographicContext()
            {
            CryptoConfig.AddAlgorithm(typeof(Gost3410_12_256_SignatureDescription), URN_GOST_SIGN_2012_256);
            CryptoConfig.AddAlgorithm(typeof(Gost3410_12_512_SignatureDescription), URN_GOST_SIGN_2012_512);
            CryptoConfig.AddAlgorithm(typeof(Gost3411_12_256), URN_GOST_DIGEST_2012_256);
            CryptoConfig.AddAlgorithm(typeof(Gost3411_12_512), URN_GOST_DIGEST_2012_512);
            CryptoConfig.AddAlgorithm(typeof(Gost3410_12_256_SignatureDescription), ObjectIdentifiers.szOID_CP_GOST_R3410_12_256);
            CryptoConfig.AddAlgorithm(typeof(Gost3410_12_512_SignatureDescription), ObjectIdentifiers.szOID_CP_GOST_R3410_12_512);
            }

        #region M:OIDToXmlDSig(Oid):String
        public static String OIDToXmlDSig(Oid algid)
            {
            if (algid == null) { throw new ArgumentNullException(nameof(algid)); }
            switch (algid.Value) {
                case ObjectIdentifiers.szOID_NIST_sha256: { return XmlDsigSHA256; }
                case ObjectIdentifiers.szOID_NIST_sha384: { return XmlDsigSHA384; }
                case ObjectIdentifiers.szOID_NIST_sha512: { return XmlDsigSHA512; }
                case ObjectIdentifiers.szOID_CP_GOST_R3411_12_256: { return URN_GOST_DIGEST_2012_256; }
                case ObjectIdentifiers.szOID_CP_GOST_R3411_12_512: { return URN_GOST_DIGEST_2012_512; }
                case ObjectIdentifiers.szOID_CP_GOST_R3410_12_256: { return URN_GOST_SIGN_2012_256; }
                case ObjectIdentifiers.szOID_CP_GOST_R3410_12_512: { return URN_GOST_SIGN_2012_512; }
                default: throw new ArgumentOutOfRangeException(nameof(algid));
                }
            }
        #endregion

        public static IEnumerable<RegisteredProviderInfo> AvailableProviders { get {
            foreach (var i in SCryptographicContext.RegisteredProviders) { yield return i; }
            //#if FEATURE_OPENSSL
            //yield return new RegisteredProviderInfo(CRYPT_PROVIDER_TYPE.OPENSSL);
            //#endif
            }}
        }
    }