using System;

// ReSharper disable once LocalVariableHidesMember

namespace BinaryStudio.Numeric
    {
    public struct UInt256 : IComparable<UInt256>, IEquatable<UInt256>, IComparable
        {
        public static readonly UInt256 Zero     = new UInt256(UInt128.MinValue, UInt128.MinValue);
        public static readonly UInt256 MinValue = new UInt256(UInt128.MinValue, UInt128.MinValue);
        public static readonly UInt256 MaxValue = new UInt256(UInt128.MaxValue, UInt128.MaxValue);

        private unsafe fixed UInt32 value[8];

        /// <summary>
        /// Constructs <see cref="UInt256"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt256(UInt32[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 8) { throw new ArgumentOutOfRangeException(nameof(source)); }
            value[0] = source[7];
            value[1] = source[6];
            value[2] = source[5];
            value[3] = source[4];
            value[4] = source[3];
            value[5] = source[2];
            value[6] = source[1];
            value[7] = source[0];
            }

        /// <summary>
        /// Constructs <see cref="UInt256"/> structure from <see cref="UInt64"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt256(UInt64[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 4) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt64*)target)[0] = source[3];
                ((UInt64*)target)[1] = source[2];
                ((UInt64*)target)[2] = source[1];
                ((UInt64*)target)[3] = source[0];
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt256"/> structure from <see cref="UInt128"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt256(UInt128[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 2) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt128*)target)[0] = source[1];
                ((UInt128*)target)[1] = source[0];
                }
            }

        public unsafe UInt256(ref UInt128 hi, ref UInt128 lo) {
            fixed (void* target = value) {
                ((UInt128*)target)[0] = lo;
                ((UInt128*)target)[1] = hi;
                }
            }

        private UInt256(UInt128 hi, UInt128 lo)
            :this(ref hi, ref lo)
            {
            }

        public Int32 CompareTo(UInt256 other)
            {
            return CompareTo(ref other);
            }

        public unsafe Int32 CompareTo(ref UInt256 other) {
            fixed (void* x = value)
            fixed (void* y = other.value) {
                Int32 r;
                return ((r = 
                    (((UInt128*)x)[1]).CompareTo(ref ((UInt128*)y)[1])) == 0) ?
                    (((UInt128*)x)[0]).CompareTo(ref ((UInt128*)y)[0]) : r;
                }
            }

        public Int32 CompareTo(Object other) {
            if (other == null) { return +1; }
            if (other is UInt256 value) {
                return CompareTo(ref value);
                }
            throw new ArgumentOutOfRangeException(nameof(other));
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(UInt256 other)
            {
            return Equals(ref other);
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public unsafe Boolean Equals(ref UInt256 other)
            {
            fixed (void* x = value)
            fixed (void* y = other.value) {
                return ((((UInt128*)x)[1]).Equals(ref (((UInt128*)y)[1])))
                    && ((((UInt128*)x)[0]).Equals(ref (((UInt128*)y)[0])));
                }
            }

        public static Boolean operator ==(UInt256 x, UInt256 y)
            {
            return x.Equals(ref y);
            }

        public static Boolean operator !=(UInt256 x, UInt256 y)
            {
            return !x.Equals(ref y);
            }
        }
    }