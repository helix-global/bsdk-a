using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    internal abstract class S_BLOCKSYM32 : CodeViewSymbol
        {
        public UInt16 Segment { get; }
        public new UInt32 Offset { get; }
        public String Value { get; }
        public UInt32 pParent { get; }
        public UInt32 pEnd { get; }
        public UInt32 len { get; }
        protected unsafe S_BLOCKSYM32(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            var r = (BLOCKSYM32*)content;
            len = r->len;
            pParent = r->pParent;
            pEnd = r->pEnd;
            Segment = r->seg;
            Offset = r->off;
            Value = ToString(Section.Section.Encoding, (Byte*)(r + 1), Section.Section.IsLengthPrefixedString);
            }
        }
    }