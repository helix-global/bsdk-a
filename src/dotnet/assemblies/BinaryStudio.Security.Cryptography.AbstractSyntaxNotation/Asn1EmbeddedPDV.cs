using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    class Asn1EmbeddedPDV : Asn1UniversalObject
        {
        public Asn1EmbeddedPDV(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        public override Asn1ObjectType Type { get { return Asn1ObjectType.EmbeddedPDV; }}
        }
    }
