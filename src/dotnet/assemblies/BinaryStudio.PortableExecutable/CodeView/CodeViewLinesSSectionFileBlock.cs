using System;
using System.Collections.Generic;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewLinesSSectionFileBlock
        {
        public UInt32 Identifier { get; }
        public IList<CodeViewLinesSSectionFileBlockLine> Lines { get; }
        internal CodeViewLinesSSectionFileBlock(UInt32 identifier)
            {
            Identifier = identifier;
            Lines = new List<CodeViewLinesSSectionFileBlockLine>();
            }

        internal unsafe void Add(CV_Line_t* value, UInt32 offset)
            {
            Lines.Add(new CodeViewLinesSSectionFileBlockLine(value, offset));
            }

        internal unsafe void Add(CV_Column_t* value)
            {
            }
        }
    }