using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="GRAPHICSTRING"/> type.
    /// </summary>
    internal sealed class Asn1GraphicString : Asn1String
        {
        /// <summary>
        /// ASN.1 universal type. Always returns <see cref="Asn1ObjectType.GraphicString"/>.
        /// </summary>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.GraphicString; }}
        public override Encoding Encoding { get { return Encoding.ASCII; }}

        internal Asn1GraphicString(ReadOnlyMappingStream source, long forceoffset)
            : base(source, forceoffset)
            {
            }
        }
    }
