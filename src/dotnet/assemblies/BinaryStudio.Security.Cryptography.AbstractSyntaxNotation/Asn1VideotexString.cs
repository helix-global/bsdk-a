using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="VIDEOTEXSTRING"/> type.
    /// </summary>
    internal sealed class Asn1VideotexString : Asn1String
        {
        /// <inheritdoc/>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.VideotexString; }}
        public override Encoding Encoding { get { return Encoding.ASCII; }}

        internal Asn1VideotexString(ReadOnlyMappingStream source, long forceoffset)
            : base(source, forceoffset)
            {
            }
        }
    }
