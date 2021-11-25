using System;
using System.Collections.Generic;
using System.Text;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewSourceFileNameStringTableRecord : CodeViewMajorRecord
        {
        public IDictionary<Int32, String> Files { get; }
        internal unsafe CodeViewSourceFileNameStringTableRecord(CV8_MAJOR_RECORD_TYPE type, Int32 length, Byte* content)
            : base(type, length, content)
            {
            Files = new Dictionary<Int32, String>();
            var li = content + length;
            var fi = content;
            content++;
            while (content < li) {
                Files[(Int32)(content - fi)] = ReadString(Encoding.UTF8, ref content);
                }
            }

        #region M:ReadString(Encoding,[Ref]Byte*):String
        private static unsafe String ReadString(Encoding encoding, ref Byte* source) {
            if (source == null) { return null; }
            var c = 0;
            for (;;++c) {
                if (source[c] == 0) {
                    break;
                    }
                }
            var r = new Byte[c];
            for (var i = 0;i < c;++i) {
                r[i] = source[i];
                }
            source += c + 1;
            return encoding.GetString(r);
            }
        #endregion
        }
    }