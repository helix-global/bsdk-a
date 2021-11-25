namespace BinaryStudio.PortableExecutable.Win32
    {
    public enum IMAGE_REL_ALPHA : short
        {
        IMAGE_REL_ALPHA_ABSOLUTE        = 0x0000,   /* This relocation is ignored. */
        IMAGE_REL_ALPHA_REFLONG         = 0x0001,   /* The target's 32-bit virtual address. */
        IMAGE_REL_ALPHA_REFQUAD         = 0x0002,   /* The target's 64-bit virtual address. */
        IMAGE_REL_ALPHA_GPREL32         = 0x0003,   /* 32-bit signed displacement of the target relative to the Global Pointer (GP) register. */
        IMAGE_REL_ALPHA_LITERAL         = 0x0004,   /* 16-bit signed displacement of the target relative to the Global Pointer (GP) register. */
        IMAGE_REL_ALPHA_LITUSE          = 0x0005,   /* Reserved for future use. */
        IMAGE_REL_ALPHA_GPDISP          = 0x0006,   /* Reserved for future use. */
        IMAGE_REL_ALPHA_BRADDR          = 0x0007,   /* The 21-bit relative displacement to the target. */
        IMAGE_REL_ALPHA_HINT            = 0x0008,   /* 14-bit hints to the processor for the target of an Alpha jump instruction. */
        IMAGE_REL_ALPHA_INLINE_REFLONG  = 0x0009,   /* The target's 32-bit virtual address split into high and low 16-bit parts. */
        IMAGE_REL_ALPHA_REFHI           = 0x000A,   /* The high 16 bits of the target's 32-bit virtual address. */
        IMAGE_REL_ALPHA_REFLO           = 0x000B,   /* The low 16 bits of the target's virtual address. */
        IMAGE_REL_ALPHA_PAIR            = 0x000C,   /* This relocation is only valid when it immediately follows a REFHI , REFQ3, REFQ2, or SECRELHI relocation. */
        IMAGE_REL_ALPHA_MATCH           = 0x000D,   /* This relocation is only valid when it immediately follows INLINE_REFLONG relocation. */
        IMAGE_REL_ALPHA_SECTION         = 0x000E,   /* The 16-bit section index of the section containing the target. */
        IMAGE_REL_ALPHA_SECREL          = 0x000F,   /* The 32-bit offset of the target from the beginning of its section. */
        IMAGE_REL_ALPHA_REFLONGNB       = 0x0010,   /* The target's 32-bit relative virtual address. */
        IMAGE_REL_ALPHA_SECRELLO        = 0x0011,   /* The low 16 bits of the 32-bit offset of the target from the beginning of its section. */
        IMAGE_REL_ALPHA_SECRELHI        = 0x0012,   /* The high 16 bits of the 32-bit offset of the target from the beginning of its section. */
        IMAGE_REL_ALPHA_REFQ3           = 0x0013,   /* The low 16 bits of the high 32 bits of the target's 64-bit virtual address. */
        IMAGE_REL_ALPHA_REFQ2           = 0x0014,   /* The high 16 bits of the low 32 bits of the target's 64-bit virtual address. */
        IMAGE_REL_ALPHA_REFQ1           = 0x0015,   /* The low 16 bits of the target's 64-bit virtual address. */
        IMAGE_REL_ALPHA_GPRELLO         = 0x0016,   /* The low 16 bits of the 32-bit signed displacement of the target relative to the Global Pointer (GP) register. */
        IMAGE_REL_ALPHA_GPRELHI         = 0x0017    /* The high 16 bits of the 32-bit signed displacement of the target relative to the Global Pointer (GP) register. */
        }
    }