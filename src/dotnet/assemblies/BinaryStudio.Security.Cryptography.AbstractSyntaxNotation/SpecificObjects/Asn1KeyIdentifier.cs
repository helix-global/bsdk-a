using System;
using System.Linq;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public sealed class Asn1KeyIdentifier
        {
        public Byte[] Value { get; }
        public Asn1KeyIdentifier(Byte[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Value = source;
            }

        public Asn1KeyIdentifier(ReadOnlyMappingStream source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Value = source.ToArray();
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return String.Join(String.Empty, Value.Select(i => i.ToString("X2")));
            }
        }
    }