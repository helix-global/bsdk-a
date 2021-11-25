using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewFrameData
        {
        public UInt32 ulRvaStart { get; }
        public UInt32 cbBlock { get; }
        public UInt32 cbLocals { get; }
        public UInt32 cbParams { get; }
        public UInt32 cbStkMax { get; }
        public UInt32 frameFunc { get; }
        public UInt16 cbProlog { get; }
        public UInt16 cbSavedRegs { get; }
        public FRAMEDATA_FLAGS flags { get; }

        internal unsafe CodeViewFrameData(FRAMEDATA* source) {
            ulRvaStart = source->ulRvaStart;
            cbBlock = source->cbBlock;
            cbLocals = source->cbLocals;
            cbParams = source->cbParams;
            cbStkMax = source->cbStkMax;
            frameFunc = source->frameFunc;
            cbProlog = source->cbProlog;
            cbSavedRegs = source->cbSavedRegs;
            flags = (FRAMEDATA_FLAGS)(source->flags & 0x7U);
            }
        }
    }