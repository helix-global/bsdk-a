namespace BinaryStudio.PortableExecutable.Win32
    {
    public enum IMAGE_REL_CEE : short
        {
        IMAGE_REL_CEE_ABSOLUTE          = 0x0000,   /* Reference is absolute, no relocation is necessary. */
        IMAGE_REL_CEE_ADDR32            = 0x0001,   /* 32-bit address (VA). */
        IMAGE_REL_CEE_ADDR64            = 0x0002,   /* 64-bit address (VA). */
        IMAGE_REL_CEE_ADDR32NB          = 0x0003,   /* 32-bit address w/o image base (RVA). */
        IMAGE_REL_CEE_SECTION           = 0x0004,   /* Section index. */
        IMAGE_REL_CEE_SECREL            = 0x0005,   /* 32 bit offset from base of section containing target. */
        IMAGE_REL_CEE_TOKEN             = 0x0006    /* 32 bit metadata token. */
        }
    }