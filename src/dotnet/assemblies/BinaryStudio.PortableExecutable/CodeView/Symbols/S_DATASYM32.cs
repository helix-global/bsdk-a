using BinaryStudio.PortableExecutable.Win32;
using System;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public abstract class S_DATASYM32 : CodeViewSymbol
        {
        public DEBUG_TYPE_ENUM TypeIndex { get; }
        public UInt16 Segment { get; }
        public new UInt32 Offset { get; }
        public String Value { get; }
        protected unsafe S_DATASYM32(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            var r = (DATASYM32*)content;
            TypeIndex = r->TypeIndex;
            Segment = r->Segment;
            Offset = r->Offset;
            Value = ToString(Section.Section.Encoding, (Byte*)(r + 1), Section.Section.IsLengthPrefixedString);
            }
        }
    }