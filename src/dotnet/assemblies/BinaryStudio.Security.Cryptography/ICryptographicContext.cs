using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.Certificates;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography
    {
    public interface ICryptographicContext
        {
        /// <summary>
        /// Creates <see cref="IHashAlgorithm"/> using specified algorithm identifer.
        /// </summary>
        /// <param name="algid">Algorithm identifier.</param>
        /// <returns>Returns hash engine.</returns>
        IHashAlgorithm CreateHashAlgorithm(Oid algid);
        void CreateMessageSignature(Stream inputstream, Stream output, IList<IX509Certificate> certificates, CryptographicMessageFlags flags, RequestSecureStringEventHandler requesthandler);
        void VerifySignature(IX509Certificate subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags);
        void VerifySignature(IX509CertificateRevocationList subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags);
        void EncryptMessageBER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream);
        void EncryptMessageDER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream);
        void VerifyAttachedMessageSignature(Stream input, Stream output, out IList<IX509Certificate> certificates, IX509CertificateResolver finder);
        void VerifyDetachedMessageSignature(Stream input, Stream inputdata, out IList<IX509Certificate> certificates, IX509CertificateResolver finder);
        Boolean VerifySignature(out Exception e, IX509CertificateRevocationList subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags);
        IEnumerable<ICryptKey> Keys { get; }
        IX509CertificateChainPolicy GetChainPolicy(CertificateChainPolicy policy);

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
        void Verify(IX509Certificate certificate,IX509CertificateStorage store,OidCollection applicationpolicy,OidCollection issuancepolicy,TimeSpan timeout,DateTime datetime,CERT_CHAIN_FLAGS flags,CertificateChainPolicy policy, IntPtr chainengine);
        }
    }