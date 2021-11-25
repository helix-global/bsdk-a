using System;
using System.Text;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    [CodeViewSymbol(DEBUG_SYMBOL_INDEX.S_UNAMESPACE)]
    public class S_UNAMESPACE : CodeViewSymbol
        {
        public override DEBUG_SYMBOL_INDEX Type { get { return DEBUG_SYMBOL_INDEX.S_UNAMESPACE; }}
        public String Value { get; }
        public S_UNAMESPACE(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            Value = Encoding.UTF8.GetString(Content).Trim('\0');
            }
        }
    }