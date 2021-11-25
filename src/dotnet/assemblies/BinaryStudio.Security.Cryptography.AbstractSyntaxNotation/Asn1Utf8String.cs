using System;
using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    internal class Asn1Utf8String : Asn1String
        {
        public override Asn1ObjectType Type { get { return Asn1ObjectType.Utf8String; }}
        public override Encoding Encoding { get { return Encoding.UTF8; }}

        public Asn1Utf8String(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }
        }
    }
