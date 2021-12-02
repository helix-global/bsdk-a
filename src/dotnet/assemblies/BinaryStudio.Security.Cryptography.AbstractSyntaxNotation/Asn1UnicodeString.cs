﻿using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="BMPSTRING"/> type.
    /// </summary>
    internal sealed class Asn1UnicodeString : Asn1String
        {
        /// <inheritdoc/>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.UnicodeString; }}
        public override Encoding Encoding { get { return new UnicodeEncoding(true, false); }}

        internal Asn1UnicodeString(ReadOnlyMappingStream source, long forceoffset)
            : base(source, forceoffset)
            {
            }
        }
    }
