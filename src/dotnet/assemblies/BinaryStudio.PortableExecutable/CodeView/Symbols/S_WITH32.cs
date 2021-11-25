using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    [CodeViewSymbol(DEBUG_SYMBOL_INDEX.S_WITH32)]
    internal class S_WITH32 : S_BLOCKSYM32
        {
        public override DEBUG_SYMBOL_INDEX Type { get { return DEBUG_SYMBOL_INDEX.S_WITH32; }}
        public S_WITH32(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            }
        }
    }