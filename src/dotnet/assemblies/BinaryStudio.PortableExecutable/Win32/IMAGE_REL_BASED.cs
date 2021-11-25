namespace BinaryStudio.PortableExecutable.Win32
    {
    public enum IMAGE_REL_BASED : short
        {
        IMAGE_REL_BASED_ABSOLUTE              = 0,  /*  */
        IMAGE_REL_BASED_HIGH                  = 1,  /*  */
        IMAGE_REL_BASED_LOW                   = 2,  /*  */
        IMAGE_REL_BASED_HIGHLOW               = 3,  /*  */
        IMAGE_REL_BASED_HIGHADJ               = 4,  /*  */
        IMAGE_REL_BASED_MACHINE_SPECIFIC_5    = 5,  /*  */
        IMAGE_REL_BASED_RESERVED              = 6,  /*  */
        IMAGE_REL_BASED_MACHINE_SPECIFIC_7    = 7,  /*  */
        IMAGE_REL_BASED_MACHINE_SPECIFIC_8    = 8,  /*  */
        IMAGE_REL_BASED_MACHINE_SPECIFIC_9    = 9,  /*  */
        IMAGE_REL_BASED_DIR64                 = 10, /*  */
        IMAGE_REL_BASED_IA64_IMM64            = 9,  /*  */
        IMAGE_REL_BASED_MIPS_JMPADDR          = 5,  /*  */
        IMAGE_REL_BASED_MIPS_JMPADDR16        = 9,  /*  */
        IMAGE_REL_BASED_ARM_MOV32             = 5,  /*  */
        IMAGE_REL_BASED_THUMB_MOV32           = 7   /*  */
        }
    }