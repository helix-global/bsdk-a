﻿using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="OBJECT DESCRIPTOR"/> type.
    /// </summary>
    public class Asn1ObjectDescriptor : Asn1UniversalObject
        {
        internal Asn1ObjectDescriptor(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        /// <inheritdoc/>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.ObjectDescriptor; }}
        }
    }
