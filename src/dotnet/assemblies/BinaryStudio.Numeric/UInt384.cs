using System;
using System.Runtime.InteropServices;

// ReSharper disable once LocalVariableHidesMember

namespace BinaryStudio.Numeric
    {
    [StructLayout(LayoutKind.Explicit,Pack = 1)]
    public struct UInt384 : IComparable<UInt384>,IComparable,IEquatable<UInt384>
        {
        public static readonly UInt384 Zero     = new UInt384(UInt192.MinValue, UInt192.MinValue);
        public static readonly UInt384 MinValue = new UInt384(UInt192.MinValue, UInt192.MinValue);
        public static readonly UInt384 MaxValue = new UInt384(UInt192.MaxValue, UInt192.MaxValue);
        
        [FieldOffset( 0)] internal unsafe fixed UInt32 value[12];
        [FieldOffset( 0)] internal UInt192 a;
        [FieldOffset(48)] internal UInt192 b;

        /// <summary>
        /// Constructs <see cref="UInt384"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flags"></param>
        private unsafe UInt384(UInt32[] source, NumericSourceFlags flags)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 12) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt192.MinValue;
            b = UInt192.MinValue;
            if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                value[ 0] = source[11];
                value[ 1] = source[10];
                value[ 2] = source[ 9];
                value[ 3] = source[ 8];
                value[ 4] = source[ 7];
                value[ 5] = source[ 6];
                value[ 6] = source[ 5];
                value[ 7] = source[ 4];
                value[ 8] = source[ 3];
                value[ 9] = source[ 2];
                value[10] = source[ 1];
                value[11] = source[ 0];
                }
            else
                {
                value[ 0] = source[ 0];
                value[ 1] = source[ 1];
                value[ 2] = source[ 2];
                value[ 3] = source[ 3];
                value[ 4] = source[ 4];
                value[ 5] = source[ 5];
                value[ 6] = source[ 6];
                value[ 7] = source[ 7];
                value[ 8] = source[ 8];
                value[ 9] = source[ 9];
                value[10] = source[10];
                value[11] = source[11];
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt384"/> structure from <see cref="UInt64"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flags"></param>
        private unsafe UInt384(UInt64[] source, NumericSourceFlags flags) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 6) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt192.MinValue;
            b = UInt192.MinValue;
            fixed (void* target = value) {
                if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                    ((UInt64*)target)[0] = source[5];
                    ((UInt64*)target)[1] = source[4];
                    ((UInt64*)target)[2] = source[3];
                    ((UInt64*)target)[3] = source[2];
                    ((UInt64*)target)[4] = source[1];
                    ((UInt64*)target)[5] = source[0];
                    }
                else
                    {
                    ((UInt64*)target)[0] = source[0];
                    ((UInt64*)target)[1] = source[1];
                    ((UInt64*)target)[2] = source[2];
                    ((UInt64*)target)[3] = source[3];
                    ((UInt64*)target)[4] = source[4];
                    ((UInt64*)target)[5] = source[5];
                    }
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt384"/> structure from <see cref="UInt128"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flags"></param>
        private unsafe UInt384(UInt128[] source, NumericSourceFlags flags) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 3) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt192.MinValue;
            b = UInt192.MinValue;
            fixed (void* target = value) {
                if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                    ((UInt128*)target)[0] = source[2];
                    ((UInt128*)target)[1] = source[1];
                    ((UInt128*)target)[2] = source[0];
                    }
                else
                    {
                    ((UInt128*)target)[0] = source[0];
                    ((UInt128*)target)[1] = source[1];
                    ((UInt128*)target)[2] = source[2];
                    }
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt384"/> structure from <see cref="UInt192"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flags"></param>
        private unsafe UInt384(UInt192[] source, NumericSourceFlags flags) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 2) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt192.MinValue;
            b = UInt192.MinValue;
            fixed (void* target = value) {
                if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                    ((UInt192*)target)[0] = source[1];
                    ((UInt192*)target)[1] = source[0];
                    }
                else
                    {
                    ((UInt192*)target)[0] = source[0];
                    ((UInt192*)target)[1] = source[1];
                    }
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt384"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        private unsafe UInt384(UInt32[] source, Int32 firstindex, Int32 size, NumericSourceFlags flags)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (firstindex < 0) { throw new ArgumentOutOfRangeException(nameof(firstindex)); }
            if (size != 8) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt192.MinValue;
            b = UInt192.MinValue;
            if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                value[ 0] = source[firstindex + 11];
                value[ 1] = source[firstindex + 10];
                value[ 2] = source[firstindex +  9];
                value[ 3] = source[firstindex +  8];
                value[ 4] = source[firstindex +  7];
                value[ 5] = source[firstindex +  6];
                value[ 6] = source[firstindex +  5];
                value[ 7] = source[firstindex +  4];
                value[ 8] = source[firstindex +  3];
                value[ 9] = source[firstindex +  2];
                value[10] = source[firstindex +  1];
                value[11] = source[firstindex +  0];
                }
            else
                {
                value[ 0] = source[firstindex +  0];
                value[ 1] = source[firstindex +  1];
                value[ 2] = source[firstindex +  2];
                value[ 3] = source[firstindex +  3];
                value[ 4] = source[firstindex +  4];
                value[ 5] = source[firstindex +  5];
                value[ 6] = source[firstindex +  6];
                value[ 7] = source[firstindex +  7];
                value[ 8] = source[firstindex +  8];
                value[ 9] = source[firstindex +  9];
                value[10] = source[firstindex + 10];
                value[11] = source[firstindex + 11];
                }
            }

        private UInt384(ref UInt192 hi, ref UInt192 lo) {
            a = lo;
            b = hi;
            }

        private UInt384(UInt192 hi, UInt192 lo)
            :this(ref hi, ref lo)
            {
            }

        public unsafe Int32 CompareTo(ref UInt384 other) {
            fixed (void* x = value)
            fixed (void* y = other.value) {
                Int32 r;
                return ((r = 
                    (((UInt192*)x)[1]).CompareTo(ref ((UInt192*)y)[1])) == 0) ?
                    (((UInt192*)x)[0]).CompareTo(ref ((UInt192*)y)[0]) : r;
                }
            }

        public Int32 CompareTo(UInt384 other)
            {
            return CompareTo(ref other);
            }

        public Int32 CompareTo(Object other) {
            if (other == null) { return +1; }
            if (other is UInt384 value) {
                return CompareTo(ref value);
                }
            throw new ArgumentOutOfRangeException(nameof(other));
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(UInt384 other)
            {
            return Equals(ref other);
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public unsafe Boolean Equals(ref UInt384 other)
            {
            fixed (void* x = value)
            fixed (void* y = other.value) {
                return ((((UInt192*)x)[1]).Equals(ref (((UInt192*)y)[1])))
                    && ((((UInt192*)x)[0]).Equals(ref (((UInt192*)y)[0])));
                }
            }

        public static Boolean operator ==(UInt384 x, UInt384 y)
            {
            return x.Equals(ref y);
            }

        public static Boolean operator !=(UInt384 x, UInt384 y)
            {
            return !x.Equals(ref y);
            }

        /// <summary>Performs a bitwise <see langword="or"/> operation on two <see cref="UInt384"/> values.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The result of the bitwise <see langword="or"/> operation.</returns>
        public static UInt384 operator |(UInt384 x, UInt384 y)
            {
            return new UInt384{
                a = x.a | y.a,
                b = x.b | y.b
                };
            }

        /// <summary>Performs a bitwise <see langword="and"/> operation on two <see cref="UInt384"/> values.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The result of the bitwise <see langword="and"/> operation.</returns>
        public static UInt384 operator &(UInt384 x, UInt384 y)
            {
            return new UInt384{
                a = x.a & y.a,
                b = x.b & y.b
                };
            }

        /// <summary>Performs a bitwise exclusive <see langword="or"/> (<see langword="xor"/>) operation on two <see cref="UInt384"/> values.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The result of the bitwise <see langword="or"/> operation.</returns>
        public static UInt384 operator ^(UInt384 x, UInt384 y)
            {
            return new UInt384{
                a = x.a ^ y.a,
                b = x.b ^ y.b
                };
            }

        public static explicit operator UInt384(Byte   source) { return new UInt384(UInt192.MinValue, (UInt192)source); }
        public static explicit operator UInt384(UInt16 source) { return new UInt384(UInt192.MinValue, (UInt192)source); }
        public static explicit operator UInt384(UInt32 source) { return new UInt384(UInt192.MinValue, (UInt192)source); }
        public static explicit operator UInt384(UInt64 source) { return new UInt384(UInt192.MinValue, (UInt192)source); }
        public static explicit operator UInt384(UInt128 source) {
            return new UInt384(new []{
                UInt128.MinValue,
                UInt128.MinValue,
                source,
                }, NumericSourceFlags.BigEndian);
            }
        public static explicit operator UInt384(UInt192 source) {
            return new UInt384(new []{
                UInt192.MinValue,
                source,
                }, NumericSourceFlags.BigEndian);
            }
        public static unsafe explicit operator UInt384(UInt224 source) {
            return new UInt384(new []{
                0U,
                0U,
                0U,
                0U,
                0U,
                source.value[6],
                source.value[5],
                source.value[4],
                source.value[3],
                source.value[2],
                source.value[1],
                source.value[0]
                }, NumericSourceFlags.BigEndian);
            }
        public static explicit operator UInt384(UInt256 source) {
            return new UInt384(new []{
                UInt128.MinValue,
                source.b,
                source.a
                }, NumericSourceFlags.BigEndian);
            }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override Int32 GetHashCode()
            {
            return NumericHelper.GetHashCode(
                a.GetHashCode(),
                b.GetHashCode());
            }
        }
    }