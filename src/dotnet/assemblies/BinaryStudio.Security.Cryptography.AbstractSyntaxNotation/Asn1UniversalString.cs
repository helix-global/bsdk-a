using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="UNIVERSALSTRING"/> type.
    /// </summary>
    internal sealed class Asn1UniversalString : Asn1String
        {
        public override Asn1ObjectType Type { get { return Asn1ObjectType.UniversalString; }}
        public override Encoding Encoding { get { return Encoding.ASCII; }}

        internal Asn1UniversalString(ReadOnlyMappingStream source, long forceoffset)
            : base(source, forceoffset)
            {
            }
        }
    }
