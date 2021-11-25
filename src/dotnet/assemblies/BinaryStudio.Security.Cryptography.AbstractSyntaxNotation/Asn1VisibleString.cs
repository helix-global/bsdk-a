using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    internal class Asn1VisibleString : Asn1String
        {
        public override Asn1ObjectType Type { get { return Asn1ObjectType.VisibleString; }}
        public override Encoding Encoding { get { return Encoding.ASCII; }}

        public Asn1VisibleString(ReadOnlyMappingStream source, long forceoffset)
            : base(source, forceoffset)
            {
            }
        }
    }
