using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="GENERALSTRING"/> type.
    /// </summary>
    internal sealed class Asn1GeneralString : Asn1String
        {
        /// <summary>
        /// ASN.1 universal type. Always returns <see cref="Asn1ObjectType.GeneralString"/>.
        /// </summary>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.GeneralString; }}
        public override Encoding Encoding { get { return Encoding.ASCII; }}

        internal Asn1GeneralString(ReadOnlyMappingStream source, long forceoffset)
            : base(source, forceoffset)
            {
            }
        }
    }
