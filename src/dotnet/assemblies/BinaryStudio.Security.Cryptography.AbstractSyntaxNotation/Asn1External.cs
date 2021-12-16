using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="EXTERNAL"/> type.
    /// </summary>
    public sealed class Asn1External : Asn1UniversalObject
        {
        internal Asn1External(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        /// <summary>
        /// ASN.1 universal type. Always returns <see cref="Asn1ObjectType.External"/>.
        /// </summary>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.External; }}
        }
    }
