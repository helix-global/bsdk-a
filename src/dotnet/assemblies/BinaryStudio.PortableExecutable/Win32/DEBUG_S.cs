namespace BinaryStudio.PortableExecutable.Win32
    {
    public enum DEBUG_S
        {
        DEBUG_S_SYMBOLS                 = 0xF1,
        DEBUG_S_LINES                   = 0xF2,
        DEBUG_S_STRINGTABLE             = 0xF3,
        DEBUG_S_FILECHKSMS              = 0xF4,
        DEBUG_S_FRAMEDATA               = 0xF5,
        DEBUG_S_INLINEELINES            = 0xF6,
        DEBUG_S_CROSSSCOPEIMPORTS       = 0xF7,
        DEBUG_S_CROSSSCOPEEXPORTS       = 0xF8,
        DEBUG_S_IL_LINES                = 0xF9,
        DEBUG_S_FUNC_MDTOKEN_MAP        = 0xFA,
        DEBUG_S_TYPE_MDTOKEN_MAP        = 0xFB,
        DEBUG_S_MERGED_ASSEMBLYINPUT    = 0xFC,
        DEBUG_S_COFF_SYMBOL_RVA         = 0xFD
        }
    }