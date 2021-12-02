using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public class Asn1PrivateObject : Asn1Object, IAsn1Object
        {
        /// <inheritdoc/>
        public override Asn1ObjectClass Class { get { return Asn1ObjectClass.Private; }}
        public SByte Type { get; }

        internal Asn1PrivateObject(ReadOnlyMappingStream source, Int64 forceoffset, SByte type)
            : base(source, forceoffset)
            {
            Type = type;
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public override Boolean Equals(Asn1Object other)
            {
            return base.Equals(other) && (((Asn1PrivateObject)other).Type == Type);
            }
        }
    }
