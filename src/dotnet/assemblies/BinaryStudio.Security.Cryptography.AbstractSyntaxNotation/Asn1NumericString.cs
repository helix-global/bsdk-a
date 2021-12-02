using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="NUMERICSTRING"/> type.
    /// </summary>
    internal sealed class Asn1NumericString : Asn1String
        {
        /// <summary>
        /// ASN.1 universal type. Always returns <see cref="Asn1ObjectType.NumericString"/>.
        /// </summary>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.NumericString; }}
        public override Encoding Encoding { get { return Encoding.ASCII; }}

        internal Asn1NumericString(ReadOnlyMappingStream source, long forceoffset)
            : base(source, forceoffset)
            {
            }
        }
    }
