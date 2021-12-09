using System;

// ReSharper disable once LocalVariableHidesMember

namespace BinaryStudio.Numeric
    {
    public struct UInt128 : IComparable<UInt128>, IComparable, IEquatable<UInt128>
        {
        private unsafe fixed UInt32 value[4];

        /// <summary>
        /// Constructs <see cref="UInt128"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt128(UInt32[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 4) { throw new ArgumentOutOfRangeException(nameof(source)); }
            value[0] = source[3];
            value[1] = source[2];
            value[2] = source[1];
            value[3] = source[0];
            }

        /// <summary>
        /// Constructs <see cref="UInt128"/> structure from <see cref="UInt64"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt128(UInt64[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 2) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt64*)target)[0] = source[1];
                ((UInt64*)target)[1] = source[0];
                }
            }

        public unsafe UInt128(ref UInt64 hi, ref UInt64 lo) {
            fixed (void* target = value) {
                ((UInt64*)target)[0] = lo;
                ((UInt64*)target)[1] = hi;
                }
            }

        public unsafe UInt128(UInt64 hi, UInt64 lo) {
            fixed (void* target = value) {
                ((UInt64*)target)[0] = lo;
                ((UInt64*)target)[1] = hi;
                }
            }


        public Int32 CompareTo(UInt128 other)
            {
            return CompareTo(ref other);
            }

        public unsafe Int32 CompareTo(ref UInt128 other)
            {
            fixed (void* x = value)
            fixed (void* y = other.value) {
                Int32 r;
                return ((r = 
                    (((UInt64*)x)[1]).CompareTo(((UInt64*)y)[1])) == 0) ?
                    (((UInt64*)x)[0]).CompareTo(((UInt64*)y)[0]) : r;
                }
            }

        public Int32 CompareTo(Object other) {
            if (other == null) { return +1; }
            if (other is UInt128 value) {
                return CompareTo(ref value);
                }
            throw new ArgumentOutOfRangeException(nameof(other));
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(UInt128 other)
            {
            return Equals(ref other);
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public unsafe Boolean Equals(ref UInt128 other)
            {
            fixed (void* x = value)
            fixed (void* y = other.value) {
                return ((((UInt64*)x)[1]) == (((UInt64*)y)[1]))
                    && ((((UInt64*)x)[0]) == (((UInt64*)y)[0]));
                }
            }
        }
    }