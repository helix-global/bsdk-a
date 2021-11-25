using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.PublicKeyInfrastructure;

namespace BinaryStudio.Security.Cryptography.Certificates.AbstractSyntaxNotation
    {
    internal sealed class Asn1CertificateSubjectPublicKeyInfo : Asn1LinkObject
        {
        public X509AlgorithmIdentifier AlgorithmIdentifier { get; }
        internal Asn1CertificateSubjectPublicKeyInfo(Asn1Object o)
            : base(o)
            {
            State |= ObjectState.Failed;
            if (o is Asn1Sequence u)
                {
                if (u[0] is Asn1Sequence i)
                    {
                    AlgorithmIdentifier = new X509AlgorithmIdentifier(i);
                    }
                State &= ~ObjectState.Failed;
                }
            }
        }
    }