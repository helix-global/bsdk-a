namespace BinaryStudio.PortableExecutable.Win32
    {
    internal enum IMAGE_REL_AM : short
        {
        IMAGE_REL_AM_ABSOLUTE           = 0x0000,   /*  */
        IMAGE_REL_AM_ADDR32             = 0x0001,   /*  */
        IMAGE_REL_AM_ADDR32NB           = 0x0002,   /*  */
        IMAGE_REL_AM_CALL32             = 0x0003,   /*  */
        IMAGE_REL_AM_FUNCINFO           = 0x0004,   /*  */
        IMAGE_REL_AM_REL32_1            = 0x0005,   /*  */
        IMAGE_REL_AM_REL32_2            = 0x0006,   /*  */
        IMAGE_REL_AM_SECREL             = 0x0007,   /*  */
        IMAGE_REL_AM_SECTION            = 0x0008,   /*  */
        IMAGE_REL_AM_TOKEN              = 0x0009    /*  */
        }
    }