using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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
        Boolean VerifyCertificateSignature(out Exception e, IX509Certificate subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags);
        void EncryptMessageBER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream);
        void EncryptMessageDER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream);
        void VerifyAttachedMessageSignature(Stream input, Stream output, out IList<IX509Certificate> certificates, IX509CertificateResolver finder);
        void VerifyDetachedMessageSignature(Stream input, Stream inputdata, out IList<IX509Certificate> certificates, IX509CertificateResolver finder);
        }
    }