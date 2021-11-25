using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    internal class Asn1IA5String : Asn1String
        {
        public override Asn1ObjectType Type { get { return Asn1ObjectType.IA5String; }}
        public override Encoding Encoding { get { return Encoding.ASCII; }}

        public Asn1IA5String(ReadOnlyMappingStream source, long forceoffset)
            : base(source, forceoffset)
            {
            }

        internal Asn1IA5String(ReadOnlyMappingStream source)
            : base(source, 0)
            {
            }
        }
    }
