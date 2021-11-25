using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    [CodeViewSymbol(DEBUG_SYMBOL_INDEX.S_FRAMEPROC)]
    internal class S_FRAMEPROC : CodeViewSymbol
        {
        public override DEBUG_SYMBOL_INDEX Type { get { return DEBUG_SYMBOL_INDEX.S_FRAMEPROC; }}
        public UInt32 cbFrame { get; }
        public UInt32 cbPad { get; }
        public UInt32 offPad { get; }
        public UInt32 cbSaveRegs { get; }
        public UInt32 offExHdlr { get; }
        public UInt16 sectExHdlr { get; }
        public FRAMEPROCSYM_FLAGS Flags { get; }
        private Byte encodedLocalBasePointer { get; }
        private Byte encodedParamBasePointer { get; }
        public Object LocalBasePointer { get { return FramePointerRegX86[encodedLocalBasePointer]; }}
        public Object ParamBasePointer { get { return FramePointerRegX86[encodedParamBasePointer]; }}

        private static readonly Object[] FramePointerRegX86 =
            {
            CV_REG.CV_REG_NONE,
            CV_REG.CV_ALLREG_VFRAME,
            CV_REG.CV_REG_EBP,
            CV_REG.CV_REG_EBX
            };

        private static readonly Object[] FramePointerRegX64 =
            {
            CV_REG.CV_REG_NONE,
            CV_AMD64.CV_AMD64_RSP,
            CV_AMD64.CV_AMD64_RBP,
            CV_AMD64.CV_AMD64_R13
            };

        private static readonly Object[] FramePointerRegArm =
            {
            CV_REG.CV_REG_NONE,
            CV_ARM.CV_ARM_SP,
            CV_ARM.CV_ARM_R7,
            CV_REG.CV_REG_NONE
            };

        public unsafe S_FRAMEPROC(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            var r = (FRAMEPROCSYM*)content;
            cbFrame = r->cbFrame;
            cbPad = r->cbPad;
            offPad = r->offPad;
            cbSaveRegs = r->cbSaveRegs;
            offExHdlr = r->offExHdlr;
            sectExHdlr = r->sectExHdlr;
            Flags = (FRAMEPROCSYM_FLAGS)(r->Flags & 0x7C3FFF);
            encodedLocalBasePointer = (Byte)((r->Flags >> 14) & 0x03);
            encodedParamBasePointer = (Byte)((r->Flags >> 16) & 0x03);
            switch (section.Section.CommonObjectFile.Machine)
                {

                }
            }
        }
    }