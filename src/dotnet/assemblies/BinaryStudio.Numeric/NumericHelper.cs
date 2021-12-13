using System;

namespace BinaryStudio.Numeric
    {
    internal static class NumericHelper
        {
        #region M:ParseFormatSpecifier(String,[Out]Int32):Char
        public static char ParseFormatSpecifier(String format, out Int32 digits) {
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

        public static unsafe UInt32[] Add(UInt32* x, Int32 c, UInt32 y) {
            var r = new UInt32[c + 1];
            UInt64 α;
            var β = (UInt32*)&α;
            α = x[0] + (UInt64)y;
            r[0]  = β[0];
            var γ = β[1];
            for (var i = 1; i < c; i++) {
                α = x[i] + (UInt64)γ;
                r[i] = β[0];
                γ    = β[1];
                }
            r[c] = γ;
            return r;
            }

        public static unsafe UInt32[] Sub(UInt32* x, Int32 c, UInt32 y) {
            var r = new UInt32[c];
            Int64 α;
            var β = (UInt32*)&α;
            α = x[0] - (Int64)y;
            r[0]  = β[0];
            var γ = unchecked((Int32)β[1]);
            for (var i = 1; i < c; i++) {
                α = x[i] + (Int64)γ;
                r[i] = β[0];
                γ    = unchecked((Int32)β[1]);
                }
            return r;
            }

        public static unsafe UInt32[] Div(UInt32* x, Int32 c, UInt32 y) {
            var r = new UInt32[c];
            var α = 0UL;
            for (var i = c- 1; i >= 0; i--) {
                var β = (α << 32) | x[i];
                var γ = β/y;
                r[i] = (UInt32)γ;
                α = β - γ*y;
                }
            return r;
            }

        public static Byte[] Shl(Byte[] source, Int32 count)
            {
            return Shl(source,source,count);
            }

        public static unsafe Byte[] Shl(Byte[] source, Byte[] target, Int32 count)
            {
            if (source.Length != target.Length) { throw new ArgumentOutOfRangeException(nameof(target)); }
            if (count == 0)   { return source; }
            if (count  < 0)   { return Shr(source, target,-count); }
            fixed (Byte* x = source)
            fixed (Byte* y = target)
                {
                Shl(x, y, source.Length, count);
                }
            return target;
            }

        public static unsafe Byte* Shl(Byte* source, Byte* target, Int32 length, Int32 count)
            {
            if (count == 0)   { return source; }
            if (count  < 0)   { return Shr(source, target,length, -count); }
            if (count >= length*8) {
                for (var i = 0; i < length; i++) {
                    target[i] = 0;
                    }
                return target;
                }
            if (count%8 == 0) {
                var offset = count/8;
                for (var i = 0; i < length - offset;i++) {
                    target[i + offset] = source[i];
                    }
                return target;
                }
            else
                {
                var shift  = count%8;
                var offset = count/8;
                var j = 0;
                for (var i = length-1; (i >= offset);i--,j++) {
                    if (j == 0)
                        {
                        target[i] = source[i - offset];
                        target[i] <<= shift;
                        }
                    else
                        {
                        var value = (UInt16)(source[i - offset]);
                        value <<= shift;
                        target[i] = (Byte)(value & 0xff);
                        target[i + 1] |= (Byte)((value >> 8) & 0xff);
                        }
                    }
                return target;
                }
            throw new NotImplementedException();
            }

        public static Byte[] Shr(Byte[] source, Int32 count)
            {
            return Shr(source, source, count);
            }

        public static unsafe Byte[] Shr(Byte[] source, Byte[] target, Int32 count)
            {
            if (source.Length != target.Length) { throw new ArgumentOutOfRangeException(nameof(target)); }
            if (count == 0)   { return target; }
            if (count  < 0)   { return Shl(source, target, -count); }
            fixed (Byte* x = source)
            fixed (Byte* y = target)
                {
                Shr(x, y, source.Length, count);
                }
            return target;
            }

        public static unsafe Byte* Shr(Byte* source, Byte* target, Int32 length, Int32 count)
            {
            if (count == 0)   { return target; }
            if (count  < 0)   { return Shl(source, target, length, -count); }
            if (count >= length*8) {
                for (var i = 0; i < length; i++) {
                    target[i] = 0;
                    }
                return target;
                }
            throw new NotImplementedException();
            }

        public static Int32 GetHashCode(UInt32 x, UInt32 y)
            {
            return unchecked((Int32)(((x << 7) | (x >> 25)) ^ y));
            }

        public static Int32 GetHashCode(Int32 x, Int32 y)
            {
            return GetHashCode(
                unchecked((UInt32)x),
                unchecked((UInt32)y));
            }

        public static Int32 GetHashCode(UInt64 r1)
            {
            return GetHashCode(
                (UInt32)(r1 & 0xffffffff),
                (UInt32)((r1 >> 32) & 0xffffffff));
            }
        }
    }