using System;
using System.Runtime.InteropServices;

// ReSharper disable once LocalVariableHidesMember

namespace BinaryStudio.Numeric
    {
    [StructLayout(LayoutKind.Explicit,Pack = 1)]
    public struct UInt192 : IComparable<UInt192>, IComparable, IEquatable<UInt192>
        {
        public static readonly UInt192 Zero     = new UInt192(new []{ UInt64.MinValue, UInt64.MinValue, UInt64.MinValue }, NumericSourceFlags.BigEndian);
        public static readonly UInt192 MinValue = new UInt192(new []{ UInt64.MinValue, UInt64.MinValue, UInt64.MinValue }, NumericSourceFlags.BigEndian);
        public static readonly UInt192 MaxValue = new UInt192(new []{ UInt64.MaxValue, UInt64.MaxValue, UInt64.MaxValue }, NumericSourceFlags.BigEndian);

        [FieldOffset( 0)] internal unsafe fixed UInt32 value[6];
        [FieldOffset( 0)] internal UInt64 a;
        [FieldOffset( 8)] internal UInt64 b;
        [FieldOffset(16)] internal UInt64 c;

        /// <summary>
        /// Constructs <see cref="UInt192"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flags"></param>
        private unsafe UInt192(UInt32[] source, NumericSourceFlags flags)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 6) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = 0;
            b = 0;
            c = 0;
            if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                value[0] = source[5];
                value[1] = source[4];
                value[2] = source[3];
                value[3] = source[2];
                value[4] = source[1];
                value[5] = source[0];
                }
            else
                {
                value[0] = source[0];
                value[1] = source[1];
                value[2] = source[2];
                value[3] = source[3];
                value[4] = source[4];
                value[5] = source[5];
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt192"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        private unsafe UInt192(UInt32[] source, Int32 firstindex, Int32 size, NumericSourceFlags flags)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (firstindex < 0) { throw new ArgumentOutOfRangeException(nameof(firstindex)); }
            if (size != 6) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt64.MinValue;
            b = UInt64.MinValue;
            c = UInt64.MinValue;
            if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                value[0] = source[firstindex + 5];
                value[1] = source[firstindex + 4];
                value[2] = source[firstindex + 3];
                value[3] = source[firstindex + 2];
                value[4] = source[firstindex + 1];
                value[5] = source[firstindex + 0];
                }
            else
                {
                value[0] = source[firstindex + 0];
                value[1] = source[firstindex + 1];
                value[2] = source[firstindex + 2];
                value[3] = source[firstindex + 3];
                value[4] = source[firstindex + 4];
                value[5] = source[firstindex + 5];
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt192"/> structure from <see cref="UInt64"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flags"></param>
        private unsafe UInt192(UInt64[] source, NumericSourceFlags flags) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 3) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = 0;
            b = 0;
            c = 0;
            fixed (void* target = value) {
                if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                    ((UInt64*)target)[0] = source[2];
                    ((UInt64*)target)[1] = source[1];
                    ((UInt64*)target)[2] = source[0];
                    }
                else
                    {
                    ((UInt64*)target)[0] = source[0];
                    ((UInt64*)target)[1] = source[1];
                    ((UInt64*)target)[2] = source[2];
                    }
                }
            }

        public Int32 CompareTo(UInt192 other)
            {
            return CompareTo(ref other);
            }

        public Int32 CompareTo(ref UInt64 other)
            {
            Int32 r;
            if ((r = c.CompareTo(UInt64.MinValue)) != 0) { return r; }
            if ((r = b.CompareTo(UInt64.MinValue)) != 0) { return r; }
            return a.CompareTo(other);
            }

        #region M:CompareTo(Int64):Int32
        public Int32 CompareTo(Int64 other)
            {
            if (other < 0) { return +1; }
            return CompareTo((UInt64)other);
            }
        #endregion

        public unsafe Int32 CompareTo(ref UInt192 other)
            {
            fixed (void* x = value)
            fixed (void* y = other.value) {
                Int32 r;
                if ((r = (((UInt64*)x)[2]).CompareTo(((UInt64*)y)[2])) != 0) return r;
                if ((r = (((UInt64*)x)[1]).CompareTo(((UInt64*)y)[1])) != 0) return r;
                return (((UInt64*)x)[0]).CompareTo(((UInt64*)y)[0]);
                }
            }

        public Int32 CompareTo(Object other) {
            if (other == null) { return +1; }
            if (other is UInt192 value) {
                return CompareTo(ref value);
                }
            throw new ArgumentOutOfRangeException(nameof(other));
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(UInt192 other)
            {
            return Equals(ref other);
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public unsafe Boolean Equals(ref UInt192 other)
            {
            fixed (void* x = value)
            fixed (void* y = other.value) {
                return ((((UInt64*)x)[2]) == (((UInt64*)y)[2]))
                    && ((((UInt64*)x)[1]) == (((UInt64*)y)[1]))
                    && ((((UInt64*)x)[0]) == (((UInt64*)y)[0]));
                }
            }

        public static Boolean operator ==(UInt192 x, UInt192 y)
            {
            return x.Equals(ref y);
            }

        public static Boolean operator !=(UInt192 x, UInt192 y)
            {
            return !x.Equals(ref y);
            }

        /// <summary>Performs a bitwise <see langword="or"/> operation on two <see cref="UInt192"/> values.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The result of the bitwise <see langword="or"/> operation.</returns>
        public static UInt192 operator |(UInt192 x, UInt192 y)
            {
            return new UInt192{
                a = x.a | y.a,
                b = x.b | y.b,
                c = x.c | y.c,
                };
            }

        /// <summary>Performs a bitwise <see langword="and"/> operation on two <see cref="UInt192"/> values.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The result of the bitwise <see langword="and"/> operation.</returns>
        public static UInt192 operator &(UInt192 x, UInt192 y)
            {
            return new UInt192{
                a = x.a & y.a,
                b = x.b & y.b,
                c = x.c & y.c,
                };
            }
        /// <summary>Performs a bitwise exclusive <see langword="or"/> (<see langword="xor"/>) operation on two <see cref="UInt192"/> values.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The result of the bitwise <see langword="or"/> operation.</returns>
        public static UInt192 operator ^(UInt192 x, UInt192 y)
            {
            return new UInt192{
                a = x.a ^ y.a,
                b = x.b ^ y.b,
                c = x.c ^ y.c,
                };
            }

        public static explicit operator UInt192(Byte    source) { return new UInt192(new []{ UInt64.MinValue, UInt64.MinValue, source }, NumericSourceFlags.BigEndian); }
        public static explicit operator UInt192(UInt16  source) { return new UInt192(new []{ UInt64.MinValue, UInt64.MinValue, source }, NumericSourceFlags.BigEndian); }
        public static explicit operator UInt192(UInt32  source) { return new UInt192(new []{ UInt64.MinValue, UInt64.MinValue, source }, NumericSourceFlags.BigEndian); }
        public static explicit operator UInt192(UInt64  source) { return new UInt192(new []{ UInt64.MinValue, UInt64.MinValue, source }, NumericSourceFlags.BigEndian); }
        public static explicit operator UInt192(UInt128 source) { return new UInt192(new []{ UInt64.MinValue, source.b, source.a }, NumericSourceFlags.BigEndian); }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override Int32 GetHashCode()
            {
            return NumericHelper.GetHashCode(
                NumericHelper.GetHashCode(a), NumericHelper.GetHashCode(
                NumericHelper.GetHashCode(b),
                NumericHelper.GetHashCode(c)));
            }
        }
    }