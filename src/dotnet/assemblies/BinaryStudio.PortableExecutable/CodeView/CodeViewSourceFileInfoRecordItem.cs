using System;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewSourceFileInfoRecordItem
        {
        public Byte[] HashValue { get; }
        public Int32 FileNameOffset { get; }
        public String FileName { get;internal set; }
        internal CodeViewSourceFileInfoRecordItem(Int32 offset, Byte[] hashvalue)
            {
            FileNameOffset = offset;
            HashValue = hashvalue;
            }
        }
    }