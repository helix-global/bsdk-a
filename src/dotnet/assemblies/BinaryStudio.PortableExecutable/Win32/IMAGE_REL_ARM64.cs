namespace BinaryStudio.PortableExecutable.Win32
    {
    /// <summary>
    /// [ARM64]
    /// </summary>
    public enum IMAGE_REL_ARM64 : short
        {
        IMAGE_REL_ARM64_ABSOLUTE        = 0x0000,   /* The relocation is ignored. */
        IMAGE_REL_ARM64_ADDR32          = 0x0001,   /* The 32-bit VA of the target. */
        IMAGE_REL_ARM64_ADDR32NB        = 0x0002,   /* The 32-bit RVA of the target. */
        IMAGE_REL_ARM64_BRANCH26        = 0x0003,   /* The 26-bit relative displacement to the target, for B and BL instructions. */
        IMAGE_REL_ARM64_PAGEBASE_REL21  = 0x0004,   /* The page base of the target, for ADRP instruction. */
        IMAGE_REL_ARM64_REL21           = 0x0005,   /* The 12-bit relative displacement to the target, for instruction ADR. */
        IMAGE_REL_ARM64_PAGEOFFSET_12A  = 0x0006,   /* The 12-bit page offset of the target, for instructions ADD/ADDS (immediate) with zero shift. */
        IMAGE_REL_ARM64_PAGEOFFSET_12L  = 0x0007,   /* The 12-bit page offset of the target, for instruction LDR (indexed, unsigned immediate). */
        IMAGE_REL_ARM64_SECREL          = 0x0008,   /* The 32-bit offset of the target from the beginning of its section. This is used to support debugging information and static thread local storage. */
        IMAGE_REL_ARM64_SECREL_LOW12A   = 0x0009,   /* Bit 0:11 of section offset of the target, for instructions ADD/ADDS (immediate) with zero shift. */
        IMAGE_REL_ARM64_SECREL_HIGH12A  = 0x000A,   /* Bit 12:23 of section offset of the target, for instructions ADD/ADDS (immediate) with zero shift. */
        IMAGE_REL_ARM64_SECREL_LOW12L   = 0x000B,   /* Bit 0:11 of section offset of the target, for instruction LDR (indexed, unsigned immediate). */
        IMAGE_REL_ARM64_TOKEN           = 0x000C,   /* CLR token. */
        IMAGE_REL_ARM64_SECTION         = 0x000D,   /* The 16-bit section index of the section that contains the target. This is used to support debugging information. */
        IMAGE_REL_ARM64_ADDR64          = 0x000E,   /* The 64-bit VA of the relocation target. */
        IMAGE_REL_ARM64_BRANCH19        = 0x000F,   /* The 19-bit offset to the relocation target, for conditional B instruction. */
        IMAGE_REL_ARM64_BRANCH14        = 0x0010    /* The 14-bit offset to the relocation target, for instructions TBZ and TBNZ. */
        }
    }