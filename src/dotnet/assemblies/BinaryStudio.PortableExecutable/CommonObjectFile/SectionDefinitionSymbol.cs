using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable
    {
    public class SectionDefinitionSymbol : ISymbol
        {
        public Int32  Length { get; }
        public Int16  NumberOfRelocations { get; }
        public Int16  NumberOfLineNumbers { get; }
        public UInt32 CheckSum { get; }
        public Int32  Number { get; }
        public Byte   Selection { get; }

        internal unsafe SectionDefinitionSymbol(IMAGE_AUX_SYMBOL_SECTION_DEF* source)
            {
            Length = source->Length;
            NumberOfLineNumbers = source->NumberOfLineNumbers;
            NumberOfRelocations = source->NumberOfRelocations;
            CheckSum = source->CheckSum;
            Selection = source->Selection;
            Number = unchecked((Int32)(((UInt32)source->HighNumber) << 16) | source->LowNumber);
            }
        }
    }