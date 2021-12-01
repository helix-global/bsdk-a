using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="SET"/> type.
    /// </summary>
    public sealed class Asn1Set : Asn1UniversalObject
        {
        internal Asn1Set(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        public override Asn1ObjectType Type { get { return Asn1ObjectType.Set; }}
        }
    }
