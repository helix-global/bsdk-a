namespace BinaryStudio.PortableExecutable.Win32
    {
    public enum IMAGE_REL_EBC : short
        {
        IMAGE_REL_EBC_ABSOLUTE          = 0x0000,   /* No relocation required. */
        IMAGE_REL_EBC_ADDR32NB          = 0x0001,   /* 32 bit address w/o image base. */
        IMAGE_REL_EBC_REL32             = 0x0002,   /* 32-bit relative address from byte following reloc. */
        IMAGE_REL_EBC_SECTION           = 0x0003,   /* Section table index. */
        IMAGE_REL_EBC_SECREL            = 0x0004    /* Offset within section. */
        }
    }