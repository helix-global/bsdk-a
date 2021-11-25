﻿using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    internal class Asn1NumericString : Asn1String
        {
        public override Asn1ObjectType Type { get { return Asn1ObjectType.NumericString; }}
        public override Encoding Encoding { get { return Encoding.ASCII; }}

        public Asn1NumericString(ReadOnlyMappingStream source, long forceoffset)
            : base(source, forceoffset)
            {
            }
        }
    }
