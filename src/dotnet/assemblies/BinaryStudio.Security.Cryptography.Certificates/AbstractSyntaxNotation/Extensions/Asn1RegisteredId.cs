using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    internal class Asn1RegisteredId : Asn1GeneralNameObject
        {
        public Asn1RegisteredId(Asn1ContextSpecificObject source)
            : base(source)
            {
                //throw new NotImplementedException();
            }

        protected override X509GeneralNameType InternalType { get { return X509GeneralNameType.ObjectIdentifier; }}
        }
    }