namespace BinaryStudio.PortableExecutable.Win32
    {
    public enum IMAGE_SYM_CLASS : sbyte
        {
        IMAGE_SYM_CLASS_END_OF_FUNCTION    = -1,
        IMAGE_SYM_CLASS_NULL               = 0x0000,
        IMAGE_SYM_CLASS_AUTOMATIC          = 0x0001,
        IMAGE_SYM_CLASS_EXTERNAL           = 0x0002,
        IMAGE_SYM_CLASS_STATIC             = 0x0003,
        IMAGE_SYM_CLASS_REGISTER           = 0x0004,
        IMAGE_SYM_CLASS_EXTERNAL_DEF       = 0x0005,
        IMAGE_SYM_CLASS_LABEL              = 0x0006,
        IMAGE_SYM_CLASS_UNDEFINED_LABEL    = 0x0007,
        IMAGE_SYM_CLASS_MEMBER_OF_STRUCT   = 0x0008,
        IMAGE_SYM_CLASS_ARGUMENT           = 0x0009,
        IMAGE_SYM_CLASS_STRUCT_TAG         = 0x000A,
        IMAGE_SYM_CLASS_MEMBER_OF_UNION    = 0x000B,
        IMAGE_SYM_CLASS_UNION_TAG          = 0x000C,
        IMAGE_SYM_CLASS_TYPE_DEFINITION    = 0x000D,
        IMAGE_SYM_CLASS_UNDEFINED_STATIC   = 0x000E,
        IMAGE_SYM_CLASS_ENUM_TAG           = 0x000F,
        IMAGE_SYM_CLASS_MEMBER_OF_ENUM     = 0x0010,
        IMAGE_SYM_CLASS_REGISTER_PARAM     = 0x0011,
        IMAGE_SYM_CLASS_BIT_FIELD          = 0x0012,
        IMAGE_SYM_CLASS_FAR_EXTERNAL       = 0x0044,
        IMAGE_SYM_CLASS_BLOCK              = 0x0064,
        IMAGE_SYM_CLASS_FUNCTION           = 0x0065,
        IMAGE_SYM_CLASS_END_OF_STRUCT      = 0x0066,
        IMAGE_SYM_CLASS_FILE               = 0x0067,
        IMAGE_SYM_CLASS_SECTION            = 0x0068,
        IMAGE_SYM_CLASS_WEAK_EXTERNAL      = 0x0069,
        IMAGE_SYM_CLASS_CLR_TOKEN          = 0x006B
        }
    }