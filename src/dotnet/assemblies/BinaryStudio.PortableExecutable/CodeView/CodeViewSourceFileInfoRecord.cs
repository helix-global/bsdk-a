using System;
using System.Collections.Generic;
using System.Linq;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewSourceFileInfoRecord : CodeViewMajorRecord
        {
        public IList<CodeViewSourceFileInfoRecordItem> Files { get; }
        internal unsafe CodeViewSourceFileInfoRecord(CV8_MAJOR_RECORD_TYPE type, Int32 length, Byte* content)
            : base(type, length, content)
            {
            Files = new List<CodeViewSourceFileInfoRecordItem>();
            var li = content + length;
            var fi = content;
            for (;content < li;) {
                var i = (CV8_FILE_INFO_HEADER*)content;
                var sz = sizeof(CV8_FILE_INFO_HEADER) + i->HashSize;
                Files.Add(new CodeViewSourceFileInfoRecordItem(
                    i->FileNameOffset,
                    GetBytes(content + sizeof(CV8_FILE_INFO_HEADER), i->HashSize)));
                content += (Int32)Math.Ceiling(sz/4.0)*4;
                }
            Files = Files.ToArray();
            }

        #region M:GetBytes(Byte*,Int64):Byte[]
        private static unsafe Byte[] GetBytes(Byte* source, Int64 size)
            {
            if (source == null) { return null; }
            var r = new Byte[size];
            for (var i = 0;i < size;++i) {
                r[i] = source[i];
                }
            source += size;
            return r;
            }
        #endregion
        }
    }