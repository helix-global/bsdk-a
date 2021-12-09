using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace BinaryStudio.Numeric
    {
    public class LongInteger
        {
        private readonly UInt32[] magnitude;
        private readonly Int32 sign;

        public LongInteger(Byte value) {
            magnitude = EmptyArray<UInt32>.Value;
            sign = value;
            magnitude = new UInt32[]{ value };
            }

        public LongInteger(SByte value) {
            magnitude = EmptyArray<UInt32>.Value;
            sign = value;
            }

        public LongInteger(Int32 value) {
            magnitude = EmptyArray<UInt32>.Value;
            sign = value;
            }

        public LongInteger(UInt32 value) {
            magnitude = EmptyArray<UInt32>.Value;
            if (value <= Int32.MaxValue) {
                sign = (Int32)value;
                return;
                }
            sign = 1;
            magnitude = new []{value};
            }

        public unsafe LongInteger(UInt64 value) {
            magnitude = EmptyArray<UInt32>.Value;
            if (value <= Int32.MaxValue) {
                sign = (Int32)value;
                return;
                }
            sign = 1;
            var r = (UInt32*)&value;
            if (value <= UInt32.MaxValue) {
                magnitude = new [] {
                    r[0]
                    };
                }
            else
                {
                magnitude = new [] {
                    r[0],
                    r[1]
                    };
                }
            }

        public unsafe LongInteger(Int64 value) {
            magnitude = EmptyArray<UInt32>.Value;
            if ((value >= Int32.MinValue) && (value <= Int32.MaxValue)) {
                sign = (Int32)value;
                return;
                }
            UInt64 α;
            if (value < 0)
                {
                α = (UInt64)(-value);
                sign = -1;
                }
            else
                {
                α = (UInt64)(+value);
                sign = 1;
                }
            var β = (UInt32*)&α;
            if (α <= UInt32.MaxValue) {
                magnitude = new []
                    {
                    β[0]
                    };
                }
            else
                {
                magnitude = new []
                    {
                    β[0],
                    β[1]
                    };
                }
            }

        public LongInteger(Byte[] value, Boolean U = false, Boolean B = false) {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            magnitude = EmptyArray<UInt32>.Value;
            var c = value.Length;
            if (c > 0) {
                var left  = 0;
                var right = c - 1;
                var F = B
                    ? value[left ]
                    : value[right];
                if (F == 0) {
                    /* skip first lead zero bytes */
                    if (B) {
                        for (var i = 1; (i <= right) && (value[i] == 0); i++)
                            {
                            left++;
                            }
                        }
                    else
                        {
                        for (var i = right - 1; (i >= left) && (value[i] == 0);i--) {
                            right--;
                            }
                        }
                    }
                if (right != left) {
                    //var size = right-left;
                    //magnitude = new Byte[];
                    //Array.Copy(value,left,magnitude,0,right-left);
                    }
                }
            else
                {
                
                }
            }

        private LongInteger(Boolean neg, UInt32[] value) {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            magnitude = EmptyArray<UInt32>.Value;
            var c = value.Length;
            while ((c > 0) && (value[c - 1] == 0)) {
                c--;
                }
            switch (c)
                {
                case 0:
                    {
                    this.sign = 0;
                    return;
                    }
                case 1:
                    {
                    if (neg)
                        {
                        if (value[0] <= ((UInt32)Int32.MaxValue + 1)) {
                            sign = -(Int32)value[0];
                            return;
                            }
                        }
                    else
                        {
                        if (value[0] <= Int32.MaxValue) {
                            sign = (Int32)value[0];
                            return;
                            }
                        }
                    }
                    break;
                }
            sign = (neg)
                ? -1
                : +1;
            magnitude = new UInt32[c];
            Array.Copy(value, magnitude, c);
            }

        #region M:Divide(UInt32[],UInt32):UInt32[]
        private static UInt32[] Divide(UInt32[] x, UInt32 y) {
            var c = x.Length;
            var r = new UInt32[c];
            var α = 0UL;
            for (var i = c - 1; i >= 0; i--) {
                var β = (α << 32) | x[i];
                var γ = β/y;
                r[i] = (UInt32)γ;
                α = β - γ*y;
                }
            return r;
            }
        #endregion
        #region M:Divide(UInt32[],UInt32[]):UInt32[]
        private static UInt32[] Divide(UInt32[] x, UInt32[] y) {
            var r = EmptyArray<UInt32>.Value;
            var α = x.Length;
            var β = y.Length;
            if (α < β) { return r; }
            if (β == 1) { return Divide(x, y[0]); }

            return r;
            }
        #endregion
        private static LongInteger Subtract(UInt32[] x, UInt32 y) {
            var γ = 0L;
            var α = x.Length;
            if (α == 0) { return new LongInteger(-(Int64)y); }
            if (α == 1) { return new LongInteger(((Int64)x[0]) - ((Int64)y)); }
            var r = new UInt32[x.Length];
            var β = (Int64)x[0] - (Int64)y;
            r[0] = (UInt32)β;
            for (var i = 1; i < α; i++) {
                β = x[i] + γ;
                r[i] = (UInt32)β;
                γ = β >> 32;
                }
            return new LongInteger(false, r);
            } 

        //private static LongInteger Subtract(UInt32[] x, UInt32[] y) {
        //    var γ = 0L;
        //    var α = x.Length;
        //    if (α == 0) { return new LongInteger(-(Int64)y); }
        //    if (α == 1) { return new LongInteger(((Int64)x[0]) - ((Int64)y)); }
        //    var r = new UInt32[x.Length];
        //    var β = (Int64)x[0] - (Int64)y;
        //    r[0] = (UInt32)β;
        //    for (var i = 1; i < α; i++) {
        //        β = x[i] + γ;
        //        r[i] = (UInt32)β;
        //        γ = β >> 32;
        //        }
        //    return new LongInteger(r);
        //    }

        #region M:CompareTo(Int64):Int32
        public Int32 CompareTo(Int64 other) {
            var c = magnitude.Length;
            if (c == 0) { return ((Int64)sign).CompareTo(other); }
            if (((sign ^ other) < 0) || (c > 2)) { return sign; }
            var α = (other < 0)
                ? (UInt64)(-other)
                : (UInt64)(+other);
            var β = (c == 2)
                ? (((UInt64)magnitude[1]) << 32) | magnitude[0]
                : magnitude[0];
            return sign*(β.CompareTo(α));
            }
        #endregion
        #region M:CompareTo(UInt64):Int32
        public Int32 CompareTo(UInt64 other) {
            if (sign < 0) { return -1; }
            var c = magnitude.Length;
            if (c == 0) { return ((UInt64)sign).CompareTo(other); }
            if (c > 2) { return 1; }
            var β = (c == 2)
                ? (((UInt64)magnitude[1]) << 32) | magnitude[0]
                : magnitude[0];
            return β.CompareTo(other);
            }
        #endregion
        #region M:operator -(LongInteger,UInt32):LongInteger
        public static unsafe LongInteger operator -(LongInteger x, UInt32 y) {
            if (y == 0) { return x; }
            var c = x.magnitude.Length;
            if (c == 0) { return new LongInteger(((Int64)x.sign) + y); }
            if (x.sign < 0) {
                UInt64 α;
                var β = (UInt32*)&α;
                var r = new UInt32[c + 1];
                α = x.magnitude[0] + (UInt64)y;
                r[0]  = β[0];
                var γ = β[1];
                for (var i = 1; i <c; i++) {
                    α = x.magnitude[i] + (UInt64)γ;
                    r[i] = β[0];
                    γ    = β[1];
                    }
                r[c] = γ;
                return new LongInteger(true, r);
                }
            else
                {
                Int64 α;
                var β = (UInt32*)&α;
                var r = new UInt32[c];
                α = ((Int64)x.magnitude[0]) - (Int64)y;
                r[0]  = β[0];
                var γ = (Int64)β[1];
                for (var i = 1; i <c; i++) {
                    α = (x.magnitude[i]) + γ;
                    r[i] = β[0];
                    γ    = β[1];
                    }
                return new LongInteger(false, r);
                }
            }
        #endregion
        #region M:operator -(LongInteger,Int32):LongInteger
        public static LongInteger operator -(LongInteger x, Int32 y) {
            return (y < 0)
                ? (x + (UInt32)(-y))
                : (x - (UInt32)(+y));
            }
        #endregion
        #region M:operator +(LongInteger,UInt32):LongInteger
        public static unsafe LongInteger operator +(LongInteger x, UInt32 y) {
            if (y == 0) { return x; }
            var c = x.magnitude.Length;
            if (c == 0) { return new LongInteger(((Int64)x.sign) + y); }
            if (x.sign > 0) {
                UInt64 α;
                var β = (UInt32*)&α;
                var r = new UInt32[c + 1];
                α = x.magnitude[0] + (UInt64)y;
                r[0]  = β[0];
                var γ = β[1];
                for (var i = 1; i <c; i++) {
                    α = x.magnitude[i] + (UInt64)γ;
                    r[i] = β[0];
                    γ    = β[1];
                    }
                r[c] = γ;
                return new LongInteger(false, r);
                }
            else
                {
                Int64 α;
                var β = (UInt32*)&α;
                var r = new UInt32[c];
                α = ((Int64)x.magnitude[0]) - (Int64)y;
                r[0]  = β[0];
                var γ = (Int64)β[1];
                for (var i = 1; i <c; i++) {
                    α = (x.magnitude[i]) + γ;
                    r[i] = β[0];
                    γ    = β[1];
                    }
                return new LongInteger(true, r);
                }
            }
        #endregion
        #region M:operator +(LongInteger,Int32):LongInteger
        public static LongInteger operator +(LongInteger x, Int32 y) {
            return (y < 0)
                ? (x - (UInt32)(-y))
                : (x + (UInt32)(+y));
            }
        #endregion

        public static Boolean operator <(LongInteger x, Int32 y)
            {
            return x.CompareTo(y) < 0;
            }

        public static Boolean operator >(LongInteger x, Int32 y)
            {
            return x.CompareTo(y) > 0;
            }

        #region M:operator -(LongInteger):LongInteger
        public static LongInteger operator -(LongInteger x)
            {
            var c = x.magnitude.Length;
            if (c == 0) { return new LongInteger(-(Int64)x.sign); }
            return (x.sign > 0)
                ? new LongInteger(true,  x.magnitude)
                : new LongInteger(false, x.magnitude);
            }
        #endregion
        #region M:operator ~(LongInteger):LongInteger
        public static LongInteger operator ~(LongInteger x)
            {
            return -(x + 1);
            }
        #endregion
        #region M:operator /(LongInteger,UInt32):LongInteger
        public static LongInteger operator /(LongInteger x, UInt32 y) {
            if (y == 1) { return x; }
            switch (y) {
                case 0x000000002: return (x >> 0x01);
                case 0x000000004: return (x >> 0x02);
                case 0x000000008: return (x >> 0x03);
                case 0x000000010: return (x >> 0x04);
                case 0x000000020: return (x >> 0x05);
                case 0x000000040: return (x >> 0x06);
                case 0x000000080: return (x >> 0x07);
                case 0x000000100: return (x >> 0x08);
                case 0x000000200: return (x >> 0x09);
                case 0x000000400: return (x >> 0x0a);
                case 0x000000800: return (x >> 0x0b);
                case 0x000001000: return (x >> 0x0c);
                case 0x000002000: return (x >> 0x0d);
                case 0x000004000: return (x >> 0x0e);
                case 0x000008000: return (x >> 0x0f);
                case 0x000010000: return (x >> 0x10);
                case 0x000020000: return (x >> 0x11);
                case 0x000040000: return (x >> 0x12);
                case 0x000080000: return (x >> 0x13);
                case 0x000100000: return (x >> 0x14);
                case 0x000200000: return (x >> 0x15);
                case 0x000400000: return (x >> 0x16);
                case 0x000800000: return (x >> 0x17);
                case 0x001000000: return (x >> 0x18);
                case 0x002000000: return (x >> 0x19);
                case 0x004000000: return (x >> 0x1a);
                case 0x008000000: return (x >> 0x1b);
                case 0x010000000: return (x >> 0x1c);
                case 0x020000000: return (x >> 0x1d);
                case 0x040000000: return (x >> 0x1e);
                case 0x080000000: return (x >> 0x1f);
                }
            var c = x.magnitude.Length;
            if (c == 0) { return new LongInteger((Int64)x.sign/(Int64)y); }
            var r = new UInt32[c];
            var α = 0UL;
            for (var i = c - 1; i >= 0; i--) {
                var β = (α << 32) | x.magnitude[i];
                var γ = β/y;
                r[i] = (UInt32)γ;
                α = β - γ*y;
                }
            return new LongInteger((x.sign < 0), r);
            }
        #endregion
        #region M:operator >>(LongInteger,Int32):LongInteger
        public static LongInteger operator >>(LongInteger x, Int32 y)
            {
            if (y == 0) { return x; }
            if (y < 0 ) { return x << -y; }
            return x;
            }
        #endregion
        #region M:operator <<(LongInteger,Int32):LongInteger
        public static LongInteger operator <<(LongInteger x, Int32 y)
            {
            if (y == 0) { return x; }
            if (y < 0 ) { return x >> -y; }
            throw new NotImplementedException();
            return x;
            }
        #endregion

        public unsafe String ToString(String format, IFormatProvider provider) {
            var c = magnitude.Length;
            switch (ParseFormatSpecifier(format, out var didgits)) {
                case 'x':
                    {
                    if (c == 0) { return sign.ToString("x"); }
                    if (sign > 0) {
                        return String.Join(String.Empty, magnitude.
                            Reverse().
                            Select(i => i.ToString("x8")));
                        }
                    var r = new UInt32[c];
                    for (var i = 0; i < c; i++) {
                        r[i] = ~magnitude[i];
                        }
                    return (new LongInteger(false, r) + 1).ToString(format, provider);
                    }
                default:
                    {
                    if (c == 0) { return sign.ToString(null, CultureInfo.CurrentCulture); }
                    //var r = new StringBuilder();
                    //
                    //if (c == 0) { return "0"; }
                    //var j = 0;
                    //for (var i = c - 1; i >= 0; i--) {
                    //    var B = j + magnitude[i];
                    //    var R = B%10;
                    //    var Q = B/10;
                    //    r.Insert(0, (Char)(Q + '0'));
                    //    j = R << 3;
                    //    }
                    //return r.ToString();
                    break;
                    }
                }
            return "{none}";
            }

        public String ToString(String format) {
            return ToString(format, CultureInfo.CurrentCulture);
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return ToString("x");
            }

        #region M:ParseFormatSpecifier(String,[Out]Int32):Char
        internal static char ParseFormatSpecifier(String format, out Int32 digits) {
            digits = -1;
            if ((format == null) || format.Length == 0) { return 'R'; }
            var i = 0;
            var c = format[i];
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')) {
                i++;
                var r = -1;
                if (i < format.Length && format[i] >= '0' && format[i] <= '9') {
                    r = format[i++] - 48;
                    while (i < format.Length && format[i] >= '0' && format[i] <= '9') {
                        r = r * 10 + (format[i++] - 48);
                        if (r >= 10)
                            {
                            break;
                            }
                        }
                    }
                if (i >= format.Length || format[i] == '\0') {
                    digits = r;
                    return c;
                    }
                }
            return '\0';
            }
        #endregion
        }
    }