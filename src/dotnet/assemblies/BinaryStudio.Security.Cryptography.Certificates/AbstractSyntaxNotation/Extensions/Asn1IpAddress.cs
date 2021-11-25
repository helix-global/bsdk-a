using BinaryStudio.Security.Cryptography.Certificates.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    internal class Asn1IpAddress : Asn1GeneralNameObject
        {
        public Asn1IpAddress(Asn1ContextSpecificObject source)
            : base(source)
            {
            }

        protected override X509GeneralNameType InternalType { get { return X509GeneralNameType.OctetString; }}
        }
    }