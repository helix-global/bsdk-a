using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    [CodeViewSymbol(DEBUG_SYMBOL_INDEX.S_UDT)]
    internal class S_UDT : CodeViewSymbol
        {
        public DEBUG_TYPE_ENUM TypeIndex { get; }
        public override DEBUG_SYMBOL_INDEX Type { get { return DEBUG_SYMBOL_INDEX.S_UDT; }}
        public String Value { get; }
        public unsafe S_UDT(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            var r = (UDTSYM*)content;
            TypeIndex = r->TypeIndex;
            Value = ToString(Section.Section.Encoding, (Byte*)(r + 1), Section.Section.IsLengthPrefixedString);
            }
        }
    }