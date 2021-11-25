using System;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [Flags]
    public enum FRAMEDATA_FLAGS : uint
        {
        fHasSEH          = 0x01,
        fHasEH           = 0x02,
        fIsFunctionStart = 0x04
        }
    }