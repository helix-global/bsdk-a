namespace BinaryStudio.PortableExecutable.Win32
    {
    public enum THUNK_ORDINAL : byte
        {
        THUNK_ORDINAL_NOTYPE,       // standard thunk
        THUNK_ORDINAL_ADJUSTOR,     // "this" adjustor thunk
        THUNK_ORDINAL_VCALL,        // virtual call thunk
        THUNK_ORDINAL_PCODE,        // pcode thunk
        THUNK_ORDINAL_LOAD,         // thunk which loads the address to jump to via unknown means...
        THUNK_ORDINAL_TRAMP_INCREMENTAL,
        THUNK_ORDINAL_TRAMP_BRANCHISLAND,
        }
    }