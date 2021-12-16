using System;
using System.Diagnostics;
using System.Security.Cryptography;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    public class CryptographicAsymmetricSignatureDeformatter : AsymmetricSignatureDeformatter, IDisposable
        {
        private SCryptographicContext context;
        private IX509Certificate certificate;
        public override void SetKey(AsymmetricAlgorithm key)
            {
            var certificate = (key as CertificateAsymmetricAlgorithm)?.Certificate;
            if (certificate != null) {
                this.certificate = certificate;
                context = new SCryptographicContext(
                    certificate.SignatureAlgorithm,
                    CryptographicContextFlags.CRYPT_SILENT|
                    CryptographicContextFlags.CRYPT_VERIFYCONTEXT);
                return;
                }
            throw new NotSupportedException();
            }

        public override void SetHashAlgorithm(String strName)
            {
                throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, verifies the signature for the specified data.</summary>
        /// <param name="digest">The data signed with <paramref name="signature"/>.</param>
        /// <param name="signature">The signature to be verified for <paramref name="digest"/>.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="signature"/> matches the signature computed using the specified hash algorithm and key on <paramref name="digest"/>; otherwise, <see langword="false"/>.</returns>
        public override Boolean VerifySignature(Byte[] digest, Byte[] signature)
            {
            Debug.Print($"digestvalue:{Convert.ToBase64String(digest)}");
            Debug.Print($"signature:{Convert.ToBase64String(signature)}");
            using (var hashalg = (IHashOperation)context.CreateHashAlgorithm(certificate.HashAlgorithm)) {
                using (var key = context.ImportPublicKey(certificate.Handle)) {
                    return hashalg.VerifySignature(out var e, signature, digest, key);
                    }
                }
            }

        protected virtual void Dispose(Boolean disposing) {
            if (disposing) {
                if (context != null) {
                    context.Dispose();
                    context = null;
                    }
                }
            }

        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        }
    }