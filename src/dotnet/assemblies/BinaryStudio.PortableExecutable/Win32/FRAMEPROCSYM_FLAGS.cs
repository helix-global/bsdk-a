using System;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [Flags]
    public enum FRAMEPROCSYM_FLAGS : uint
        {
        fHasAlloca         = 0x00000001,
        fHasSetJmp         = 0x00000002,
        fHasLongJmp        = 0x00000004,
        fHasInlAsm         = 0x00000008,
        fHasEH             = 0x00000010,
        fInlSpec           = 0x00000020,
        fHasSEH            = 0x00000040,
        fNaked             = 0x00000080,
        fSecurityChecks    = 0x00000100,
        fAsyncEH           = 0x00000200,
        fGSNoStackOrdering = 0x00000400,
        fWasInlined        = 0x00000800,
        fGSCheck           = 0x00001000,
        fSafeBuffers       = 0x00002000,
        fPogoOn            = 0x00040000,
        fValidCounts       = 0x00080000,
        fOptSpeed          = 0x00100000,
        fGuardCF           = 0x00200000,
        fGuardCFW          = 0x00400000
        }
    }