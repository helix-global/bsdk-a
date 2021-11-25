using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Win32
    {
    #region E:IMAGE_SCN
    [Flags]
    public enum IMAGE_SCN : uint
        {
        IMAGE_SCN_TYPE_REG                  = 0x00000000,
        IMAGE_SCN_TYPE_DSECT                = 0x00000001,
        IMAGE_SCN_TYPE_NOLOAD               = 0x00000002,
        IMAGE_SCN_TYPE_GROUP                = 0x00000004,
        IMAGE_SCN_TYPE_NO_PAD               = 0x00000008,
        IMAGE_SCN_TYPE_COPY                 = 0x00000010,
        IMAGE_SCN_CNT_CODE                  = 0x00000020,
        IMAGE_SCN_CNT_INITIALIZED_DATA      = 0x00000040,
        IMAGE_SCN_CNT_UNINITIALIZED_DATA    = 0x00000080,
        IMAGE_SCN_LNK_OTHER                 = 0x00000100,
        IMAGE_SCN_LNK_INFO                  = 0x00000200,
        IMAGE_SCN_TYPE_OVER                 = 0x00000400,
        IMAGE_SCN_LNK_REMOVE                = 0x00000800,
        IMAGE_SCN_LNK_COMDAT                = 0x00001000,
        IMAGE_SCN_NO_DEFER_SPEC_EXC         = 0x00004000,
        IMAGE_SCN_GPREL                     = 0x00008000,
        IMAGE_SCN_MEM_SYSHEAP               = 0x00010000,
        IMAGE_SCN_MEM_PURGEABLE             = 0x00020000,
        IMAGE_SCN_MEM_16BIT                 = 0x00020000,
        IMAGE_SCN_MEM_LOCKED                = 0x00040000,
        IMAGE_SCN_MEM_PRELOAD               = 0x00080000,
        IMAGE_SCN_ALIGN_1BYTES              = 0x00100000,
        IMAGE_SCN_ALIGN_2BYTES              = 0x00200000,
        IMAGE_SCN_ALIGN_4BYTES              = 0x00300000,
        IMAGE_SCN_ALIGN_8BYTES              = 0x00400000,
        IMAGE_SCN_ALIGN_16BYTES             = 0x00500000,
        IMAGE_SCN_ALIGN_32BYTES             = 0x00600000,
        IMAGE_SCN_ALIGN_64BYTES             = 0x00700000,
        IMAGE_SCN_ALIGN_128BYTES            = 0x00800000,
        IMAGE_SCN_ALIGN_256BYTES            = 0x00900000,
        IMAGE_SCN_ALIGN_512BYTES            = 0x00A00000,
        IMAGE_SCN_ALIGN_1024BYTES           = 0x00B00000,
        IMAGE_SCN_ALIGN_2048BYTES           = 0x00C00000,
        IMAGE_SCN_ALIGN_4096BYTES           = 0x00D00000,
        IMAGE_SCN_ALIGN_8192BYTES           = 0x00E00000,
        IMAGE_SCN_ALIGN_MASK                = 0x00F00000,
        IMAGE_SCN_LNK_NRELOC_OVFL           = 0x01000000,
        IMAGE_SCN_MEM_DISCARDABLE           = 0x02000000,
        IMAGE_SCN_MEM_NOT_CACHED            = 0x04000000,
        IMAGE_SCN_MEM_NOT_PAGED             = 0x08000000,
        IMAGE_SCN_MEM_SHARED                = 0x10000000,
        IMAGE_SCN_MEM_EXECUTE               = 0x20000000,
        IMAGE_SCN_MEM_READ                  = 0x40000000,
        IMAGE_SCN_MEM_WRITE                 = 0x80000000,
        }
    #endregion
    #region E:IMAGE_FILE_MACHINE
    public enum IMAGE_FILE_MACHINE : ushort
        {
        IMAGE_FILE_MACHINE_UNKNOWN      = 0x0000,
        IMAGE_FILE_MACHINE_AM33         = 0x01d3,
        IMAGE_FILE_MACHINE_AMD64        = 0x8664,
        IMAGE_FILE_MACHINE_ARM          = 0x01c0,
        IMAGE_FILE_MACHINE_ARM64        = 0xaa64,
        IMAGE_FILE_MACHINE_ARMNT        = 0x01c4,
        IMAGE_FILE_MACHINE_EBC          = 0x0ebc,
        IMAGE_FILE_MACHINE_I386         = 0x014c,
        IMAGE_FILE_MACHINE_IA64         = 0x0200,
        IMAGE_FILE_MACHINE_M32R         = 0x9041,
        IMAGE_FILE_MACHINE_MIPS16       = 0x0266,
        IMAGE_FILE_MACHINE_MIPSFPU      = 0x0366,
        IMAGE_FILE_MACHINE_MIPSFPU16    = 0x0466,
        IMAGE_FILE_MACHINE_POWERPC      = 0x01f0,
        IMAGE_FILE_MACHINE_POWERPCFP    = 0x01f1,
        IMAGE_FILE_MACHINE_R3000        = 0x0162,
        IMAGE_FILE_MACHINE_R10000       = 0x0168,
        IMAGE_FILE_MACHINE_R4000        = 0x0166,
        IMAGE_FILE_MACHINE_RISCV32      = 0x5032,
        IMAGE_FILE_MACHINE_RISCV64      = 0x5064,
        IMAGE_FILE_MACHINE_RISCV128     = 0x5128,
        IMAGE_FILE_MACHINE_SH3          = 0x01a2,
        IMAGE_FILE_MACHINE_SH3DSP       = 0x01a3,
        IMAGE_FILE_MACHINE_SH4          = 0x01a6,
        IMAGE_FILE_MACHINE_SH5          = 0x01a8,
        IMAGE_FILE_MACHINE_THUMB        = 0x01c2,
        IMAGE_FILE_MACHINE_ALPHA        = 0x0184,
        IMAGE_FILE_MACHINE_WCEMIPSV2    = 0x0169,
        IMAGE_FILE_MACHINE_ALPHA64      = 0x0284,
        IMAGE_FILE_MACHINE_TRICORE      = 0x0520,
        IMAGE_FILE_MACHINE_CEE          = 0xC0EE,
        IMAGE_FILE_MACHINE_CEF          = 0x0CEF
        }
    #endregion
    #region E:IMAGE_FILE_CHARACTERISTIC
    [Flags]
    public enum IMAGE_FILE_CHARACTERISTIC : ushort
        {
        IMAGE_FILE_NONE                     = 0x0000,
        IMAGE_FILE_RELOCS_STRIPPED          = 0x0001,
        IMAGE_FILE_EXECUTABLE_IMAGE         = 0x0002,
        IMAGE_FILE_LINE_NUMS_STRIPPED       = 0x0004,
        IMAGE_FILE_LOCAL_SYMS_STRIPPED      = 0x0008,
        IMAGE_FILE_AGGRESSIVE_WS_TRIM       = 0x0010,
        IMAGE_FILE_LARGE_ADDRESS_AWARE      = 0x0020,
        IMAGE_FILE_RESERVED                 = 0x0040,
        IMAGE_FILE_BYTES_REVERSED_LO        = 0x0080,
        IMAGE_FILE_32BIT_MACHINE            = 0x0100,
        IMAGE_FILE_DEBUG_STRIPPED           = 0x0200,
        IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP  = 0x0400,
        IMAGE_FILE_NET_RUN_FROM_SWAP        = 0x0800,
        IMAGE_FILE_SYSTEM                   = 0x1000,
        IMAGE_FILE_DLL                      = 0x2000,
        IMAGE_FILE_UP_SYSTEM_ONLY           = 0x4000,
        IMAGE_FILE_BYTES_REVERSED_HI        = 0x8000
        }
    #endregion
    #region T:IMAGE_FILE_HEADER
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IMAGE_FILE_HEADER
        {
        public readonly IMAGE_FILE_MACHINE Machine;
        public readonly UInt16 NumberOfSections;
        public readonly UInt32 TimeDateStamp;
        public readonly UInt32 PointerToSymbolTable;
        public readonly UInt32 NumberOfSymbols;
        public readonly UInt16 SizeOfOptionalHeader;
        public readonly IMAGE_FILE_CHARACTERISTIC Characteristics;
        }
    #endregion
    #region T:IMAGE_DATA_DIRECTORY
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [DebuggerDisplay("{VirtualAddress}:{Size}")]
    public struct IMAGE_DATA_DIRECTORY
        {
        public readonly UInt32 VirtualAddress;
        public readonly UInt32 Size;
        }
    #endregion
    #region T:IMAGE_SECTION_HEADER
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    [DebuggerDisplay("{ToString()}")]
    public struct IMAGE_SECTION_HEADER
        {
        [DebuggerDisplay("{ToString()}")] public unsafe fixed Byte Name[8];
        public readonly UInt32 VirtualSize;
        public readonly UInt32 VirtualAddress;
        public readonly UInt32 SizeOfRawData;
        public readonly UInt32 PointerToRawData;
        public readonly UInt32 PointerToRelocations;
        public readonly UInt32 PointerToLineNumbers;
        public readonly UInt16 NumberOfRelocations;
        public readonly UInt16 NumberOfLineNumbers;
        public readonly IMAGE_SCN Characteristics;

        public override unsafe String ToString() {
            var r = new StringBuilder(8);
            fixed (Byte* bytes = Name) {
                for (var i = 0; i < 8; i++) {
                    if (bytes[i] == 0) { break; }
                    r.Append((Char)bytes[i]);
                    }
                }
            return r.ToString();
            }
        }
    #endregion
    #region E:IMAGE_RESOURCE_LEVEL
    internal enum IMAGE_RESOURCE_LEVEL
        {
        LEVEL_TYPE,
        LEVEL_NAME,
        LEVEL_LANGUAGE
        }
    #endregion
    #region T:IMAGE_DIRECTORY_ENTRY_RESOURCE
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct IMAGE_DIRECTORY_ENTRY_RESOURCE
        {
        [FieldOffset(0)] public readonly UInt32 NameOffset;
        [FieldOffset(0)] public readonly UInt32 IntegerId;
        [FieldOffset(4)] public readonly UInt32 DataEntryOffset;
        }
    #endregion
    #region E:IMAGE_RESOURCE_TYPE
    internal enum IMAGE_RESOURCE_TYPE
        {
        RT_CURSOR = 1,
        RT_BITMAP,
        RT_ICON,
        RT_MENU,
        RT_DIALOG,
        RT_STRING,
        RT_FONTDIR,
        RT_FONT,
        RT_ACCELERATOR,
        RT_RCDATA,
        RT_MESSAGETABLE,
        RT_GROUP_CURSOR,
        RT_GROUP_ICON = 14,
        RT_VERSION = 16,
        RT_DLGINCLUDE,
        RT_PLUGPLAY = 19,
        RT_VXD,
        RT_ANICURSOR,
        RT_ANIICON,
        RT_HTML,
        RT_MANIFEST
        }
    #endregion

    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    public struct ANON_OBJECT_HEADER
        {
        public readonly UInt16 Sig1;            // Must be IMAGE_FILE_MACHINE_UNKNOWN
        public readonly UInt16 Sig2;            // Must be 0xffff
        public readonly UInt16 Version;         // >= 1 (implies the CLSID field is present)
        public readonly IMAGE_FILE_MACHINE Machine;
        public readonly UInt32 TimeDateStamp;
        public Guid   ClassID;         // Used to invoke CoCreateInstance
        public readonly UInt32 SizeOfData;      // Size of data that follows the header
        }

    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    internal struct ANON_OBJECT_HEADER_V2
        {
        public readonly UInt16 Sig1;            // Must be IMAGE_FILE_MACHINE_UNKNOWN
        public readonly UInt16 Sig2;            // Must be 0xffff
        public readonly UInt16 Version;         // >= 2 (implies the Flags field is present - otherwise V1)
        public readonly IMAGE_FILE_MACHINE Machine;
        public readonly UInt32 TimeDateStamp;
        public readonly Guid   ClassID;         // Used to invoke CoCreateInstance
        public readonly UInt32 SizeOfData;      // Size of data that follows the header
        public readonly UInt32 Flags;           // 0x1 -> contains metadata
        public readonly UInt32 MetaDataSize;    // Size of CLR metadata
        public readonly UInt32 MetaDataOffset;  // Offset of CLR metadata
        }

    [StructLayout(LayoutKind.Explicit,Pack = 1,Size = 52)]
    internal struct ANON_OBJECT_HEADER_BIGOBJ_V1
        {
        [FieldOffset( 0)] public readonly UInt16 Sig1;            // Must be IMAGE_FILE_MACHINE_UNKNOWN
        [FieldOffset( 2)] public readonly UInt16 Sig2;            // Must be 0xffff
        [FieldOffset( 4)] public readonly UInt16 Version;         // >= 2 (implies the Flags field is present)
        [FieldOffset( 6)] public readonly IMAGE_FILE_MACHINE Machine;         // Actual machine - IMAGE_FILE_MACHINE_xxx
        [FieldOffset( 8)] public readonly UInt32 TimeDateStamp;
        [FieldOffset(12)] public readonly Guid   ClassID;
        [FieldOffset(28)] public readonly UInt32 SizeOfData;      // Size of data that follows the header
        [FieldOffset(32)] public readonly UInt16 Flags;           // 0x1 -> contains metadata
        [FieldOffset(34)] public readonly UInt16 NumberOfSections; // extended from WORD
        [FieldOffset(36)] public readonly UInt32 NumberOfSymbols;
        [FieldOffset(40)] public readonly UInt32 PointerToSymbolTable;
        }

    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    internal struct ANON_OBJECT_HEADER_BIGOBJ_V2
        {
        /* same as ANON_OBJECT_HEADER_V2 */
        public readonly UInt16 Sig1;            // Must be IMAGE_FILE_MACHINE_UNKNOWN
        public readonly UInt16 Sig2;            // Must be 0xffff
        public readonly UInt16 Version;         // >= 2 (implies the Flags field is present)
        public readonly IMAGE_FILE_MACHINE Machine;         // Actual machine - IMAGE_FILE_MACHINE_xxx
        public readonly UInt32 TimeDateStamp;
        public readonly Guid   ClassID;         // {D1BAA1C7-BAEE-4ba9-AF20-FAF66AA4DCB8}
        public readonly UInt32 SizeOfData;      // Size of data that follows the header
        public readonly UInt32 Flags;           // 0x1 -> contains metadata
        public readonly UInt32 MetaDataSize;    // Size of CLR metadata
        public readonly UInt32 MetaDataOffset;  // Offset of CLR metadata

        /* bigobj specifics */
        public readonly UInt32 NumberOfSections; // extended from WORD
        public readonly UInt32 PointerToSymbolTable;
        public readonly UInt32 NumberOfSymbols;
        }
    }