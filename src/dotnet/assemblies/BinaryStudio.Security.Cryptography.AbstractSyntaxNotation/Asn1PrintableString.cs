using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="PRINTABLESTRING"/> type.
    /// </summary>
    internal sealed class Asn1PrintableString : Asn1String
        {
        public override Asn1ObjectType Type { get { return Asn1ObjectType.PrintableString; }}
        public override Encoding Encoding { get { return Encoding.ASCII; }}

        public Asn1PrintableString(ReadOnlyMappingStream source, long forceoffset)
            : base(source, forceoffset)
            {
            }
        }
    }
