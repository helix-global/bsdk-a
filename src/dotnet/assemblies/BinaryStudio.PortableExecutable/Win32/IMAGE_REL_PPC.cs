namespace BinaryStudio.PortableExecutable.Win32
    {
    /// <summary>
    /// [PowerPC]
    /// </summary>
    public enum IMAGE_REL_PPC : short
        {
        IMAGE_REL_PPC_ABSOLUTE          = 0x0000,   /* The relocation is ignored. */
        IMAGE_REL_PPC_ADDR64            = 0x0001,   /* The 64-bit VA of the target. */
        IMAGE_REL_PPC_ADDR32            = 0x0002,   /* The 32-bit VA of the target. */
        IMAGE_REL_PPC_ADDR24            = 0x0003,   /* The low 24 bits of the VA of the target. This is valid only when the target symbol is absolute and can be sign-extended to its original value. */
        IMAGE_REL_PPC_ADDR16            = 0x0004,   /* The low 16 bits of the target’s VA. */
        IMAGE_REL_PPC_ADDR14            = 0x0005,   /* The low 14 bits of the target’s VA. This is valid only when the target symbol is absolute and can be sign-extended to its original value. */
        IMAGE_REL_PPC_REL24             = 0x0006,   /* A 24-bit PC-relative offset to the symbol’s location. */
        IMAGE_REL_PPC_REL14             = 0x0007,   /* A 14-bit PC-relative offset to the symbol’s location. */
        IMAGE_REL_PPC_TOCREL16          = 0x0008,   /* 16-bit offset from TOC base. */
        IMAGE_REL_PPC_TOCREL14          = 0x0009,   /* 16-bit offset from TOC base, shifted left 2 (load doubleword). */
        IMAGE_REL_PPC_ADDR32NB          = 0x000A,   /* The 32-bit RVA of the target. */
        IMAGE_REL_PPC_SECREL            = 0x000B,   /* The 32-bit offset of the target from the beginning of its section. This is used to support debugging information and static thread local storage. */
        IMAGE_REL_PPC_SECTION           = 0x000C,   /* The 16-bit section index of the section that contains the target. This is used to support debugging information. */
        IMAGE_REL_PPC_IFGLUE            = 0x000D,   /* Substitute TOC restore instruction iff symbol is glue code. */
        IMAGE_REL_PPC_IMGLUE            = 0x000E,   /* Symbol is glue code; virtual address is TOC restore instruction. */
        IMAGE_REL_PPC_SECREL16          = 0x000F,   /* The 16-bit offset of the target from the beginning of its section. This is used to support debugging information and static thread local storage. */
        IMAGE_REL_PPC_REFHI             = 0x0010,   /* The high 16 bits of the target’s 32-bit VA. This is used for the first instruction in a two-instruction sequence that loads a full address. This relocation must be immediately followed by a PAIR relocation whose SymbolTableIndex contains a signed 16-bit displacement that is added to the upper 16 bits that was taken from the location that is being relocated. */
        IMAGE_REL_PPC_REFLO             = 0x0011,   /* The low 16 bits of the target’s VA. */
        IMAGE_REL_PPC_PAIR              = 0x0012,   /* A relocation that is valid only when it immediately follows a REFHI or SECRELHI relocation. Its SymbolTableIndex contains a displacement and not an index into the symbol table. */
        IMAGE_REL_PPC_SECRELLO          = 0x0013,   /* The low 16 bits of the 32-bit offset of the target from the beginning of its section. */
        IMAGE_REL_PPC_SECRELHI          = 0x0014,   /* High 16-bit section relative reference (used for >32k TLS). */
        IMAGE_REL_PPC_GPREL             = 0x0015,   /* The 16-bit signed displacement of the target relative to the GP register. */
        IMAGE_REL_PPC_TOKEN             = 0x0016    /* The CLR token. */
        }
    }