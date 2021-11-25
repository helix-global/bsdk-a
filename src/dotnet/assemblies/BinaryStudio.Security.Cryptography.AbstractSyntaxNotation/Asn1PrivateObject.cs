using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public class Asn1PrivateObject : Asn1Object, IAsn1Object
        {
        public override Asn1ObjectClass Class { get { return Asn1ObjectClass.Private; }}
        public SByte Type { get; }

        public Asn1PrivateObject(ReadOnlyMappingStream source, Int64 forceoffset, SByte type)
            : base(source, forceoffset)
            {
            Type = type;
            }

        public override Boolean Equals(Asn1Object other)
            {
            return base.Equals(other) && (((Asn1PrivateObject)other).Type == Type);
            }
        }
    }
