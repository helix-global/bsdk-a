using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CV8_FILE_INFO_HEADER
        {
        public readonly Int32 FileNameOffset;
        public readonly Byte  HashSize;
        public readonly CHKSUM_TYPE HashType;
        }
    }