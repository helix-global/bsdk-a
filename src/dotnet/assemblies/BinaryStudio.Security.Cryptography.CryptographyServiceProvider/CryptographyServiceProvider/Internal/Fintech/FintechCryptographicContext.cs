﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.Certificates;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    public class FintechCryptographicContext : CryptographicObject, ICryptographicContext
        {
        private IntPtr _handle;
        protected internal override ILogger Logger { get; }
        private IEnumerable<ICryptKey> _keys;
        private IFintechLibrary UnderlyingObject;
        String ICryptographicContext.ProviderName { get { return "Fintech Cryptographic Library Context."; }}
        CRYPT_PROVIDER_TYPE ICryptographicContext.ProviderType { get { return CRYPT_PROVIDER_TYPE.FINTECH; }}
        Boolean ICryptographicContext.UseMachineKeySet { get { return false; }}
        public static String ReportPrefix { get;set; }

        /// <summary>
        /// Constructs <see cref="FintechCryptographicContext"/> instance using <paramref name="flags"/>.
        /// </summary>
        /// <param name="logger">Reference to logger.</param>
        /// <param name="flags">Provider flags.</param>
        public FintechCryptographicContext(ILogger logger, CryptographicContextFlags flags)
            {
            Logger = logger ?? DefaultLogger;
            }

        public override IntPtr Handle
            {
            get { return _handle; }
            }

        IHashAlgorithm ICryptographicContext.CreateHashAlgorithm(Oid algid)
            {
            throw new NotImplementedException();
            }

        void ICryptographicContext.CreateMessageSignature(Stream inputstream, Stream output, IList<IX509Certificate> certificates, CryptographicMessageFlags flags,
            RequestSecureStringEventHandler requesthandler)
            {
            throw new NotImplementedException();
            }

        void ICryptographicContext.VerifySignature(IX509Certificate subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags)
            {
            throw new NotImplementedException();
            }

        void ICryptographicContext.VerifySignature(IX509CertificateRevocationList subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags)
            {
            throw new NotImplementedException();
            }

        void ICryptographicContext.EncryptMessageBER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream)
            {
            throw new NotImplementedException();
            }

        void ICryptographicContext.EncryptMessageDER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream)
            {
            throw new NotImplementedException();
            }

        void ICryptographicContext.VerifyAttachedMessageSignature(Stream input, Stream output, out IList<IX509Certificate> certificates,
            IX509CertificateResolver finder, VerificationPolicy policy) {
            certificates = null;
            if (policy == VerificationPolicy.Icao) {
                EnsureCore();
                UnderlyingObject.VerifyMrtdMessage(input);
                return;
                }
            throw new NotImplementedException();
            }

        void ICryptographicContext.VerifyDetachedMessageSignature(Stream input, Stream inputdata, out IList<IX509Certificate> certificates,
            IX509CertificateResolver finder)
            {
            throw new NotImplementedException();
            }

        Boolean ICryptographicContext.VerifySignature(out Exception e, IX509CertificateRevocationList subject, IX509Certificate issuer,
            CRYPT_VERIFY_CERT_SIGN flags)
            {
            throw new NotImplementedException();
            }

        IEnumerable<ICryptKey> ICryptographicContext.Keys
            {
            get { return _keys; }
            }

        IX509CertificateChainPolicy ICryptographicContext.GetChainPolicy(CertificateChainPolicy policy) {
            return null;
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
            CertificateChainPolicy policy, IntPtr chainengine) {
            if (policy == CertificateChainPolicy.Icao) {
                EnsureCore();
                UnderlyingObject.VerifyMrtdCertificate(certificate.Handle);
                return;
                }
            throw new NotSupportedException();
            }

        private void EnsureCore() {
            lock (this)
                {
                UnderlyingObject = UnderlyingObject ?? new FintechLibrary(Logger);
                }
            }

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
            {
            Dispose(ref UnderlyingObject);
            base.Dispose(disposing);
            }
        }
    }