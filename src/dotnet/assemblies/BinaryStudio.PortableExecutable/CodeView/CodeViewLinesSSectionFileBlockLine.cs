using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewLinesSSectionFileBlockLine
        {
        public UInt32 Offset { get; }
        public UInt32 Value { get; }
        public Boolean IsSpecialLine { get; }
        public Int32? LineStartNumber { get; }
        public Int32 LineEndDelta { get; }
        internal unsafe CodeViewLinesSSectionFileBlockLine(CV_Line_t* value, UInt32 offset) {
            Offset = value->offset + offset;
            Value = value->Value;
            var linestart = value->Value & 0xFFFFFF;
            IsSpecialLine = (linestart == 0xfeefee) || (linestart == 0xf00f00);
            if (!IsSpecialLine) {
                LineStartNumber = (Int32)linestart;
                LineEndDelta = (Int32)((value->Value >> 24) & 0x7F);
                }
            }
        }
    }