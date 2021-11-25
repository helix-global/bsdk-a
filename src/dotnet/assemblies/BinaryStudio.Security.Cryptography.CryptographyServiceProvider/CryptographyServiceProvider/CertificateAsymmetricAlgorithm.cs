using System;
using System.Security.Cryptography;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    public class CertificateAsymmetricAlgorithm : AsymmetricAlgorithm
        {
        public IX509Certificate Certificate { get; }
        public CertificateAsymmetricAlgorithm(IX509Certificate certificate)
            {
            if (certificate == null) { throw new ArgumentNullException(nameof(certificate)); }
            Certificate = certificate;
            }

        /// <summary>When overridden in a derived class, reconstructs an <see cref="AsymmetricAlgorithm"/> object from an XML string.
        /// Otherwise, throws a <see cref="NotImplementedException"/>.</summary>
        /// <param name="xmlString">The XML string to use to reconstruct the <see cref="AsymmetricAlgorithm"/> object.</param>
        public override void FromXmlString(String xmlString)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, creates and returns an XML string representation of the current <see cref="AsymmetricAlgorithm"/> object.
        /// Otherwise, throws a <see cref="NotImplementedException"/>.</summary>
        /// <param name="includePrivateParameters"><see langword="true"/> to include private parameters; otherwise, <see langword="false"/>.</param>
        /// <returns>An XML string encoding of the current <see cref="AsymmetricAlgorithm"/> object.</returns>
        public override String ToXmlString(Boolean includePrivateParameters)
            {
            throw new NotImplementedException();
            }

        public override String SignatureAlgorithm { get; }
        public override String KeyExchangeAlgorithm { get; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return Certificate.ToString();
            }
        }
    }