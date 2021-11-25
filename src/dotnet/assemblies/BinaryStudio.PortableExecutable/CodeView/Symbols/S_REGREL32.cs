using BinaryStudio.PortableExecutable.Win32;
using System;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    [CodeViewSymbol(DEBUG_SYMBOL_INDEX.S_REGREL32)]
    internal class S_REGREL32 : CodeViewSymbol
        {
        public override DEBUG_SYMBOL_INDEX Type { get { return DEBUG_SYMBOL_INDEX.S_REGREL32; }}
        public DEBUG_TYPE_ENUM TypeIndex { get; }
        public new UInt32 Offset { get; }
        public String Value { get; }
        public Object Registry { get; }
        public unsafe S_REGREL32(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            var r = (REGREL32*)content;
            TypeIndex = r->TypeIndex;
            Offset = r->off;
            Value = ToString(Section.Section.Encoding, (Byte*)(r + 1), Section.Section.IsLengthPrefixedString);
            Registry = section.DecodeRegister(r->reg);
            }
        }
    }