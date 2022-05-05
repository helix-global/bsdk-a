using System;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    internal sealed class X400Address : Asn1GeneralNameObject
        {
        public X400Address(Asn1ContextSpecificObject source)
            : base(source)
            {
            throw new NotImplementedException();
            }

        protected override X509GeneralNameType InternalType { get { return X509GeneralNameType.X400Address; }}
        }
    }