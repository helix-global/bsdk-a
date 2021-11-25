using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    [CodeViewSymbol(DEBUG_SYMBOL_INDEX.S_GPROC32_ID)]
    internal class S_GPROC32_ID : S_PROCSYM32
        {
        public override DEBUG_SYMBOL_INDEX Type { get { return DEBUG_SYMBOL_INDEX.S_GPROC32_ID; }}
        public S_GPROC32_ID(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            }
        }
    }