namespace BinaryStudio.PortableExecutable.Win32
    {
    /// <summary>
    /// [MIPS]
    /// </summary>
    public enum IMAGE_REL_MIPS : short
        {
        IMAGE_REL_MIPS_ABSOLUTE         = 0x0000,   /* The relocation is ignored. */
        IMAGE_REL_MIPS_REFHALF          = 0x0001,   /* The high 16 bits of the target’s 32-bit VA. */
        IMAGE_REL_MIPS_REFWORD          = 0x0002,   /* The target’s 32-bit VA. */
        IMAGE_REL_MIPS_JMPADDR          = 0x0003,   /* The low 26 bits of the target’s VA. This supports the MIPS J and JAL instructions. */
        IMAGE_REL_MIPS_REFHI            = 0x0004,   /* The high 16 bits of the target’s 32-bit VA. This is used for the first instruction in a two-instruction sequence that loads a full address. This relocation must be immediately followed by a PAIR relocation whose SymbolTableIndex contains a signed 16-bit displacement that is added to the upper 16 bits that are taken from the location that is being relocated. */
        IMAGE_REL_MIPS_REFLO            = 0x0005,   /* The low 16 bits of the target’s VA. */
        IMAGE_REL_MIPS_GPREL            = 0x0006,   /* A 16-bit signed displacement of the target relative to the GP register. */
        IMAGE_REL_MIPS_LITERAL          = 0x0007,   /* The same as IMAGE_REL_MIPS_GPREL. */
        IMAGE_REL_MIPS_SECTION          = 0x000A,   /* The 16-bit section index of the section contains the target. This is used to support debugging information. */
        IMAGE_REL_MIPS_SECREL           = 0x000B,   /* The 32-bit offset of the target from the beginning of its section. This is used to support debugging information and static thread local storage. */
        IMAGE_REL_MIPS_SECRELLO         = 0x000C,   /* The low 16 bits of the 32-bit offset of the target from the beginning of its section. */
        IMAGE_REL_MIPS_SECRELHI         = 0x000D,   /* The high 16 bits of the 32-bit offset of the target from the beginning of its section. An IMAGE_REL_MIPS_PAIR relocation must immediately follow this one. The SymbolTableIndex of the PAIR relocation contains a signed 16-bit displacement that is added to the upper 16 bits that are taken from the location that is being relocated. */
        IMAGE_REL_MIPS_TOKEN            = 0x000E,   /* A CLR token. */
        IMAGE_REL_MIPS_JMPADDR16        = 0x0010,   /* The low 26 bits of the target’s VA. This supports the MIPS16 JAL instruction. */
        IMAGE_REL_MIPS_REFWORDNB        = 0x0022,   /* The target’s 32-bit RVA. */
        IMAGE_REL_MIPS_PAIR             = 0x0025    /* The relocation is valid only when it immediately follows a REFHI or SECRELHI relocation. Its SymbolTableIndex contains a displacement and not an index into the symbol table. */
        }
    }