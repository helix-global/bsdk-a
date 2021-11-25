using System;
using BinaryStudio.Security.Cryptography.Certificates.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    internal sealed class EdiPartyName : Asn1GeneralNameObject
        {
        public EdiPartyName(Asn1ContextSpecificObject source)
            : base(source)
            {
            throw new NotImplementedException();
            }

        protected override X509GeneralNameType InternalType { get { return X509GeneralNameType.EDIParty; }}
        }
    }