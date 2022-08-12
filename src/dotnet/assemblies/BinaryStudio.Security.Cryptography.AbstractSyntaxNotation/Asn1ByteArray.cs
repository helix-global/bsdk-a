using System;
using System.Linq;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public class Asn1ByteArray : Asn1LinkObject, IEquatable<Asn1ByteArray>
        {
        public Byte[] Value { get; }
        internal Asn1ByteArray(Asn1Object source)
            :base(source)
            {
            Value = source.Content.ToArray();
            }

        internal Asn1ByteArray(Asn1Integer source)
            :this((Asn1Object)source)
            {
            #if NET35
            Value = source.Value;
            #else
            Value = source.Value.ToByteArray().Reverse().ToArray();
            #endif
            }

        internal Asn1ByteArray(Asn1Object source, Byte[] value)
            :this(source)
            {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            Value = value;
            }

        public override String ToString()
            {
            #if NET35
            return String.Join(String.Empty, Value.Select(i => i.ToString("X2")).ToArray());
            #else
            return String.Join(String.Empty, Value.Select(i => i.ToString("X2")));
            #endif
            }

        public Boolean Equals(Asn1ByteArray other)
            {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true;  }
            return Equals(Value, other.Value);
            }

        private static Boolean Equals(Byte[] x, Byte[] y) {
            if (ReferenceEquals(x, y)) { return true; }
            if ((x == null) || (y == null)) { return false; }
            if (x.Length == y.Length) {
                var c = x.Length;
                for (var i = 0; i < c; ++i)
                    {
                    if (x[i] != y[i]) { return false; }
                    }
                return true;
                }
            return false;
            }

        #region M:Equals(Asn1ByteArray,Asn1ByteArray):Boolean
        public static Boolean Equals(Asn1ByteArray x, Asn1ByteArray y) {
            if (ReferenceEquals(x, y))    { return true;  }
            if (ReferenceEquals(x, null)) { return false; }
            return x.Equals(y);
            }
        #endregion

        public override bool Equals(object obj)
            {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Asn1ByteArray) obj);
            }

        public override int GetHashCode()
            {
            unchecked
                {
                return (base.GetHashCode() * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                }
            }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Asn1ByteArray" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(Asn1ByteArray left, Asn1ByteArray right)
            {
            return Equals(left, right);
            }

        /// <summary>Returns a value that indicates whether two <see cref="T:BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Asn1ByteArray" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(Asn1ByteArray left, Asn1ByteArray right)
            {
            return !Equals(left, right);
            }
        }
    }
