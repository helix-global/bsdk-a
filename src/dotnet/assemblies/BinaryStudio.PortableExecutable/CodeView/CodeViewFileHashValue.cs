using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewFileHashValue
        {
        public Byte[] HashValue { get; }
        public Int32 FileNameOffset { get; }
        public String FileName { get;internal set; }
        public CHKSUM_TYPE HashType { get; }
        internal CodeViewFileHashValue(Int32 offset, Byte[] hashvalue, CHKSUM_TYPE type)
            {
            FileNameOffset = offset;
            HashValue = hashvalue;
            HashType = type;
            }
        }
    }