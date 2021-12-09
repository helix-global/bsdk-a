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
        }
    }