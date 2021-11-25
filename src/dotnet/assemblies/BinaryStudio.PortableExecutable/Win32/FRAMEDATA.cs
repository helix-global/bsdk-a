using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct FRAMEDATA
        {
        public readonly UInt32 ulRvaStart;
        public readonly UInt32 cbBlock;
        public readonly UInt32 cbLocals;
        public readonly UInt32 cbParams;
        public readonly UInt32 cbStkMax;
        public readonly UInt32 frameFunc;
        public readonly UInt16 cbProlog;
        public readonly UInt16 cbSavedRegs;
        public readonly UInt32 flags;
        }
    }