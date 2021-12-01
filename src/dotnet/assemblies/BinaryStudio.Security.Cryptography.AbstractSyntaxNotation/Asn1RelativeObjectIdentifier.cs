using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="RELATIVE-OID"/> type.
    /// </summary>
    public sealed class Asn1RelativeObjectIdentifier : Asn1ObjectIdentifier
        {
        internal Asn1RelativeObjectIdentifier(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        public override Asn1ObjectType Type { get { return Asn1ObjectType.RelativeObjectIdentifier; }}
        }
    }
