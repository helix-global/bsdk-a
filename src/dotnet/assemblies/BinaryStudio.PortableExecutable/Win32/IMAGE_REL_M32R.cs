namespace BinaryStudio.PortableExecutable.Win32
    {
    /// <summary>
    /// [Mitsubishi M32R]
    /// </summary>
    public enum IMAGE_REL_M32R : short
        {
        IMAGE_REL_M32R_ABSOLUTE         = 0x0000,   /* The relocation is ignored. */
        IMAGE_REL_M32R_ADDR32           = 0x0001,   /* The target’s 32-bit VA. */
        IMAGE_REL_M32R_ADDR32NB         = 0x0002,   /* The target’s 32-bit RVA. */
        IMAGE_REL_M32R_ADDR24           = 0x0003,   /* The target’s 24-bit VA. */
        IMAGE_REL_M32R_GPREL16          = 0x0004,   /* The target’s 16-bit offset from the GP register. */
        IMAGE_REL_M32R_PCREL24          = 0x0005,   /* The target’s 24-bit offset from the program counter (PC), shifted left by 2 bits and sign-extended. */
        IMAGE_REL_M32R_PCREL16          = 0x0006,   /* The target’s 16-bit offset from the PC, shifted left by 2 bits and sign-extended. */
        IMAGE_REL_M32R_PCREL8           = 0x0007,   /* The target’s 8-bit offset from the PC, shifted left by 2 bits and sign-extended. */
        IMAGE_REL_M32R_REFHALF          = 0x0008,   /* The 16 MSBs of the target VA. */
        IMAGE_REL_M32R_REFHI            = 0x0009,   /* The 16 MSBs of the target VA, adjusted for LSB sign extension. This is used for the first instruction in a two-instruction sequence that loads a full 32-bit address. This relocation must be immediately followed by a PAIR relocation whose SymbolTableIndex contains a signed 16-bit displacement that is added to the upper 16 bits that are taken from the location that is being relocated. */
        IMAGE_REL_M32R_REFLO            = 0x000A,   /* The 16 LSBs of the target VA. */
        IMAGE_REL_M32R_PAIR             = 0x000B,   /* The relocation must follow the REFHI relocation. Its SymbolTableIndex contains a displacement and not an index into the symbol table. */
        IMAGE_REL_M32R_SECTION          = 0x000C,   /* The 16-bit section index of the section that contains the target. This is used to support debugging information. */
        IMAGE_REL_M32R_SECREL           = 0x000D,   /* The 32-bit offset of the target from the beginning of its section. This is used to support debugging information and static thread local storage. */
        IMAGE_REL_M32R_TOKEN            = 0x000E    /* The CLR token. */
        }
    }