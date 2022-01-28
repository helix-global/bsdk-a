﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.Certificates;
using Microsoft.Win32;

// ReSharper disable VirtualMemberCallInConstructor

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
        /// <param name="logger">Reference to logger.</param>
        /// <param name="providertype">Type of provider (<see cref="CRYPT_PROVIDER_TYPE"/>).</param>
        /// <param name="flags">Provider flags.</param>
        public CryptographicContext(ILogger logger, CRYPT_PROVIDER_TYPE providertype, CryptographicContextFlags flags) {
            Logger = logger ?? DefaultLogger;
            switch (providertype) {
                case CRYPT_PROVIDER_TYPE.OPENSSL: UnderlyingObject = new OpenSSLCryptographicContext(Logger, flags); break;
                case CRYPT_PROVIDER_TYPE.FINTECH: UnderlyingObject = new FintechCryptographicContext(Logger, flags); break;
                case  0: UnderlyingObject = new DefaultCryptographicContext(Logger, flags); break;
                default: UnderlyingObject = new SCryptographicContext(Logger, providertype, flags); break;
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

        public void VerifySignature(IX509Certificate subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags)
            {
            UnderlyingObject.VerifySignature(subject, issuer, flags);
            }

        public void VerifySignature(IX509CertificateRevocationList subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags)
            {
            UnderlyingObject.VerifySignature(subject, issuer, flags);
            }

        public void EncryptMessageBER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream)
            {
            UnderlyingObject.EncryptMessageBER(algid, recipients, inputstream, outputstream);
            }

        public void EncryptMessageDER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream)
            {
            UnderlyingObject.EncryptMessageDER(algid, recipients, inputstream, outputstream);
            }

        public void VerifyAttachedMessageSignature(Stream input, Stream output, out IList<IX509Certificate> certificates, IX509CertificateResolver finder, VerificationPolicy policy)
            {
            UnderlyingObject.VerifyAttachedMessageSignature(
                input, output, out certificates,
                finder, policy);
            }

        public void VerifyDetachedMessageSignature(Stream input, Stream inputdata, out IList<IX509Certificate> certificates, IX509CertificateResolver finder)
            {
            UnderlyingObject.VerifyDetachedMessageSignature(
                input, inputdata, out certificates, finder);
            }

        public Boolean VerifySignature(out Exception e, IX509CertificateRevocationList subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags)
            {
            return UnderlyingObject.VerifySignature(out e, subject, issuer, flags);
            }

        public IEnumerable<ICryptKey> Keys { get { return UnderlyingObject.Keys; }}
        public IX509CertificateChainPolicy GetChainPolicy(CertificateChainPolicy policy) {
            var r = UnderlyingObject?.GetChainPolicy(policy);
            if (r == null) {
                switch (policy) {
                    case CertificateChainPolicy.CERT_CHAIN_POLICY_BASE:              r = X509CertificateChainPolicy.POLICY_BASE;                break;
                    case CertificateChainPolicy.CERT_CHAIN_POLICY_AUTHENTICODE:      r = X509CertificateChainPolicy.POLICY_AUTHENTICODE;        break;
                    case CertificateChainPolicy.CERT_CHAIN_POLICY_AUTHENTICODE_TS:   r = X509CertificateChainPolicy.POLICY_AUTHENTICODE_TS;     break;
                    case CertificateChainPolicy.CERT_CHAIN_POLICY_SSL:               r = X509CertificateChainPolicy.POLICY_SSL;                 break;
                    case CertificateChainPolicy.CERT_CHAIN_POLICY_BASIC_CONSTRAINTS: r = X509CertificateChainPolicy.POLICY_BASIC_CONSTRAINTS;   break;
                    case CertificateChainPolicy.CERT_CHAIN_POLICY_NT_AUTH:           r = X509CertificateChainPolicy.POLICY_NT_AUTH;             break;
                    case CertificateChainPolicy.CERT_CHAIN_POLICY_MICROSOFT_ROOT:    r = X509CertificateChainPolicy.POLICY_MICROSOFT_ROOT;      break;
                    case CertificateChainPolicy.CERT_CHAIN_POLICY_EV:                r = X509CertificateChainPolicy.POLICY_EV;                  break;
                    case CertificateChainPolicy.CERT_CHAIN_POLICY_SSL_F12:           r = X509CertificateChainPolicy.POLICY_SSL_F12;             break;
                    case CertificateChainPolicy.CERT_CHAIN_POLICY_SSL_HPKP_HEADER:   r = X509CertificateChainPolicy.POLICY_SSL_HPKP_HEADER;     break;
                    case CertificateChainPolicy.CERT_CHAIN_POLICY_THIRD_PARTY_ROOT:  r = X509CertificateChainPolicy.POLICY_THIRD_PARTY_ROOT;    break;
                    case CertificateChainPolicy.CERT_CHAIN_POLICY_SSL_KEY_PIN:       r = X509CertificateChainPolicy.POLICY_SSL_KEY_PIN;         break;
                    case CertificateChainPolicy.Icao:                                r = X509CertificateChainPolicy.IcaoCertificateChainPolicy; break;
                    default: throw new ArgumentOutOfRangeException(nameof(policy), policy, null);
                    }
                }
            return r;
            }

        /// <summary>
        /// Verify a certificate using certificate chain to check its validity, including its compliance with any specified validity policy criteria.
        /// </summary>
        /// <param name="certificate">Certificate to verify.</param>
        /// <param name="store">Certificate store.</param>
        /// <param name="applicationpolicy">Application policy.</param>
        /// <param name="issuancepolicy">Issuance policy.</param>
        /// <param name="timeout">Optional time, before revocation checking times out. This member is optional.</param>
        /// <param name="datetime">Indicates the time for which the chain is to be validated.</param>
        /// <param name="flags">Flag values that indicate special processing.</param>
        /// <param name="policy">Certificate policy.</param>
        /// <param name="chainengine">A handle of the chain engine.</param>
        public void Verify(IX509Certificate certificate, IX509CertificateStorage store, OidCollection applicationpolicy,
            OidCollection issuancepolicy, TimeSpan timeout, DateTime datetime, CERT_CHAIN_FLAGS flags,
            CertificateChainPolicy policy, IntPtr chainengine)
            {
            UnderlyingObject.Verify(
                certificate,store,
                applicationpolicy,issuancepolicy,
                timeout,datetime,flags,
                policy,chainengine);
            }

        static CryptographicContext()
            {
            CryptoConfig.AddAlgorithm(typeof(Gost3410_12_256_SignatureDescription), URN_GOST_SIGN_2012_256);
            CryptoConfig.AddAlgorithm(typeof(Gost3410_12_512_SignatureDescription), URN_GOST_SIGN_2012_512);
            CryptoConfig.AddAlgorithm(typeof(Gost3411_12_256), URN_GOST_DIGEST_2012_256);
            CryptoConfig.AddAlgorithm(typeof(Gost3411_12_512), URN_GOST_DIGEST_2012_512);
            CryptoConfig.AddAlgorithm(typeof(Gost3410_12_256_SignatureDescription), ObjectIdentifiers.szOID_CP_GOST_R3410_12_256);
            CryptoConfig.AddAlgorithm(typeof(Gost3410_12_512_SignatureDescription), ObjectIdentifiers.szOID_CP_GOST_R3410_12_512);
            CryptoConfig.AddAlgorithm(typeof(SHA384Managed), ObjectIdentifiers.szOID_NIST_sha384);
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