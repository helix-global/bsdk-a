using System;
using System.Diagnostics;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public abstract class Asn1UniversalObject : Asn1Object, IAsn1Object
        {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Asn1ObjectClass Class { get { return Asn1ObjectClass.Universal; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] SByte IAsn1Object.Type { get { return (SByte)Type; }}
        public abstract Asn1ObjectType Type { get; }

        protected Asn1UniversalObject(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        internal Asn1UniversalObject()
            {
            }

        public override Boolean Equals(Asn1Object other)
            {
            return base.Equals(other) && (((Asn1UniversalObject)other).Type == Type);
            }
        }
    }
