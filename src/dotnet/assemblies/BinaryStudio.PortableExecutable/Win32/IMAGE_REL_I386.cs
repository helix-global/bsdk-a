namespace BinaryStudio.PortableExecutable.Win32
    {
    /// <summary>
    /// [Intel 386 Processors]
    /// </summary>
    public enum IMAGE_REL_I386 : short
        {
        IMAGE_REL_I386_ABSOLUTE         = 0x0000,   /* The relocation is ignored. */
        IMAGE_REL_I386_DIR16            = 0x0001,   /* Not supported. */
        IMAGE_REL_I386_REL16            = 0x0002,   /* Not supported. */
        IMAGE_REL_I386_DIR32            = 0x0006,   /* The target’s 32-bit VA. */
        IMAGE_REL_I386_DIR32NB          = 0x0007,   /* The target’s 32-bit RVA. */
        IMAGE_REL_I386_SEG12            = 0x0009,   /* Not supported. */
        IMAGE_REL_I386_SECTION          = 0x000A,   /* The 16-bit section index of the section that contains the target. This is used to support debugging information. */
        IMAGE_REL_I386_SECREL           = 0x000B,   /* The 32-bit offset of the target from the beginning of its section. This is used to support debugging information and static thread local storage. */
        IMAGE_REL_I386_TOKEN            = 0x000C,   /* The CLR token. */
        IMAGE_REL_I386_SECREL7          = 0x000D,   /* A 7-bit offset from the base of the section that contains the target. */
        IMAGE_REL_I386_REL32            = 0x0014    /* The 32-bit relative displacement to the target. This supports the x86 relative branch and call instructions. */
        }
    }