using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    [Asn1SpecificObject("1.3.6.1.4.1.311.21.10")]
    internal class WceeCertificateApplicationPolicy : Asn1CertificatePoliciesExtension
        {
        public WceeCertificateApplicationPolicy(Asn1CertificateExtension source)
            : base(source)
            {
            }
        }
    }