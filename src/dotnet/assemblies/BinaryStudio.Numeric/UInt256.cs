using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable once LocalVariableHidesMember

namespace BinaryStudio.Numeric
    {
    [StructLayout(LayoutKind.Explicit,Pack = 1)]
    public struct UInt256 : IComparable<UInt256>, IEquatable<UInt256>, IComparable
        {
        public static readonly UInt256 Zero     = new UInt256(UInt128.MinValue, UInt128.MinValue);
        public static readonly UInt256 MinValue = new UInt256(UInt128.MinValue, UInt128.MinValue);
        public static readonly UInt256 MaxValue = new UInt256(UInt128.MaxValue, UInt128.MaxValue);

        [FieldOffset( 0)] internal unsafe fixed UInt32 value[8];
        [FieldOffset( 0)] internal UInt128 a;
        [FieldOffset(16)] internal UInt128 b;

        /// <summary>
        /// Constructs <see cref="UInt256"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flags"></param>
        private unsafe UInt256(UInt32[] source, NumericSourceFlags flags)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 8) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt128.MinValue;
            b = UInt128.MinValue;
            if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                value[0] = source[7];
                value[1] = source[6];
                value[2] = source[5];
                value[3] = source[4];
                value[4] = source[3];
                value[5] = source[2];
                value[6] = source[1];
                value[7] = source[0];
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
                value[7] = source[7];
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt256"/> structure from <see cref="UInt64"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flags"></param>
        private unsafe UInt256(UInt64[] source, NumericSourceFlags flags) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 4) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt128.MinValue;
            b = UInt128.MinValue;
            fixed (void* target = value) {
                if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                    ((UInt64*)target)[0] = source[3];
                    ((UInt64*)target)[1] = source[2];
                    ((UInt64*)target)[2] = source[1];
                    ((UInt64*)target)[3] = source[0];
                    }
                else
                    {
                    ((UInt64*)target)[0] = source[0];
                    ((UInt64*)target)[1] = source[1];
                    ((UInt64*)target)[2] = source[2];
                    ((UInt64*)target)[3] = source[3];
                    }
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt256"/> structure from <see cref="UInt128"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flags"></param>
        private unsafe UInt256(UInt128[] source, NumericSourceFlags flags) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 2) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt128.MinValue;
            b = UInt128.MinValue;
            fixed (void* target = value) {
                if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                    ((UInt128*)target)[0] = source[1];
                    ((UInt128*)target)[1] = source[0];
                    }
                else
                    {
                    ((UInt128*)target)[0] = source[0];
                    ((UInt128*)target)[1] = source[1];
                    }
                }
            }

        public UInt256(ref UInt128 hi, ref UInt128 lo) {
            a = lo;
            b = hi;
            }

        private UInt256(UInt128 hi, UInt128 lo)
            :this(ref hi, ref lo)
            {
            }

        /// <summary>
        /// Constructs <see cref="UInt128"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        private unsafe UInt256(UInt32[] source, Int32 firstindex, Int32 size, NumericSourceFlags flags)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (firstindex < 0) { throw new ArgumentOutOfRangeException(nameof(firstindex)); }
            if (size != 8) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt128.MinValue;
            b = UInt128.MinValue;
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

        public Int32 CompareTo(UInt256 other)
            {
            return CompareTo(ref other);
            }

        public Int32 CompareTo(ref UInt256 other) {
            Int32 r;
            return (r =
                b.CompareTo(other.b)) != 0 ? r :
                a.CompareTo(other.a);
            }

        #region M:CompareTo(UInt128):Int32
        public Int32 CompareTo(UInt128 other)
            {
            return CompareTo(ref other);
            }
        #endregion
        #region M:CompareTo(UInt64):Int32
        public Int32 CompareTo(UInt64 other)
            {
            Int32 r;
            return (r =
                b.CompareTo(UInt128.MinValue)) != 0 ? r :
                a.CompareTo(other);
            }
        #endregion
        #region M:CompareTo(Int64):Int32
        public Int32 CompareTo(Int64 other)
            {
            if (other < 0) { return +1; }
            return CompareTo((UInt64)other);
            }
        #endregion
        #region M:CompareTo([ref]UInt128):Int32
        public Int32 CompareTo(ref UInt128 other)
            {
            Int32 r;
            return (r =
                b.CompareTo(UInt128.MinValue)) != 0 ? r :
                a.CompareTo(other);
            }
        #endregion

        public Int32 CompareTo(Object other) {
            if (other == null) { return +1; }
            if (other is UInt256 value) {
                return CompareTo(ref value);
                }
            throw new ArgumentOutOfRangeException(nameof(other));
            }

        /// <summary>Returns the remainder that results from division with two specified <see cref="UInt256"/> and <see cref="UInt32"/> values.</summary>
        /// <param name="x">The value to be divided.</param>
        /// <param name="y">The value to divide by.</param>
        /// <returns>The remainder that results from the division.</returns>
        /// <exception cref="T:System.DivideByZeroException"><paramref name="y"/> is 0 (zero).</exception>
        public static unsafe UInt32 operator %(UInt256 x, UInt32 y)
            {
            if (y == 0)   { throw new DivideByZeroException(); }
            if (x.b == 0) { return (x.a%y); }
            var r = 0L;
            for (var i = 7; i >= 0; i--) {
                var α = (Int64)x.value[i];
                var β = r << 32;
                var γ = (β | α);
                r = (γ %y);
                }
            return (UInt32)r;
            }

        /// <summary>Returns the remainder that results from division with two specified <see cref="UInt256"/> and <see cref="Int32"/> values.</summary>
        /// <param name="x">The value to be divided.</param>
        /// <param name="y">The value to divide by.</param>
        /// <returns>The remainder that results from the division.</returns>
        /// <exception cref="T:System.DivideByZeroException"><paramref name="y"/> is 0 (zero).</exception>
        public static Int32 operator %(UInt256 x, Int32 y) {
            if (y == 0) { throw new DivideByZeroException(); }
            return (y < 0)
                ? -(Int32)(x % (UInt32)(-y))
                : +(Int32)(x % (UInt32)(+y));
            }

        #region M:operator ==(UInt256,UInt256):Boolean
        public static Boolean operator ==(UInt256 x, UInt256 y)
            {
            return x.Equals(ref y);
            }
        #endregion
        #region M:operator !=(UInt256,UInt256):Boolean
        public static Boolean operator !=(UInt256 x, UInt256 y)
            {
            return !x.Equals(ref y);
            }
        #endregion
        #region M:operator ==(UInt256,UInt64):Boolean
        public static Boolean operator ==(UInt256 x, UInt64 y)
            {
            return x.Equals(y);
            }
        #endregion
        #region M:operator !=(UInt128,UInt64):Boolean
        public static Boolean operator !=(UInt256 x, UInt64 y)
            {
            return !x.Equals(y);
            }
        #endregion
        #region M:operator ==(UInt256,Int64):Boolean
        public static Boolean operator ==(UInt256 x, Int64 y)
            {
            return x.Equals(y);
            }
        #endregion
        #region M:operator !=(UInt256,Int64):Boolean
        public static Boolean operator !=(UInt256 x, Int64 y)
            {
            return !x.Equals(y);
            }
        #endregion

        /// <summary>Divides a specified <see cref="UInt256"/> value by another specified <see cref="UInt32"/> value by using integer division.</summary>
        /// <param name="x">The value to be divided.</param>
        /// <param name="y">The value to divide by.</param>
        /// <returns>The integral result of the division.</returns>
        /// <exception cref="T:System.DivideByZeroException"><paramref name="y"/> is 0 (zero).</exception>
        public static unsafe UInt256 operator /(UInt256 x, UInt32 y)
            {
            if (y == 0) { throw new DivideByZeroException(); }
            return (y == 0)
                ? x
                : new UInt256(
                    NumericHelper.Div(x.value, 8, y),
                    0, 8, 0);
            }

        #region M:operator >(UInt256,UInt64):Boolean
        public static Boolean operator >(UInt256 x, UInt64 y)
            {
            return x.CompareTo(y) > 0;
            }
        #endregion
        #region M:operator <(UInt256,UInt64):Boolean
        public static Boolean operator <(UInt256 x, UInt64 y)
            {
            return x.CompareTo(y) < 0;
            }
        #endregion
        #region M:operator <(UInt256,Int64):Boolean
        public static Boolean operator <(UInt256 x, Int64 y)
            {
            return x.CompareTo(y) < 0;
            }
        #endregion
        #region M:operator >(UInt256,Int64):Boolean
        public static Boolean operator >(UInt256 x, Int64 y)
            {
            return x.CompareTo(y) > 0;
            }
        #endregion

        public String ToString(String format, IFormatProvider provider) {
            switch (NumericHelper.ParseFormatSpecifier(format, out var digits)) {
                case 'x':
                    {
                    return $"{b:x}{a:x}";
                    }
                case 'r':
                case 'R':
                    {
                    var q = this;
                    var o = new StringBuilder();
                    var r = q%10;
                        q = q/10;
                    o.Append((char)('0' + r));
                    for (;q > 0;) {
                        r = q%10;
                        q = q/10;
                        o.Insert(0, (char)('0' + r));
                        }
                    return o.ToString();
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

        /// <summary>Performs a bitwise <see langword="or"/> operation on two <see cref="UInt256"/> values.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The result of the bitwise <see langword="or"/> operation.</returns>
        public static UInt256 operator |(UInt256 x, UInt256 y)
            {
            return new UInt256{
                a = x.a | y.a,
                b = x.b | y.b
                };
            }

        /// <summary>Performs a bitwise <see langword="and"/> operation on two <see cref="UInt256"/> values.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The result of the bitwise <see langword="and"/> operation.</returns>
        public static UInt256 operator &(UInt256 x, UInt256 y)
            {
            return new UInt256{
                a = x.a & y.a,
                b = x.b & y.b
                };
            }

        /// <summary>Performs a bitwise exclusive <see langword="or"/> (<see langword="xor"/>) operation on two <see cref="UInt256"/> values.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The result of the bitwise <see langword="or"/> operation.</returns>
        public static UInt256 operator ^(UInt256 x, UInt256 y)
            {
            return new UInt256{
                a = x.a ^ y.a,
                b = x.b ^ y.b
                };
            }

        public static explicit operator UInt256(Byte    source) { return new UInt256(UInt128.MinValue, (UInt128)source); }
        public static explicit operator UInt256(UInt16  source) { return new UInt256(UInt128.MinValue, (UInt128)source); }
        public static explicit operator UInt256(UInt32  source) { return new UInt256(UInt128.MinValue, (UInt128)source); }
        public static explicit operator UInt256(UInt64  source) { return new UInt256(UInt128.MinValue, (UInt128)source); }
        public static explicit operator UInt256(UInt128 source) { return new UInt256(UInt128.MinValue,          source); }
        public static unsafe explicit operator UInt256(UInt192 source) {
            return new UInt256(new []{
                0U, 0U,
                source.value[5],
                source.value[4],
                source.value[3],
                source.value[2],
                source.value[1],
                source.value[0],
                }, NumericSourceFlags.BigEndian);
            }
        public static unsafe explicit operator UInt256(UInt224 source) {
            return new UInt256(new []{
                0U,
                source.value[6],
                source.value[5],
                source.value[4],
                source.value[3],
                source.value[2],
                source.value[1],
                source.value[0],
                }, NumericSourceFlags.BigEndian);
            }

        #region M:Equals(UInt256):Boolean
        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(UInt256 other)
            {
            return Equals(ref other);
            }
        #endregion
        #region M:Equals([ref]UInt256):Boolean
        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(ref UInt256 other)
            {
            return (a == other.a) &&
                   (b == other.b);
            }
        #endregion
        #region M:Equals(UInt128):Boolean
        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(UInt128 other)
            {
            return Equals(ref other);
            }
        #endregion
        #region M:Equals([ref]UInt128):Boolean
        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(ref UInt128 other)
            {
            return (b == UInt128.MinValue) &&
                   (a == other);
            }
        #endregion
        #region M:Equals(UInt64):Boolean
        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(UInt64 other)
            {
            return (b == UInt128.MinValue) &&
                   (a == other);
            }
        #endregion
        #region M:Equals(Int64):Boolean
        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(Int64 other)
            {
            if (other < 0) { return false; }
            return Equals((UInt64)other);
            }
        #endregion
        }
    }