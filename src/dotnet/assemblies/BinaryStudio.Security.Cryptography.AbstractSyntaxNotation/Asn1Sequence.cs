using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="SEQUENCE"/> type.
    /// </summary>
    public sealed class Asn1Sequence : Asn1UniversalObject
        {
        internal Asn1Sequence(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        public override Asn1ObjectType Type { get { return Asn1ObjectType.Sequence; }}
        }
    }
