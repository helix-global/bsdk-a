using System;

namespace BinaryStudio.PortableExecutable.Win32
    {
    /// <summary>
    /// [SH3][SH4][SHM (SH Media)]
    /// </summary>
    public enum IMAGE_REL_SH3 : short
        {
        IMAGE_REL_SH3_ABSOLUTE          = 0x0000,   /* The relocation is ignored. */
        IMAGE_REL_SH3_DIRECT16          = 0x0001,   /* A reference to the 16-bit location that contains the VA of the target symbol. */
        IMAGE_REL_SH3_DIRECT32          = 0x0002,   /* The 32-bit VA of the target symbol. */
        IMAGE_REL_SH3_DIRECT8           = 0x0003,   /* A reference to the 8-bit location that contains the VA of the target symbol. */
        IMAGE_REL_SH3_DIRECT8_WORD      = 0x0004,   /* A reference to the 8-bit instruction that contains the effective 16-bit VA of the target symbol. */
        IMAGE_REL_SH3_DIRECT8_LONG      = 0x0005,   /* A reference to the 8-bit instruction that contains the effective 32-bit VA of the target symbol. */
        IMAGE_REL_SH3_DIRECT4           = 0x0006,   /* A reference to the 8-bit location whose low 4 bits contain the VA of the target symbol. */
        IMAGE_REL_SH3_DIRECT4_WORD      = 0x0007,   /* A reference to the 8-bit instruction whose low 4 bits contain the effective 16-bit VA of the target symbol. */
        IMAGE_REL_SH3_DIRECT4_LONG      = 0x0008,   /* A reference to the 8-bit instruction whose low 4 bits contain the effective 32-bit VA of the target symbol. */
        IMAGE_REL_SH3_PCREL8_WORD       = 0x0009,   /* A reference to the 8-bit instruction that contains the effective 16-bit relative offset of the target symbol. */
        IMAGE_REL_SH3_PCREL8_LONG       = 0x000A,   /* A reference to the 8-bit instruction that contains the effective 32-bit relative offset of the target symbol. */
        IMAGE_REL_SH3_PCREL12_WORD      = 0x000B,   /* A reference to the 16-bit instruction whose low 12 bits contain the effective 16-bit relative offset of the target symbol. */
        IMAGE_REL_SH3_STARTOF_SECTION   = 0x000C,   /* A reference to a 32-bit location that is the VA of the section that contains the target symbol. */
        IMAGE_REL_SH3_SIZEOF_SECTION    = 0x000D,   /* A reference to the 32-bit location that is the size of the section that contains the target symbol. */
        IMAGE_REL_SH3_SECTION           = 0x000E,   /* The 16-bit section index of the section that contains the target. This is used to support debugging information. */
        IMAGE_REL_SH3_SECREL            = 0x000F,   /* The 32-bit offset of the target from the beginning of its section. This is used to support debugging information and static thread local storage. */
        IMAGE_REL_SH3_DIRECT32_NB       = 0x0010,   /* The 32-bit RVA of the target symbol. */
        IMAGE_REL_SH3_GPREL4_LONG       = 0x0011,   /* GP relative. */
        IMAGE_REL_SH3_TOKEN             = 0x0012,   /* CLR token. */
        IMAGE_REL_SHM_PCRELPT           = 0x0013,   /* The offset from the current instruction in longwords. If the NOMODE bit is not set, insert the inverse of the low bit at bit 32 to select PTA or PTB. */
        IMAGE_REL_SHM_REFLO             = 0x0014,   /* The low 16 bits of the 32-bit address. */
        IMAGE_REL_SHM_REFHALF           = 0x0015,   /* The high 16 bits of the 32-bit address. */
        IMAGE_REL_SHM_RELLO             = 0x0016,   /* The low 16 bits of the relative address. */
        IMAGE_REL_SHM_RELHALF           = 0x0017,   /* The high 16 bits of the relative address. */
        IMAGE_REL_SHM_PAIR              = 0x0018,   /* The relocation is valid only when it immediately follows a REFHALF, RELHALF, or RELLO relocation. The SymbolTableIndex field of the relocation contains a displacement and not an index into the symbol table. */
        IMAGE_REL_SHM_NOMODE            = unchecked((Int16)0x8000)    /* The relocation ignores section mode. */
        }
    }