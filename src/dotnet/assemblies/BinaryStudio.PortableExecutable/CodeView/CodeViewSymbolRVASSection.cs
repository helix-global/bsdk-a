using BinaryStudio.PortableExecutable.Win32;
using System;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewSymbolRVASSection : CodeViewPrimarySSection
        {
        public override DEBUG_S Type { get { return DEBUG_S.DEBUG_S_COFF_SYMBOL_RVA; }}
        internal unsafe CodeViewSymbolRVASSection(CodeViewSection section, Int32 offset, Byte* content, Int32 length)
            : base(section, offset, content, length)
            {
            Console.Error.WriteLine($"{Type}");
            }
        }
    }