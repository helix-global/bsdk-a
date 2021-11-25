using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    [CodeViewSymbol(DEBUG_SYMBOL_INDEX.S_HEAPALLOCSITE)]
    internal class S_HEAPALLOCSITE : CodeViewSymbol
        {
        public override DEBUG_SYMBOL_INDEX Type { get { return DEBUG_SYMBOL_INDEX.S_HEAPALLOCSITE; }}
        public UInt32 off { get; }
        public UInt16 sect { get; }
        public UInt16 cbInstr { get; }
        public DEBUG_TYPE_ENUM TypeIndex { get; }
        public unsafe S_HEAPALLOCSITE(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            var r = (HEAPALLOCSITE*)content;
            off = r->off;
            sect = r->sect;
            cbInstr = r->cbInstr;
            TypeIndex = r->TypeIndex;
            }
        }
    }