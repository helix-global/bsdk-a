using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public class Asn1RelativeObjectIdentifier : Asn1ObjectIdentifier
        {
        public Asn1RelativeObjectIdentifier(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        public override Asn1ObjectType Type { get { return Asn1ObjectType.RelativeObjectIdentifier; }}
        }
    }
