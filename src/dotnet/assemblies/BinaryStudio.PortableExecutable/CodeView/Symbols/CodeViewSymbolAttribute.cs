using BinaryStudio.PortableExecutable.Win32;
using System;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewSymbolAttribute : Attribute
        {
        public DEBUG_SYMBOL_INDEX Key { get; }
        public CodeViewSymbolAttribute(DEBUG_SYMBOL_INDEX key)
            {
            Key = key;
            }
        }
    }