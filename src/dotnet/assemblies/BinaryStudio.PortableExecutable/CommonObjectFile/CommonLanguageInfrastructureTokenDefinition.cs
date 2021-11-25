using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable
    {
    public class CommonLanguageInfrastructureTokenDefinition : ISymbol
        {
        public Int32 SymbolTableIndex { get; }
        internal unsafe CommonLanguageInfrastructureTokenDefinition(IMAGE_AUX_SYMBOL_TOKEN_DEF* source)
            {
            SymbolTableIndex = source->SymbolTableIndex;
            }
        }
    }