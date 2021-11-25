﻿using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public sealed class Asn1Boolean : Asn1UniversalObject
        {
        public Boolean Value { get;private set; }
        internal Asn1Boolean(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        public override Asn1ObjectType Type { get { return Asn1ObjectType.Boolean; }}
        public static implicit operator Boolean(Asn1Boolean source) {
            return source.Value;
            }

        protected internal  override Boolean Decode()
            {
            Value = Content.ReadByte() != 0;
            return true;
            }
        }
    }