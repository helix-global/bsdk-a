using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="EMBEDDED PDV"/> type.
    /// </summary>
    public sealed class Asn1EmbeddedPDV : Asn1UniversalObject
        {
        internal Asn1EmbeddedPDV(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        public override Asn1ObjectType Type { get { return Asn1ObjectType.EmbeddedPDV; }}
        }
    }
