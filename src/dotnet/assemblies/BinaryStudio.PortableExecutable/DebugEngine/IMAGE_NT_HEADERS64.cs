using System;
using System.Runtime.InteropServices;
using BinaryStudio.PortableExecutable.Win32;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct IMAGE_NT_HEADERS64
        {
        public UInt32 Signature;
        public IMAGE_FILE_HEADER FileHeader;
        public IMAGE_OPTIONAL_HEADER64 OptionalHeader;
        }
    }