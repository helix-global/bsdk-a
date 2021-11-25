using BinaryStudio.PortableExecutable.Win32;
using System;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    [CodeViewSymbol(DEBUG_SYMBOL_INDEX.S_LABEL32)]
    internal class S_LABEL32 : CodeViewSymbol
        {
        public DEBUG_TYPE_ENUM TypeIndex { get; }
        public override DEBUG_SYMBOL_INDEX Type { get { return DEBUG_SYMBOL_INDEX.S_LABEL32; }}
        public UInt16 Segment { get; }
        public new UInt32 Offset { get; }
        public String Value { get; }
        public CV_PFLAG Flags { get; }
        public unsafe S_LABEL32(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            var r = (LABELSYM32*)content;
            TypeIndex = r->TypeIndex;
            Segment = r->Segment;
            Offset = r->Offset;
            Flags = r->Flags;
            Value = ToString(Section.Section.Encoding, (Byte*)(r + 1), Section.Section.IsLengthPrefixedString);
            }
        }
    }