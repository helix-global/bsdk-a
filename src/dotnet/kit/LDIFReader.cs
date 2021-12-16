using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryStudio.Security.Cryptography.DataInterchangeFormat
    {
    internal class LDIFReader
        {
        #region M:Read(String):IEnumerable<LDIFEntry>
        public static IEnumerable<LDIFEntry> Read(String filename) {
            if (filename == null) { throw new ArgumentNullException(nameof(filename)); }
            if (String.IsNullOrWhiteSpace(filename)) { throw new ArgumentOutOfRangeException(nameof(filename)); }
            if (!File.Exists(filename)) { throw new ArgumentOutOfRangeException(filename); }
            return Read(new StreamReader(filename));
            }
        #endregion
        #region M:Read(Stream):IEnumerable<LDIFEntry>
        public static IEnumerable<LDIFEntry> Read(Stream stream) {
            if (stream == null) { throw new ArgumentNullException(nameof(stream)); }
            return Read(new StreamReader(stream));
            }
        #endregion
        #region M:Read(StreamReader):IEnumerable<LDIFEntry>
        public static IEnumerable<LDIFEntry> Read(StreamReader reader) {
            if (reader == null) { throw new ArgumentNullException(nameof(reader)); }
            var builder = new StringBuilder();
            LDIFEntry e = null;
            var lineno = 0;
            for (;;) {
                var line = reader.ReadLine();
                lineno++;
                if (line == null) {
                    if (e != null) {
                        e.Value = ToObject(builder);
                        yield return e;
                        }
                    break;
                    }
                if (line.Length > 0) {
                    if (line[0] == ' ') {
                        builder.Append(line.Trim());
                        }
                    else
                        {
                        var i = line.IndexOf(':');
                        if (i == -1) { throw new ArgumentOutOfRangeException(nameof(reader)); }
                        if (e != null) {
                            e.Value = ToObject(builder);
                            builder = new StringBuilder();
                            yield return e;
                            }
                        e = new LDIFEntry(line.Substring(0, i));
                        e.LineNumber = lineno;
                        builder.Append(line.Substring(i + 1).Trim());
                        }
                    }
                }
            }
        #endregion
        #region M:ToObject(StringBuilder):Object
        private static Object ToObject(StringBuilder source) {
            if (source[0] == ':') {
                source.Remove(0, 1);
                return Convert.FromBase64String(source.ToString());
                }
            return source.ToString();
            }
        #endregion
        }
    }