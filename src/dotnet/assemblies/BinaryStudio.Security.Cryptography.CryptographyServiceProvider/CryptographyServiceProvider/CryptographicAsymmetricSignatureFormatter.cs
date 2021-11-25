using System;
using System.IO;
using System.Security.Cryptography;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    public sealed class CryptographicAsymmetricSignatureFormatter : AsymmetricSignatureFormatter
        {
        public IX509Certificate SigningCertificate { get;private set; }
        public override void SetKey(AsymmetricAlgorithm key)
            {
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            if (key is CertificateAsymmetricAlgorithm certificate) {
                SigningCertificate = certificate.Certificate;
                return;
                }
            throw new ArgumentOutOfRangeException(nameof(key));
            }

        public override void SetHashAlgorithm(String strName)
            {
            }

        public override Byte[] CreateSignature(Byte[] digest) {
            using (var r = new MemoryStream()) {
                CryptHashAlgorithm.CreateSignature(SigningCertificate, digest, r, KeySpec.Exchange|KeySpec.Signature);
                return r.ToArray();
                }
            }

        public CryptographicAsymmetricSignatureFormatter()
            {
            }

        public CryptographicAsymmetricSignatureFormatter(AsymmetricAlgorithm key)
            {
            SetKey(key);
            }
        }
    }