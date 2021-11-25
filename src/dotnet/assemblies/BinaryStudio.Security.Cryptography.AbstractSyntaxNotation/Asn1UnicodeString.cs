using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    internal class Asn1UnicodeString : Asn1String
        {
        public override Asn1ObjectType Type { get { return Asn1ObjectType.UnicodeString; }}
        public override Encoding Encoding { get { return new UnicodeEncoding(true, false); }}

        public Asn1UnicodeString(ReadOnlyMappingStream source, long forceoffset)
            : base(source, forceoffset)
            {
            }
        }
    }
