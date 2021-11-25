using BinaryStudio.PortableExecutable.Win32;
using System;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    internal abstract class S_PROCSYM32 : CodeViewSymbol
        {
        public DEBUG_TYPE_ENUM TypeIndex { get; }
        public UInt16 Segment { get; }
        public new UInt32 Offset { get; }
        public String Value { get; }
        public CV_PFLAG Flags { get; }
        public UInt32 Parent { get; }
        public UInt32 End { get; }
        public UInt32 Next { get; }
        public UInt32 Length { get; }
        public UInt32 DbgStart { get; }
        public UInt32 DbgEnd { get; }

        protected unsafe S_PROCSYM32(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            var r = (PROCSYM32*)content;
            TypeIndex = r->TypeIndex;
            Segment = r->Segment;
            Offset = r->Offset;
            Flags = r->Flags;
            Parent = r->Parent;
            End = r->End;
            Next = r->Next;
            Length = r->Length;
            DbgStart = r->DbgStart;
            DbgEnd = r->DbgEnd;
            Value = ToString(Section.Section.Encoding, (Byte*)(r + 1), Section.Section.IsLengthPrefixedString);
            }
        }
    }