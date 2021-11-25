using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryStudio.IO
    {
    public class Base32Formatter
        {
        private static String FormatInternal(Byte[] buffer, Int32 length) {
            var builder = new StringBuilder();
            var ascii   = new StringBuilder();
            for (var i = 0; i < length; i++) {
                var c = buffer[i];
                if (i > 0) {
                    builder.Append(" ");
                    if (i == 8) {
                        builder.Append(" ");
                        }
                    }
                builder.Append(c.ToString("X2"));
                if (((c >= 'A') && (c <= 'Z')) ||
                    ((c >= 'a') && (c <= 'z')) ||
                    ((c >= '0') && (c <= '9')) ||
                    ((c >= 0x20) && (c <= 0x7E)))
                    {
                    ascii.Append((char)c);
                    }
                else
                    {
                    ascii.Append('.');
                    }
                }
            ascii.Append(new String(' ', 16 - length));
            var n = 16 - length;
            if (n > 0) {
                n *= 3;
                if (n < 8)
                    {
                    n++;
                    }
                builder.Append(new String(' ', n));
                }
            builder.Append(' ');
            builder.Append(ascii);
            return builder.ToString();
            }

        public static IEnumerable<String> Format(Stream source, Base32FormattingFlags flags) {
            var buffer = new Byte[16];
            var offset = 0;
            if (flags != Base32FormattingFlags.None) {
                if (flags.HasFlag(Base32FormattingFlags.Offset) && flags.HasFlag(Base32FormattingFlags.Header))
                    {
                    yield return " OFFSET   00 01 02 03 04 05 06 07  08 09 0A 0B 0C 0D 0E 0F      ASCII      ";
                    yield return "---------------------------------------------------------------------------";
                    }
                else if (flags.HasFlag(Base32FormattingFlags.Header))
                    {
                    yield return "00 01 02 03 04 05 06 07  08 09 0A 0B 0C 0D 0E 0F      ASCII      ";
                    yield return "-----------------------------------------------------------------";
                    }
                }
            while (true) {
                var length = source.Read(buffer, 0, 16);
                if (length == 0) { break; }
                var output = FormatInternal(buffer, length);
                if (flags.HasFlag(Base32FormattingFlags.Offset))
                    {
                    yield return $"{offset:X8}  {output}";
                    }
                else
                    {
                    yield return output;
                    }
                offset += 16;
                }
            }

        public static IEnumerable<String> Format(Byte[] source, Int32 length, Int32 offset, Base32FormattingFlags flags) {
            var buffer = new Byte[16];
            if (flags != Base32FormattingFlags.None) {
                if (flags.HasFlag(Base32FormattingFlags.Offset) && flags.HasFlag(Base32FormattingFlags.Header))
                    {
                    yield return " OFFSET   00 01 02 03 04 05 06 07  08 09 0A 0B 0C 0D 0E 0F      ASCII      ";
                    yield return "---------------------------------------------------------------------------";
                    }
                else if (flags.HasFlag(Base32FormattingFlags.Header))
                    {
                    yield return "00 01 02 03 04 05 06 07  08 09 0A 0B 0C 0D 0E 0F      ASCII      ";
                    yield return "-----------------------------------------------------------------";
                    }
                }
            while (length > 0) {
                if (length >= 16) {
                    Array.Copy(source, offset, buffer, 0, 16);
                    var output = FormatInternal(buffer, 16);
                    if (flags.HasFlag(Base32FormattingFlags.Offset))
                        {
                        yield return $"{offset:X8}  {output}";
                        }
                    else
                        {
                        yield return output;
                        }
                    
                    length -= 16;
                    offset += 16;
                    }
                else
                    {
                    Array.Copy(source, offset, buffer, 0, length);
                    var output = FormatInternal(buffer, length);
                    if (flags.HasFlag(Base32FormattingFlags.Offset))
                        {
                        yield return $"{offset:X8}  {output}";
                        }
                    else
                        {
                        yield return output;
                        }
                    length = 0;
                    offset += 16;
                    }
                }
            }

        public static IEnumerable<String> ToBase32Strings(Byte[] source) {
            var buffer = new Byte[16];
            var length = source.Length;
            var offset = 0;
            while (length > 0) {
                var count = Math.Min(length, 16);
                Array.Copy(source, offset, buffer, 0, count);
                var output = new StringBuilder();
                for (var i = 0; i < count; i++)
                    {
                    output.Append($"{buffer[i]:X2}");
                    }
                yield return output.ToString();
                length -= 16;
                offset += 16;
                }
            }
        }
    }