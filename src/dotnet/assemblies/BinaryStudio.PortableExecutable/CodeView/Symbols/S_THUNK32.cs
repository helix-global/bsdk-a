using BinaryStudio.PortableExecutable.Win32;
using System;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    [CodeViewSymbol(DEBUG_SYMBOL_INDEX.S_THUNK32)]
    internal class S_THUNK32 : CodeViewSymbol
        {
        public override DEBUG_SYMBOL_INDEX Type { get { return DEBUG_SYMBOL_INDEX.S_THUNK32; }}
        public UInt32 pParent { get; }
        public UInt32 pEnd { get; }
        public UInt32 pNext { get; }
        public UInt32 off { get; }
        public UInt16 seg { get; }
        public UInt16 len { get; }
        public THUNK_ORDINAL ord { get; }
        public String Name { get; }
        public unsafe S_THUNK32(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            var H = (THUNKSYM32*)content;
            var I = (Byte*)content;
            var L = I + length;
            pParent = H->pParent;
            pEnd = H->pEnd;
            pNext = H->pNext;
            off = H->off;
            seg = H->seg;
            len = H->len;
            ord = H->ord;
            I += sizeof(THUNKSYM32);
            Name = ReadString(Section.Section.Encoding, ref I, Section.Section.IsLengthPrefixedString);
            if (I < L) {
                /** TODO: Add VARIANT decoding */
                }
            }
        }
    }