using System;
using System.Security.Cryptography;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    public class Gost3410_12_512 : AsymmetricAlgorithm
        {
        public override void FromXmlString(String xmlString)
            {
            throw new NotImplementedException();
            }

        public override String ToXmlString(Boolean includePrivateParameters)
            {
            throw new NotImplementedException();
            }

        public override String SignatureAlgorithm { get; }
        public override String KeyExchangeAlgorithm { get; }
        }
    }