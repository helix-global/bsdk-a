using System.Security.Cryptography;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    public sealed class Gost3410_12_512_SignatureDescription : SignatureDescription
        {
        public Gost3410_12_512_SignatureDescription() {
            KeyAlgorithm = typeof(CertificateAsymmetricAlgorithm).AssemblyQualifiedName;
            DigestAlgorithm = typeof(Gost3411_12_512).AssemblyQualifiedName;
            DeformatterAlgorithm = typeof(CryptographicAsymmetricSignatureDeformatter).AssemblyQualifiedName;
            }

        /// <summary>Creates an <see cref="AsymmetricSignatureFormatter" /> instance with the specified key using the <see cref="P:System.Security.Cryptography.SignatureDescription.FormatterAlgorithm" /> property.</summary>
        /// <returns>The newly created <see cref="AsymmetricSignatureFormatter" /> instance.</returns>
        /// <param name="key">The key to use in the <see cref="AsymmetricSignatureFormatter" />.</param>
        public override AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
            {
            return new CryptographicAsymmetricSignatureFormatter(key);
            }
        }
    }