using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    [CodeViewSymbol(DEBUG_SYMBOL_INDEX.S_OBJNAME)]
    internal class S_OBJNAME : CodeViewSymbol
        {
        public override DEBUG_SYMBOL_INDEX Type { get { return DEBUG_SYMBOL_INDEX.S_OBJNAME; }}
        public String Value { get; }
        public UInt32 Signature { get; }
        public unsafe S_OBJNAME(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            var r = (OBJNAMESYM*)content;
            Signature = r->Signature;
            Value = ToString(Section.Section.Encoding, (Byte*)(r + 1), Section.Section.IsLengthPrefixedString);
            }
        }
    }