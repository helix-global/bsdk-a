using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="IA5STRING"/> type.
    /// </summary>
    internal sealed class Asn1IA5String : Asn1String
        {
        /// <summary>
        /// ASN.1 universal type. Always returns <see cref="Asn1ObjectType.IA5String"/>.
        /// </summary>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.IA5String; }}
        public override Encoding Encoding { get { return Encoding.ASCII; }}

        internal Asn1IA5String(ReadOnlyMappingStream source, long forceoffset)
            : base(source, forceoffset)
            {
            }

        internal Asn1IA5String(ReadOnlyMappingStream source)
            : base(source, 0)
            {
            }
        }
    }
