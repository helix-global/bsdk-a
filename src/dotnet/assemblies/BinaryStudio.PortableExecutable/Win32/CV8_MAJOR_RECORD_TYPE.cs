namespace BinaryStudio.PortableExecutable.Win32
    {
    public enum CV8_MAJOR_RECORD_TYPE
        {
        CV8_MAJOR_RECORD_TYPE_SOURCE_FILENAME_STRING_TABLE  = 0x00f3,
        CV8_MAJOR_RECORD_TYPE_SOURCE_FILE_INFO              = 0x00f4,
        CV8_MAJOR_RECORD_TYPE_LINE_NUMBERS_FOR_SECTION      = 0x00f2,
        CV8_MAJOR_RECORD_TYPE_SYMBOL_INFORMATION            = 0x00f1,
        CV8_MAJOR_RECORD_TYPE_NAME_OF_OBJECT_FILE           = 0x1101,
        CV8_MAJOR_RECORD_TYPE_CREATOR_SIGNATURE             = 0x1116,
        CV8_MAJOR_RECORD_TYPE_CODE_LABEL                    = 0x1105,
        CV8_MAJOR_RECORD_TYPE_LOCAL_DATA                    = 0x110c,
        CV8_MAJOR_RECORD_TYPE_GLOBAL_DATA                   = 0x110d,
        CV8_MAJOR_RECORD_TYPE_PROCEDURE_START               = 0x1110
        }
    }