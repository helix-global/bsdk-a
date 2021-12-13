using System;
using System.Globalization;
using System.Runtime.InteropServices;

// ReSharper disable once LocalVariableHidesMember

namespace BinaryStudio.Numeric
    {
    [StructLayout(LayoutKind.Explicit,Pack = 1)]
    public struct UInt128 : IComparable<UInt128>, IComparable, IEquatable<UInt128>
        {
        public static readonly UInt128 Zero     = new UInt128(UInt64.MinValue, UInt64.MinValue);
        public static readonly UInt128 MinValue = new UInt128(UInt64.MinValue, UInt64.MinValue);
        public static readonly UInt128 MaxValue = new UInt128(UInt64.MaxValue, UInt64.MaxValue);

        [FieldOffset(0)] internal unsafe fixed UInt32 value[4];
        [FieldOffset(0)] internal UInt64 a;
        [FieldOffset(8)] internal UInt64 b;

        /// <summary>
        /// Constructs <see cref="UInt128"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        private unsafe UInt128(UInt32[] source, Int32 firstindex, Int32 size, NumericSourceFlags flags)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (firstindex < 0) { throw new ArgumentOutOfRangeException(nameof(firstindex)); }
            if (size != 4) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt64.MinValue;
            b = UInt64.MinValue;
            if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                value[0] = source[firstindex + 3];
                value[1] = source[firstindex + 2];
                value[2] = source[firstindex + 1];
                value[3] = source[firstindex + 0];
                }
            else
                {
                value[0] = source[firstindex + 0];
                value[1] = source[firstindex + 1];
                value[2] = source[firstindex + 2];
                value[3] = source[firstindex + 3];
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt128"/> structure from <see cref="Byte"/> array by (low-to-high) ordering.
        /// </summary>
        private unsafe UInt128(Byte[] source, NumericSourceFlags flags)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 16) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt64.MinValue;
            b = UInt64.MinValue;
            if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                fixed (void* src = source)
                fixed (void* target = value) {
                    ((UInt64*)target)[0] = ((UInt64*)src)[1];
                    ((UInt64*)target)[1] = ((UInt64*)src)[0];
                    }
                }
            else
                {
                fixed (void* src = source)
                fixed (void* target = value) {
                    ((UInt64*)target)[0] = ((UInt64*)src)[0];
                    ((UInt64*)target)[1] = ((UInt64*)src)[1];
                    }
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt128"/> structure from <see cref="UInt64"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flags"></param>
        private unsafe UInt128(UInt64[] source, NumericSourceFlags flags) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 2) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = UInt64.MinValue;
            b = UInt64.MinValue;
            if (flags.HasFlag(NumericSourceFlags.BigEndian)) {
                fixed (void* target = value) {
                    ((UInt64*)target)[0] = source[1];
                    ((UInt64*)target)[1] = source[0];
                    }
                }
            else
                { 
                fixed (void* target = value) {
                    ((UInt64*)target)[0] = source[0];
                    ((UInt64*)target)[1] = source[1];
                    }
                }
            }

        public UInt128(ref UInt64 hi, ref UInt64 lo) {
            a = lo;
            b = hi;
            }

        public UInt128(UInt64 hi, UInt64 lo)
            :this(ref hi, ref lo)
            {
            }

        #region M:CompareTo(UInt128):Int32
        public Int32 CompareTo(UInt128 other)
            {
            return CompareTo(ref other);
            }
        #endregion
        #region M:CompareTo(UInt64):Int32
        public unsafe Int32 CompareTo(UInt64 other)
            {
            fixed (void* x = value) {
                Int32 r;
                return ((r = 
                    (((UInt64*)x)[1]).CompareTo(0UL)) == 0) ?
                    (((UInt64*)x)[0]).CompareTo(other) : r;
                }
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
        #endregion
        #region M:CompareTo(Object):Int32
        public Int32 CompareTo(Object other) {
            if (other == null) { return +1; }
            if (other is UInt128 value) {
                return CompareTo(ref value);
                }
            throw new ArgumentOutOfRangeException(nameof(other));
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
        public unsafe Boolean Equals(ref UInt128 other)
            {
            fixed (void* x = value)
            fixed (void* y = other.value) {
                return ((((UInt64*)x)[1]) == (((UInt64*)y)[1]))
                    && ((((UInt64*)x)[0]) == (((UInt64*)y)[0]));
                }
            }
        #endregion
        #region M:Equals(UInt64):Boolean
        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public unsafe Boolean Equals(UInt64 other)
            {
            fixed (void* x = value) {
                return ((((UInt64*)x)[1]) == (UInt64.MinValue))
                    && ((((UInt64*)x)[0]) == (other));
                }
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

        #region M:operator ==(UInt128,UInt128):Boolean
        public static Boolean operator ==(UInt128 x, UInt128 y)
            {
            return x.Equals(ref y);
            }
        #endregion
        #region M:operator !=(UInt128,UInt128):Boolean
        public static Boolean operator !=(UInt128 x, UInt128 y)
            {
            return !x.Equals(ref y);
            }
        #endregion
        #region M:operator ==(UInt128,UInt64):Boolean
        public static Boolean operator ==(UInt128 x, UInt64 y)
            {
            return x.Equals(y);
            }
        #endregion
        #region M:operator !=(UInt128,UInt64):Boolean
        public static Boolean operator !=(UInt128 x, UInt64 y)
            {
            return !x.Equals(y);
            }
        #endregion
        #region M:operator ==(UInt128,Int64):Boolean
        public static Boolean operator ==(UInt128 x, Int64 y)
            {
            return x.Equals(y);
            }
        #endregion
        #region M:operator !=(UInt128,Int64):Boolean
        public static Boolean operator !=(UInt128 x, Int64 y)
            {
            return !x.Equals(y);
            }
        #endregion
        #region M:operator >(UInt128,UInt64):Boolean
        public static Boolean operator >(UInt128 x, UInt64 y)
            {
            return x.CompareTo(y) > 0;
            }
        #endregion
        #region M:operator <(UInt128,UInt64):Boolean
        public static Boolean operator <(UInt128 x, UInt64 y)
            {
            return x.CompareTo(y) < 0;
            }
        #endregion
        #region M:operator <(UInt128,Int64):Boolean
        public static Boolean operator <(UInt128 x, Int64 y)
            {
            return x.CompareTo(y) < 0;
            }
        #endregion
        #region M:operator >(UInt128,Int64):Boolean
        public static Boolean operator >(UInt128 x, Int64 y)
            {
            return x.CompareTo(y) > 0;
            }
        #endregion

        public static explicit operator UInt128(Byte   source) { return new UInt128(0, source); }
        public static explicit operator UInt128(UInt16 source) { return new UInt128(0, source); }
        public static explicit operator UInt128(UInt32 source) { return new UInt128(0, source); }
        public static explicit operator UInt128(UInt64 source) { return new UInt128(0, source); }

        public static unsafe UInt128 operator +(UInt128 x, UInt32 y)
            {
            return (y == 0)
                ? x
                : new UInt128(
                    NumericHelper.Add(x.value, 4, y),
                    0, 4, 0);
            }

        public static UInt128 operator |(UInt128 x, UInt128 y)
            {
            return new UInt128{
                a = x.a | y.a,
                b = x.b | y.b
                };
            }

        public static UInt128 operator &(UInt128 x, UInt128 y)
            {
            return new UInt128{
                a = x.a & y.a,
                b = x.b & y.b
                };
            }

        public static UInt128 operator ^(UInt128 x, UInt128 y)
            {
            return new UInt128{
                a = x.a ^ y.a,
                b = x.b ^ y.b
                };
            }

        public static unsafe UInt128 operator <<(UInt128 x, Int32 y) {
            if (y == 0)   { return x; }
            if (y  < 0)   { return x >> (-y); }
            if (y >= 128) { return Zero; }
            if (y%8 == 0) {
                var source = (Byte*)x.value;
                var target = new Byte[16];
                var offset = y/8;
                for (var i = 0; i < 16 - offset;i++) {
                    target[i + offset] = source[i];
                    }
                return new UInt128(target, 0);
                }
            throw new NotImplementedException();
            }

        public static unsafe UInt128 operator >>(UInt128 x, Int32 y) {
            if (y == 0)   { return x; }
            if (y  < 0)   { return x << (-y); }
            if (y >= 128) { return Zero; }
            throw new NotImplementedException();
            }

        public String ToString(String format, IFormatProvider provider) {
            switch (NumericHelper.ParseFormatSpecifier(format, out var digits)) {
                case 'x':
                    {
                    return $"{b:x16}{a:x16}";
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
        }
    }