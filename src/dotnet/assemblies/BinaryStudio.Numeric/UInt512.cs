using System;
using System.Globalization;
using System.Runtime.InteropServices;

// ReSharper disable once LocalVariableHidesMember

namespace BinaryStudio.Numeric
    {
    [StructLayout(LayoutKind.Explicit,Pack = 1)]
    public struct UInt512 : IComparable<UInt512>,IComparable,IEquatable<UInt512>
        {
        public static readonly UInt512 Zero     = new UInt512(UInt256.MinValue, UInt256.MinValue);
        public static readonly UInt512 MinValue = new UInt512(UInt256.MinValue, UInt256.MinValue);
        public static readonly UInt512 MaxValue = new UInt512(UInt256.MaxValue, UInt256.MaxValue);

        [FieldOffset( 0)] private unsafe fixed UInt32 value[16];
        [FieldOffset( 0)] private UInt256 a;
        [FieldOffset(32)] private UInt256 b;

        /// <summary>
        /// Constructs <see cref="UInt512"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flags"></param>
        private unsafe UInt512(UInt32[] source, NumericSourceFlags flags)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 16) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt256.MinValue;
            b = UInt256.MinValue;
            if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                value[ 0] = source[15];
                value[ 1] = source[14];
                value[ 2] = source[13];
                value[ 3] = source[12];
                value[ 4] = source[11];
                value[ 5] = source[10];
                value[ 6] = source[ 9];
                value[ 7] = source[ 8];
                value[ 8] = source[ 7];
                value[ 9] = source[ 6];
                value[10] = source[ 5];
                value[11] = source[ 4];
                value[12] = source[ 3];
                value[13] = source[ 2];
                value[14] = source[ 1];
                value[15] = source[ 0];
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
                value[12] = source[12];
                value[13] = source[13];
                value[14] = source[14];
                value[15] = source[15];
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt512"/> structure from <see cref="UInt64"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flags"></param>
        private unsafe UInt512(UInt64[] source, NumericSourceFlags flags) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 6) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt256.MinValue;
            b = UInt256.MinValue;
            fixed (void* target = value) {
                if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                    ((UInt64*)target)[0] = source[7];
                    ((UInt64*)target)[1] = source[6];
                    ((UInt64*)target)[2] = source[5];
                    ((UInt64*)target)[3] = source[4];
                    ((UInt64*)target)[4] = source[3];
                    ((UInt64*)target)[5] = source[2];
                    ((UInt64*)target)[6] = source[1];
                    ((UInt64*)target)[7] = source[0];
                    }
                else
                    {
                    ((UInt64*)target)[0] = source[0];
                    ((UInt64*)target)[1] = source[1];
                    ((UInt64*)target)[2] = source[2];
                    ((UInt64*)target)[3] = source[3];
                    ((UInt64*)target)[4] = source[4];
                    ((UInt64*)target)[5] = source[5];
                    ((UInt64*)target)[6] = source[6];
                    ((UInt64*)target)[7] = source[7];
                    }
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt512"/> structure from <see cref="UInt128"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flags"></param>
        private unsafe UInt512(UInt128[] source, NumericSourceFlags flags) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 4) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt256.MinValue;
            b = UInt256.MinValue;
            fixed (void* target = value) {
                if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                    ((UInt128*)target)[0] = source[3];
                    ((UInt128*)target)[1] = source[2];
                    ((UInt128*)target)[2] = source[1];
                    ((UInt128*)target)[3] = source[0];
                    }
                else
                    {
                    ((UInt128*)target)[0] = source[0];
                    ((UInt128*)target)[1] = source[1];
                    ((UInt128*)target)[2] = source[2];
                    ((UInt128*)target)[3] = source[3];
                    }
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt512"/> structure from <see cref="UInt256"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flags"></param>
        private unsafe UInt512(UInt256[] source, NumericSourceFlags flags) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 2) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt256.MinValue;
            b = UInt256.MinValue;
            fixed (void* target = value) {
                if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                    ((UInt256*)target)[0] = source[1];
                    ((UInt256*)target)[1] = source[0];
                    }
                else
                    {
                    ((UInt256*)target)[0] = source[0];
                    ((UInt256*)target)[1] = source[1];
                    }
                }
            }

        public UInt512(ref UInt256 hi, ref UInt256 lo) {
            a = lo;
            b = hi;
            }

        private UInt512(UInt256 hi, UInt256 lo)
            :this(ref hi, ref lo)
            {
            }

        public unsafe Int32 CompareTo(ref UInt512 other) {
            fixed (void* x = value)
            fixed (void* y = other.value) {
                Int32 r;
                return ((r = 
                    (((UInt256*)x)[1]).CompareTo(ref ((UInt256*)y)[1])) == 0) ?
                    (((UInt256*)x)[0]).CompareTo(ref ((UInt256*)y)[0]) : r;
                }
            }

        public Int32 CompareTo(UInt512 other)
            {
            return CompareTo(ref other);
            }

        public Int32 CompareTo(Object other) {
            if (other == null) { return +1; }
            if (other is UInt512 value) {
                return CompareTo(ref value);
                }
            throw new ArgumentOutOfRangeException(nameof(other));
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(UInt512 other)
            {
            return Equals(ref other);
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public unsafe Boolean Equals(ref UInt512 other)
            {
            fixed (void* x = value)
            fixed (void* y = other.value) {
                return ((((UInt256*)x)[1]).Equals(ref (((UInt256*)y)[1])))
                    && ((((UInt256*)x)[0]).Equals(ref (((UInt256*)y)[0])));
                }
            }

        /// <summary>Performs a bitwise <see langword="or"/> operation on two <see cref="UInt512"/> values.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The result of the bitwise <see langword="or"/> operation.</returns>
        public static UInt512 operator |(UInt512 x, UInt512 y)
            {
            return new UInt512{
                a = x.a | y.a,
                b = x.b | y.b
                };
            }

        /// <summary>Performs a bitwise <see langword="and"/> operation on two <see cref="UInt512"/> values.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The result of the bitwise <see langword="and"/> operation.</returns>
        public static UInt512 operator &(UInt512 x, UInt512 y)
            {
            return new UInt512{
                a = x.a & y.a,
                b = x.b & y.b
                };
            }

        /// <summary>Performs a bitwise exclusive <see langword="or"/> (<see langword="xor"/>) operation on two <see cref="UInt512"/> values.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The result of the bitwise <see langword="or"/> operation.</returns>
        public static UInt512 operator ^(UInt512 x, UInt512 y)
            {
            return new UInt512{
                a = x.a ^ y.a,
                b = x.b ^ y.b
                };
            }

        /// <summary>Returns the remainder that results from division with two specified <see cref="UInt512"/> and <see cref="UInt32"/> values.</summary>
        /// <param name="x">The value to be divided.</param>
        /// <param name="y">The value to divide by.</param>
        /// <returns>The remainder that results from the division.</returns>
        /// <exception cref="T:System.DivideByZeroException"><paramref name="y"/> is 0 (zero).</exception>
        public static unsafe UInt32 operator %(UInt512 x, UInt32 y)
            {
            if (y == 0)   { throw new DivideByZeroException(); }
            if (x.b == 0) { return (UInt32)(x.a%y); }
            var r = 0L;
            for (var i = 15; i >= 0; i--) {
                var α = (Int64)x.value[i];
                var β = r << 32;
                var γ = (β | α);
                r = (γ %y);
                }
            return (UInt32)r;
            }

        /// <summary>Returns the remainder that results from division with two specified <see cref="UInt512"/> and <see cref="Int32"/> values.</summary>
        /// <param name="x">The value to be divided.</param>
        /// <param name="y">The value to divide by.</param>
        /// <returns>The remainder that results from the division.</returns>
        /// <exception cref="T:System.DivideByZeroException"><paramref name="y"/> is 0 (zero).</exception>
        public static Int32 operator %(UInt512 x, Int32 y) {
            if (y == 0) { throw new DivideByZeroException(); }
            return (y < 0)
                ? -(Int32)(x % (UInt32)(-y))
                : +(Int32)(x % (UInt32)(+y));
            }

        public static Boolean operator ==(UInt512 x, UInt512 y)
            {
            return x.Equals(ref y);
            }

        public static Boolean operator !=(UInt512 x, UInt512 y)
            {
            return !x.Equals(ref y);
            }

        public String ToString(String format, IFormatProvider provider) {
            switch (NumericHelper.ParseFormatSpecifier(format, out var digits)) {
                case 'x':
                    {
                    return $"{b:x}{a:x}";
                    }
                }
            return "{?}";
            }

        public String ToString(String format) {
            return ToString(format, CultureInfo.CurrentCulture);
            }

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>The fully qualified type name.</returns>
        public override String ToString()
            {
            return ToString("x");
            }

        public static explicit operator UInt512(Byte   source) { return new UInt512(UInt256.MinValue, (UInt256)source); }
        public static explicit operator UInt512(UInt16 source) { return new UInt512(UInt256.MinValue, (UInt256)source); }
        public static explicit operator UInt512(UInt32 source) { return new UInt512(UInt256.MinValue, (UInt256)source); }
        public static explicit operator UInt512(UInt64 source) { return new UInt512(UInt256.MinValue, (UInt256)source); }
        public static explicit operator UInt512(UInt128 source) {
            return new UInt512(new []{
                UInt128.MinValue,
                UInt128.MinValue,
                UInt128.MinValue,
                source
                }, NumericSourceFlags.BigEndian);
            }
        public static explicit operator UInt512(UInt192 source) {
            return new UInt512(new []{
                UInt256.MinValue,
                (UInt256)source
                }, NumericSourceFlags.BigEndian);
            }
        public static explicit operator UInt512(UInt224 source) {
            return new UInt512(new []{
                UInt256.MinValue,
                (UInt256)source
                }, NumericSourceFlags.BigEndian);
            }
        public static explicit operator UInt512(UInt256 source) {
            return new UInt512(new []{
                UInt256.MinValue,
                source
                }, NumericSourceFlags.BigEndian);
            }
        public static unsafe explicit operator UInt512(UInt384 source) {
            return new UInt512(new []{
                0U,
                0U,
                0U,
                0U,
                source.value[11],
                source.value[10],
                source.value[ 9],
                source.value[ 8],
                source.value[ 7],
                source.value[ 6],
                source.value[ 5],
                source.value[ 4],
                source.value[ 3],
                source.value[ 2],
                source.value[ 1],
                source.value[ 0],
                }, NumericSourceFlags.BigEndian);
            }
        }
    }