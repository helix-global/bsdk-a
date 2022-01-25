using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.Certificates;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class OpenSSLCryptographicContext: CryptographicObject, ICryptographicContext
        {
        public readonly CryptographicContextFlags flags;
        public override IntPtr Handle { get; }
        protected internal override ILogger Logger { get; }

        /// <summary>
        /// Constructs <see cref="OpenSSLCryptographicContext"/> instance using <paramref name="flags"/>.
        /// </summary>
        /// <param name="logger">Reference to logger.</param>
        /// <param name="flags">Provider flags.</param>
        public OpenSSLCryptographicContext(ILogger logger, CryptographicContextFlags flags)
            {
            Logger = logger ?? DefaultLogger;
            this.flags = flags;
            }

        /// <summary>
        /// Creates <see cref="IHashAlgorithm"/> using specified algorithm identifer.
        /// </summary>
        /// <param name="algid">Algorithm identifier.</param>
        /// <returns>Returns hash engine.</returns>
        public IHashAlgorithm CreateHashAlgorithm(Oid algid)
            {
            if (algid == null) { throw new ArgumentNullException(nameof(algid)); }
            return OpenSSLHashAlgorithm.Create(this, algid.Value);
            }

        public void CreateMessageSignature(Stream inputstream, Stream output, IList<IX509Certificate> certificates, CryptographicMessageFlags flags,RequestSecureStringEventHandler requesthandler)
            {
            throw new NotImplementedException();
            }

        public void VerifySignature(IX509Certificate subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags)
            {
            throw new NotImplementedException();
            }

        public void VerifySignature(IX509CertificateRevocationList subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags)
            {
            throw new NotImplementedException();
            }

        public void EncryptMessageBER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream)
            {
            throw new NotImplementedException();
            }

        public void EncryptMessageDER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream)
            {
            throw new NotImplementedException();
            }

        void ICryptographicContext.VerifyAttachedMessageSignature(Stream input, Stream output, out IList<IX509Certificate> certificates,IX509CertificateResolver finder)
            {
            throw new NotImplementedException();
            }

        void ICryptographicContext.VerifyDetachedMessageSignature(Stream input, Stream inputdata, out IList<IX509Certificate> certificates,IX509CertificateResolver finder)
            {
            throw new NotImplementedException();
            }

        public Boolean VerifySignature(out Exception e, IX509CertificateRevocationList subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags)
            {
            throw new NotImplementedException();
            }

        public IEnumerable<ICryptKey> Keys { get {
            yield break;
            }}

        IX509CertificateChainPolicy ICryptographicContext.GetChainPolicy(CertificateChainPolicy policy) { return null; }

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
        void ICryptographicContext.Verify(IX509Certificate certificate, IX509CertificateStorage store, OidCollection applicationpolicy,
            OidCollection issuancepolicy, TimeSpan timeout, DateTime datetime, CERT_CHAIN_FLAGS flags,
            CertificateChainPolicy policy, IntPtr chainengine)
            {
            throw new NotImplementedException();
            }

        private static readonly Object o = new Object();
        private static IOpenSSLLibrary core;

        public IOpenSSLLibrary Core { get {
            lock(o)
                {
                if (core == null) {
                    var filename = String.Empty;
                    switch (Environment.OSVersion.Platform)
                        {
                        case PlatformID.Win32S:
                        case PlatformID.Win32Windows:
                        case PlatformID.Win32NT:
                        case PlatformID.WinCE:
                            {
                            #if NET35
                            filename = (IntPtr.Size == 8)
                                ? "libcrypto-1_1-x64.dll"
                                : "libcrypto-1_1.dll";
                            #else
                            filename = Environment.Is64BitProcess
                                ? "libcrypto-1_1-x64.dll"
                                : "libcrypto-1_1.dll";
                            #endif
                            }
                            break;
                        case PlatformID.Unix:
                            {
                            filename = "libcrypto.so.1.0.0";
                            }
                            break;
                        case PlatformID.Xbox:
                        case PlatformID.MacOSX:
                        default: { throw new NotSupportedException($"Platform {{{Environment.OSVersion.Platform}}} is not supported."); }
                        }
                    core = new OpenSSLLibrary(filename);
                    }
                return core;
                }
            }}
        }
    }