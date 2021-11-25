using System;
using System.IO;
using System.Text;

namespace BinaryStudio.Diagnostics
    {
    public static class Extensions
        {
        public static void Write(this TextWriter writer, Byte[] source) {
            using (var r = new MemoryStream(source)) {
                Write(writer, r);
                }
            }


        private static void WriteInternal(TextWriter writer, Byte[] source, Int32 size, Int32 offset)
            {
            var r = new StringBuilder();
            r.AppendFormat("{0:X8}: ", offset);
            for (var i = 0; i < 16; i++) {
                if (i < 8) {
                    if (i < size) {
                        r.AppendFormat("{0:X2} ", source[i]);
                        }
                    else
                        {
                        r.Append("   ");
                        }
                    }
                else
                    {
                    if (i == 8) {
                        if (i < size) {
                            r.Append("| ");
                            }
                        else
                            {
                            r.Append("  ");
                            }
                        }
                    if (i < size) {
                        r.AppendFormat("{0:X2} ", source[i]);
                        }
                    else
                        {
                        r.Append("   ");
                        }
                    }
                }
            for (var i = 0; i < 16; i++) {
                if (i >= size) { break; }
                var c = source[i];
                if (((c >= 'A') && (c <= 'Z')) ||
                    ((c >= 'a') && (c <= 'z')) ||
                    ((c >= '0') && (c <= '9')) ||
                    ((c >= 0x20) && (c <= 0x7E)))
                    {
                    r.Append((char)c);
                    }
                else
                    {
                    r.Append('.');
                    }
                }
            writer.WriteLine(r.ToString());
            }

        public static void Write(this TextWriter writer, Stream source)
            {
            source.Seek(0, SeekOrigin.Begin);
            var buffer = new Byte[16];
            var offset = 0;
            for (;;)
                {
                var sz = source.Read(buffer, 0, buffer.Length);
                if (sz == 0) { break; }
                WriteInternal(writer, buffer, sz, offset);
                offset += sz;
                }
            }
        }
    }