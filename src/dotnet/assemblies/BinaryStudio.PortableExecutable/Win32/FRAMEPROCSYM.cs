using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct FRAMEPROCSYM
        {
        public readonly UInt32 cbFrame;    // count of bytes of total frame of procedure
        public readonly UInt32 cbPad;      // count of bytes of padding in the frame
        public readonly UInt32 offPad;     // offset (relative to frame poniter) to where padding starts
        public readonly UInt32 cbSaveRegs; // count of bytes of callee save registers
        public readonly UInt32 offExHdlr;  // offset of exception handler
        public readonly UInt16 sectExHdlr; // section id of exception handler
        public readonly UInt32 Flags;
        }
    }