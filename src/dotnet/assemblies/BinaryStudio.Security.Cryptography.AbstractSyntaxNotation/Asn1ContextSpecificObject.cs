using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public class Asn1ContextSpecificObject : Asn1Object, IAsn1Object
        {
        public override Asn1ObjectClass Class { get { return Asn1ObjectClass.ContextSpecific; }}
        public SByte Type { get; }

        public Asn1ContextSpecificObject(ReadOnlyMappingStream source, Int64 forceoffset, SByte type)
            : base(source, forceoffset)
            {
            Type = type;
            }

        public override Boolean Equals(Asn1Object other)
            {
            return base.Equals(other) && (((Asn1ContextSpecificObject)other).Type == Type);
            }
        }
    }
