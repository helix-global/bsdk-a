using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 18)]
    public struct IMAGE_AUX_SYMBOL_SECTION_DEF
        {
        [FieldOffset( 0)] public readonly Int32  Length;              // section length
        [FieldOffset( 4)] public readonly Int16  NumberOfRelocations; // number of relocation entries
        [FieldOffset( 6)] public readonly Int16  NumberOfLineNumbers; // number of line numbers
        [FieldOffset( 8)] public readonly UInt32 CheckSum;            // checksum for communal
        [FieldOffset(12)] public readonly UInt16 LowNumber;              // section number to associate with
        [FieldOffset(14)] public readonly Byte   Selection;           // communal selection type
        [FieldOffset(16)] public readonly UInt16 HighNumber;          // high bits of the section number
        }
    }