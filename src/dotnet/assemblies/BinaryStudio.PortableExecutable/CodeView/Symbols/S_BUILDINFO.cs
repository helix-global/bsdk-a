using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    [CodeViewSymbol(DEBUG_SYMBOL_INDEX.S_BUILDINFO)]
    internal class S_BUILDINFO : CodeViewSymbol
        {
        public DEBUG_TYPE_ENUM TypeIndex { get; }
        public override DEBUG_SYMBOL_INDEX Type { get { return DEBUG_SYMBOL_INDEX.S_BUILDINFO; }}
        public unsafe S_BUILDINFO(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            var r = (UDTSYM*)content;
            TypeIndex = r->TypeIndex;
            }
        }
    }