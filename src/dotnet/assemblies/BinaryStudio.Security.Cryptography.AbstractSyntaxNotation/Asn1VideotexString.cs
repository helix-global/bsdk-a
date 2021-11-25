using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    internal class Asn1VideotexString : Asn1String
        {
        public override Asn1ObjectType Type { get { return Asn1ObjectType.VideotexString; }}
        public override Encoding Encoding { get { return Encoding.ASCII; }}

        public Asn1VideotexString(ReadOnlyMappingStream source, long forceoffset)
            : base(source, forceoffset)
            {
            }
        }
    }
