using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct EXCEPTION_RECORD64
        {
        public UInt32 ExceptionCode;
        public UInt32 ExceptionFlags;
        public Int64 ExceptionRecord;
        public Int64 ExceptionAddress;
        public UInt32 NumberParameters;
        public UInt32 __unusedAlignment;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)] public Int64[] ExceptionInformation;
        }
    }