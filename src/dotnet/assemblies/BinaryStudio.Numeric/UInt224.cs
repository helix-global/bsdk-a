using System;
using System.Runtime.InteropServices;

// ReSharper disable once LocalVariableHidesMember

namespace BinaryStudio.Numeric
    {
    [StructLayout(LayoutKind.Explicit,Pack = 1)]
    public struct UInt224 : IComparable<UInt224>, IComparable, IEquatable<UInt224>
        {
        public static readonly UInt224 MinValue = new UInt224(new []{ UInt32.MinValue, UInt32.MinValue, UInt32.MinValue, UInt32.MinValue, UInt32.MinValue, UInt32.MinValue, UInt32.MinValue }, NumericSourceFlags.BigEndian);
        public static readonly UInt224 MaxValue = new UInt224(new []{ UInt32.MaxValue, UInt32.MaxValue, UInt32.MaxValue, UInt32.MaxValue, UInt32.MaxValue, UInt32.MaxValue, UInt32.MaxValue }, NumericSourceFlags.BigEndian);
        public static readonly UInt224 Zero = MinValue;

        [FieldOffset( 0)] internal unsafe fixed UInt32 value[7];
        [FieldOffset( 0)] internal UInt128 a;
        [FieldOffset(16)] internal UInt64 b;
        [FieldOffset(24)] internal UInt32 c;

        /// <summary>
        /// Constructs <see cref="UInt224"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flags"></param>
        private unsafe UInt224(UInt32[] source, NumericSourceFlags flags)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 7) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt128.MinValue;
            b = UInt64.MinValue;
            c = UInt32.MinValue;
            if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                value[0] = source[6];
                value[1] = source[5];
                value[2] = source[4];
                value[3] = source[3];
                value[4] = source[2];
                value[5] = source[1];
                value[6] = source[0];
                }
            else
                {
                value[0] = source[0];
                value[1] = source[1];
                value[2] = source[2];
                value[3] = source[3];
                value[4] = source[4];
                value[5] = source[5];
                value[6] = source[6];
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt224"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        private unsafe UInt224(UInt32[] source, Int32 firstindex, Int32 size, NumericSourceFlags flags)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (firstindex < 0) { throw new ArgumentOutOfRangeException(nameof(firstindex)); }
            if (size != 7) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt128.MinValue;
            b = UInt64.MinValue;
            c = UInt32.MinValue;
            if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                value[0] = source[firstindex + 7];
                value[1] = source[firstindex + 6];
                value[2] = source[firstindex + 5];
                value[3] = source[firstindex + 4];
                value[4] = source[firstindex + 3];
                value[5] = source[firstindex + 2];
                value[6] = source[firstindex + 1];
                value[7] = source[firstindex + 0];
                }
            else
                {
                value[0] = source[firstindex + 0];
                value[1] = source[firstindex + 1];
                value[2] = source[firstindex + 2];
                value[3] = source[firstindex + 3];
                value[4] = source[firstindex + 4];
                value[5] = source[firstindex + 5];
                value[6] = source[firstindex + 6];
                value[7] = source[firstindex + 7];
                }
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

        /// <summary>Performs a bitwise <see langword="or"/> operation on two <see cref="UInt224"/> values.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The result of the bitwise <see langword="or"/> operation.</returns>
        public static UInt224 operator |(UInt224 x, UInt224 y)
            {
            return new UInt224{
                a = x.a | y.a,
                b = x.b | y.b,
                c = x.c | y.c
                };
            }

        /// <summary>Performs a bitwise <see langword="and"/> operation on two <see cref="UInt224"/> values.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The result of the bitwise <see langword="and"/> operation.</returns>
        public static UInt224 operator &(UInt224 x, UInt224 y)
            {
            return new UInt224{
                a = x.a & y.a,
                b = x.b & y.b,
                c = x.c & y.c
                };
            }
        /// <summary>Performs a bitwise exclusive <see langword="or"/> (<see langword="xor"/>) operation on two <see cref="UInt224"/> values.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The result of the bitwise <see langword="or"/> operation.</returns>
        public static UInt224 operator ^(UInt224 x, UInt224 y)
            {
            return new UInt224{
                a = x.a ^ y.a,
                b = x.b ^ y.b,
                c = x.c ^ y.c
                };
            }

        public static explicit operator UInt224(Byte   source) { return new UInt224(new UInt32[]{ 0,0,0,0,0,0,source }, NumericSourceFlags.BigEndian); }
        public static explicit operator UInt224(UInt16 source) { return new UInt224(new UInt32[]{ 0,0,0,0,0,0,source }, NumericSourceFlags.BigEndian); }
        public static explicit operator UInt224(UInt32 source) { return new UInt224(new UInt32[]{ 0,0,0,0,0,0,source }, NumericSourceFlags.BigEndian); }
        public static explicit operator UInt224(UInt64 source) {
            return new UInt224(new []{ 0U,0U,0U,0U,0U,
                (UInt32)((source >> 32) & 0xffffffff),
                (UInt32)((source)       & 0xffffffff)
                }, NumericSourceFlags.BigEndian);
            }
        public static unsafe explicit operator UInt224(UInt128 source) {
            return new UInt224(new []{
                UInt32.MinValue,
                UInt32.MinValue,
                UInt32.MinValue,
                source.value[3],
                source.value[2],
                source.value[1],
                source.value[0]
                }, NumericSourceFlags.BigEndian); }
        public static unsafe explicit operator UInt224(UInt192 source) {
            return new UInt224(new []{
                UInt32.MinValue,
                source.value[3],
                source.value[4],
                source.value[3],
                source.value[2],
                source.value[1],
                source.value[0]
                }, NumericSourceFlags.BigEndian);
            }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override Int32 GetHashCode()
            {
            return NumericHelper.GetHashCode(
                a.GetHashCode(), NumericHelper.GetHashCode(
                NumericHelper.GetHashCode(b),
                NumericHelper.GetHashCode(c)));
            }
        }
    }