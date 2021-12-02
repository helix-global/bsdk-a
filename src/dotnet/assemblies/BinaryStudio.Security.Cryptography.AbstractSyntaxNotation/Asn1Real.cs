﻿using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="REAL"/> type.
    /// </summary>
    public sealed class Asn1Real : Asn1UniversalObject
        {
        internal Asn1Real(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        /// <summary>
        /// ASN.1 universal type. Always returns <see cref="Asn1ObjectType.Real"/>.
        /// </summary>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.Real; }}
        }
    }
