using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="EOC"/> type.
    /// </summary>
    public sealed class Asn1EndOfContent : Asn1UniversalObject
        {
        internal Asn1EndOfContent(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        public override Asn1ObjectType Type { get { return Asn1ObjectType.EndOfContent; }}
        }
    }
