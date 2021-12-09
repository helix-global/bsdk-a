using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable once LocalVariableHidesMember

namespace BinaryStudio.Numeric
    {
    [StructLayout(LayoutKind.Explicit,Pack = 1)]
    public struct UInt128 : IComparable<UInt128>, IComparable, IEquatable<UInt128>
        {
        public static readonly UInt128 Zero     = new UInt128(UInt64.MinValue, UInt64.MinValue);
        public static readonly UInt128 MinValue = new UInt128(UInt64.MinValue, UInt64.MinValue);
        public static readonly UInt128 MaxValue = new UInt128(UInt64.MaxValue, UInt64.MaxValue);

        [FieldOffset(0)] private unsafe fixed UInt32 value[4];
        [FieldOffset(0)] private UInt64 a;
        [FieldOffset(8)] private UInt64 b;

        /// <summary>
        /// Constructs <see cref="UInt128"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt128(UInt32[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 4) { throw new ArgumentOutOfRangeException(nameof(source)); }
            a = 0;
            b = 0;
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
            a = 0;
            b = 0;
            fixed (void* target = value) {
                ((UInt64*)target)[0] = source[1];
                ((UInt64*)target)[1] = source[0];
                }
            }

        public unsafe UInt128(ref UInt64 hi, ref UInt64 lo) {
            a = 0;
            b = 0;
            fixed (void* target = value) {
                ((UInt64*)target)[0] = lo;
                ((UInt64*)target)[1] = hi;
                }
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

        public static unsafe UInt128 operator +(UInt128 x, UInt32 y) {
            if (y == 0) { return x; }
            var r = new UInt32[5];
            UInt64 α;
            var β = (UInt32*)&α;
            α = x.value[0] + (UInt64)y;
            r[0]  = β[0];
            var γ = β[1];
            for (var i = 1; i < 4; i++) {
                α = x.value[i] + (UInt64)γ;
                r[i] = β[0];
                γ    = β[1];
                }
            r[4] = γ;
            return x;
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