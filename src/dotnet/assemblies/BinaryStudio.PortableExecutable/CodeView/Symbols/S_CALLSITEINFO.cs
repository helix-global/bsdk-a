using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    [CodeViewSymbol(DEBUG_SYMBOL_INDEX.S_CALLSITEINFO)]
    internal class S_CALLSITEINFO : CodeViewSymbol
        {
        public override DEBUG_SYMBOL_INDEX Type { get { return DEBUG_SYMBOL_INDEX.S_CALLSITEINFO; }}
        public UInt32 off { get; }
        public UInt16 sect { get; }
        public DEBUG_TYPE_ENUM TypeIndex { get; }
        public unsafe S_CALLSITEINFO(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            var r = (CALLSITEINFO*)content;
            off = r->off;
            sect = r->sect;
            TypeIndex = r->TypeIndex;
            }
        }
    }