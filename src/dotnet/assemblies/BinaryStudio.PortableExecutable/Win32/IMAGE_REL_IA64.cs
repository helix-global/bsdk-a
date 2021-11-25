namespace BinaryStudio.PortableExecutable.Win32
    {
    /// <summary>
    /// [Intel Itanium Processor Family (IPF)]
    /// </summary>
    public enum IMAGE_REL_IA64 : short
        {
        IMAGE_REL_IA64_ABSOLUTE         = 0x0000,   /* The relocation is ignored. */
        IMAGE_REL_IA64_IMM14            = 0x0001,   /* The instruction relocation can be followed by an ADDEND relocation whose value is added to the target address before it is inserted into the specified slot in the IMM14 bundle. The relocation target must be absolute or the image must be fixed. */
        IMAGE_REL_IA64_IMM22            = 0x0002,   /* The instruction relocation can be followed by an ADDEND relocation whose value is added to the target address before it is inserted into the specified slot in the IMM22 bundle. The relocation target must be absolute or the image must be fixed. */
        IMAGE_REL_IA64_IMM64            = 0x0003,   /* The slot number of this relocation must be one (1). The relocation can be followed by an ADDEND relocation whose value is added to the target address before it is stored in all three slots of the IMM64 bundle. */
        IMAGE_REL_IA64_DIR32            = 0x0004,   /* The target’s 32-bit VA. This is supported only for /LARGEADDRESSAWARE:NO images. */
        IMAGE_REL_IA64_DIR64            = 0x0005,   /* The target’s 64-bit VA. */
        IMAGE_REL_IA64_PCREL21B         = 0x0006,   /* The instruction is fixed up with the 25-bit relative displacement to the 16-bit aligned target. The low 4 bits of the displacement are zero and are not stored. */
        IMAGE_REL_IA64_PCREL21M         = 0x0007,   /* The instruction is fixed up with the 25-bit relative displacement to the 16-bit aligned target. The low 4 bits of the displacement, which are zero, are not stored. */
        IMAGE_REL_IA64_PCREL21F         = 0x0008,   /* The LSBs of this relocation’s offset must contain the slot number whereas the rest is the bundle address. The bundle is fixed up with the 25-bit relative displacement to the 16-bit aligned target. The low 4 bits of the displacement are zero and are not stored. */
        IMAGE_REL_IA64_GPREL22          = 0x0009,   /* The instruction relocation can be followed by an ADDEND relocation whose value is added to the target address and then a 22-bit GP-relative offset that is calculated and applied to the GPREL22 bundle. */
        IMAGE_REL_IA64_LTOFF22          = 0x000A,   /* The instruction is fixed up with the 22-bit GP-relative offset to the target symbol’s literal table entry. The linker creates this literal table entry based on this relocation and the ADDEND relocation that might follow. */
        IMAGE_REL_IA64_SECTION          = 0x000B,   /* The 16-bit section index of the section contains the target. This is used to support debugging information. */
        IMAGE_REL_IA64_SECREL22         = 0x000C,   /* The instruction is fixed up with the 22-bit offset of the target from the beginning of its section. This relocation can be followed immediately by an ADDEND relocation, whose Value field contains the 32-bit unsigned offset of the target from the beginning of the section. */
        IMAGE_REL_IA64_SECREL64I        = 0x000D,   /* The slot number for this relocation must be one (1). The instruction is fixed up with the 64-bit offset of the target from the beginning of its section. This relocation can be followed immediately by an ADDEND relocation whose Value field contains the 32-bit unsigned offset of the target from the beginning of the section. */
        IMAGE_REL_IA64_SECREL32         = 0x000E,   /* The address of data to be fixed up with the 32-bit offset of the target from the beginning of its section. */
        IMAGE_REL_IA64_DIR32NB          = 0x0010,   /* The target’s 32-bit RVA. */
        IMAGE_REL_IA64_SREL14           = 0x0011,   /* This is applied to a signed 14-bit immediate that contains the difference between two relocatable targets. This is a declarative field for the linker that indicates that the compiler has already emitted this value. */
        IMAGE_REL_IA64_SREL22           = 0x0012,   /* This is applied to a signed 22-bit immediate that contains the difference between two relocatable targets. This is a declarative field for the linker that indicates that the compiler has already emitted this value. */
        IMAGE_REL_IA64_SREL32           = 0x0013,   /* This is applied to a signed 32-bit immediate that contains the difference between two relocatable values. This is a declarative field for the linker that indicates that the compiler has already emitted this value. */
        IMAGE_REL_IA64_UREL32           = 0x0014,   /* This is applied to an unsigned 32-bit immediate that contains the difference between two relocatable values. This is a declarative field for the linker that indicates that the compiler has already emitted this value. */
        IMAGE_REL_IA64_PCREL60X         = 0x0015,   /* A 60-bit PC-relative fixup that always stays as a BRL instruction of an MLX bundle. */
        IMAGE_REL_IA64_PCREL60B         = 0x0016,   /* A 60-bit PC-relative fixup. If the target displacement fits in a signed 25-bit field, convert the entire bundle to an MBB bundle with NOP.B in slot 1 and a 25-bit BR instruction (with the 4 lowest bits all zero and dropped) in slot 2. */
        IMAGE_REL_IA64_PCREL60F         = 0x0017,   /* A 60-bit PC-relative fixup. If the target displacement fits in a signed 25-bit field, convert the entire bundle to an MFB bundle with NOP.F in slot 1 and a 25-bit (4 lowest bits all zero and dropped) BR instruction in slot 2. */
        IMAGE_REL_IA64_PCREL60I         = 0x0018,   /* A 60-bit PC-relative fixup. If the target displacement fits in a signed 25-bit field, convert the entire bundle to an MIB bundle with NOP.I in slot 1 and a 25-bit (4 lowest bits all zero and dropped) BR instruction in slot 2. */
        IMAGE_REL_IA64_PCREL60M         = 0X0019,   /* A 60-bit PC-relative fixup. If the target displacement fits in a signed 25-bit field, convert the entire bundle to an MMB bundle with NOP.M in slot 1 and a 25-bit (4 lowest bits all zero and dropped) BR instruction in slot 2. */
        IMAGE_REL_IA64_IMMGPREL64       = 0X001A,   /* A 64-bit GP-relative fixup. */
        IMAGE_REL_IA64_TOKEN            = 0X001B,   /* A CLR token. */
        IMAGE_REL_IA64_GPREL32          = 0X001C,   /* A 32-bit GP-relative fixup. */
        IMAGE_REL_IA64_ADDEND           = 0X001F    /* The relocation is valid only when it immediately follows one of the following relocations: IMM14, IMM22, IMM64, GPREL22, LTOFF22, LTOFF64, SECREL22, SECREL64I, or SECREL32. Its value contains the addend to apply to instructions within a bundle, not for data. */
        }
    }