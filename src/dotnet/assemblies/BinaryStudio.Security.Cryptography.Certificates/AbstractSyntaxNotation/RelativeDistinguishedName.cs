using System;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.Certificates.AbstractSyntaxNotation
    {
    public class RelativeDistinguishedName : Asn1LinkObject<Asn1Sequence>
        {
        public RelativeDistinguishedName(Asn1Sequence source)
            : base(source)
            {
            throw new NotImplementedException();
            }
        }
    }