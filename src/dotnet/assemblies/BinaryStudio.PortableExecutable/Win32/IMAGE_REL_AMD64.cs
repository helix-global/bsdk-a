namespace BinaryStudio.PortableExecutable.Win32
    {
    /// <summary>
    /// [x64]
    /// </summary>
    public enum IMAGE_REL_AMD64 : short
        {
        IMAGE_REL_AMD64_ABSOLUTE        = 0x0000,   /* The relocation is ignored. */
        IMAGE_REL_AMD64_ADDR64          = 0x0001,   /* The 64-bit VA of the relocation target. */
        IMAGE_REL_AMD64_ADDR32          = 0x0002,   /* The 32-bit VA of the relocation target. */
        IMAGE_REL_AMD64_ADDR32NB        = 0x0003,   /* The 32-bit address without an image base (RVA). */
        IMAGE_REL_AMD64_REL32           = 0x0004,   /* The 32-bit relative address from the byte following the relocation. */
        IMAGE_REL_AMD64_REL32_1         = 0x0005,   /* The 32-bit address relative to byte distance 1 from the relocation. */
        IMAGE_REL_AMD64_REL32_2         = 0x0006,   /* The 32-bit address relative to byte distance 2 from the relocation. */
        IMAGE_REL_AMD64_REL32_3         = 0x0007,   /* The 32-bit address relative to byte distance 3 from the relocation. */
        IMAGE_REL_AMD64_REL32_4         = 0x0008,   /* The 32-bit address relative to byte distance 4 from the relocation. */
        IMAGE_REL_AMD64_REL32_5         = 0x0009,   /* The 32-bit address relative to byte distance 5 from the relocation. */
        IMAGE_REL_AMD64_SECTION         = 0x000A,   /* The 16-bit section index of the section that contains the target. This is used to support debugging information. */
        IMAGE_REL_AMD64_SECREL          = 0x000B,   /* The 32-bit offset of the target from the beginning of its section. This is used to support debugging information and static thread local storage. */
        IMAGE_REL_AMD64_SECREL7         = 0x000C,   /* A 7-bit unsigned offset from the base of the section that contains the target. */
        IMAGE_REL_AMD64_TOKEN           = 0x000D,   /* CLR tokens. */
        IMAGE_REL_AMD64_SREL32          = 0x000E,   /* A 32-bit signed span-dependent value emitted into the object. */
        IMAGE_REL_AMD64_PAIR            = 0x000F,   /* A pair that must immediately follow every span-dependent value. */
        IMAGE_REL_AMD64_SSPAN32         = 0x0010    /* A 32-bit signed span-dependent value that is applied at link time. */
        }
    }