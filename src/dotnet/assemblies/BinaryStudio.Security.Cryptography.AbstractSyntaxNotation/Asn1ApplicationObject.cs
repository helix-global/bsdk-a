using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public class Asn1ApplicationObject : Asn1Object, IAsn1Object
        {
        /// <summary>
        /// ASN.1 object class. Always returns <see cref="Asn1ObjectClass.Application"/>.
        /// </summary>
        public override Asn1ObjectClass Class { get { return Asn1ObjectClass.Application; }}
        public SByte Type { get; }

        internal Asn1ApplicationObject(ReadOnlyMappingStream source, Int64 forceoffset, SByte type)
            : base(source, forceoffset)
            {
            Type = type;
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public override Boolean Equals(Asn1Object other)
            {
            return base.Equals(other) && (((Asn1ApplicationObject)other).Type == Type);
            }
        }
    }
