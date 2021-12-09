using System;

// ReSharper disable once LocalVariableHidesMember

namespace BinaryStudio.Numeric
    {
    public struct UInt224 : IComparable<UInt224>, IComparable, IEquatable<UInt224>
        {
        public static readonly UInt224 MinValue = new UInt224(new []{ UInt32.MinValue, UInt32.MinValue, UInt32.MinValue, UInt32.MinValue, UInt32.MinValue, UInt32.MinValue, UInt32.MinValue });
        public static readonly UInt224 MaxValue = new UInt224(new []{ UInt32.MaxValue, UInt32.MaxValue, UInt32.MaxValue, UInt32.MaxValue, UInt32.MaxValue, UInt32.MaxValue, UInt32.MaxValue });
        public static readonly UInt224 Zero = MinValue;

        private unsafe fixed UInt32 value[7];

        /// <summary>
        /// Constructs <see cref="UInt224"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt224(UInt32[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 7) { throw new ArgumentOutOfRangeException(nameof(source)); }
            value[0] = source[6];
            value[1] = source[5];
            value[2] = source[4];
            value[3] = source[3];
            value[4] = source[2];
            value[5] = source[1];
            value[6] = source[0];
            }

        public Int32 CompareTo(UInt224 other)
            {
            return CompareTo(ref other);
            }

        public unsafe Int32 CompareTo(ref UInt224 other)
            {
            fixed (void* x = value)
            fixed (void* y = other.value) {
                Int32 r;
                if ((r = (((UInt32*)x)[6]).CompareTo(((UInt32*)y)[6])) != 0) return r;
                if ((r = (((UInt32*)x)[5]).CompareTo(((UInt32*)y)[5])) != 0) return r;
                if ((r = (((UInt32*)x)[4]).CompareTo(((UInt32*)y)[4])) != 0) return r;
                if ((r = (((UInt32*)x)[3]).CompareTo(((UInt32*)y)[3])) != 0) return r;
                if ((r = (((UInt32*)x)[2]).CompareTo(((UInt32*)y)[2])) != 0) return r;
                if ((r = (((UInt32*)x)[1]).CompareTo(((UInt32*)y)[1])) != 0) return r;
                return (((UInt32*)x)[0]).CompareTo(((UInt32*)y)[0]);
                }
            }

        public Int32 CompareTo(Object other) {
            if (other == null) { return +1; }
            if (other is UInt224 value) {
                return CompareTo(ref value);
                }
            throw new ArgumentOutOfRangeException(nameof(other));
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(UInt224 other)
            {
            return Equals(ref other);
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public unsafe Boolean Equals(ref UInt224 other)
            {
            fixed (void* x = value)
            fixed (void* y = other.value) {
                return ((((UInt32*)x)[6]) == (((UInt32*)y)[6]))
                    && ((((UInt32*)x)[5]) == (((UInt32*)y)[5]))
                    && ((((UInt32*)x)[4]) == (((UInt32*)y)[4]))
                    && ((((UInt32*)x)[3]) == (((UInt32*)y)[3]))
                    && ((((UInt32*)x)[2]) == (((UInt32*)y)[2]))
                    && ((((UInt32*)x)[1]) == (((UInt32*)y)[1]))
                    && ((((UInt32*)x)[0]) == (((UInt32*)y)[0]));
                }
            }

        public static Boolean operator ==(UInt224 x, UInt224 y)
            {
            return x.Equals(ref y);
            }

        public static Boolean operator !=(UInt224 x, UInt224 y)
            {
            return !x.Equals(ref y);
            }
        }
    }