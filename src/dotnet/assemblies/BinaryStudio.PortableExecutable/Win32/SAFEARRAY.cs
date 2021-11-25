using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [Flags]
    internal enum FADF : ushort
        {
        FADF_AUTO           = 0x0001,
        FADF_STATIC         = 0x0002,
        FADF_EMBEDDED       = 0x0004,
        FADF_FIXEDSIZE      = 0x0010,
        FADF_RECORD         = 0x0020,
        FADF_HAVEIID        = 0x0040,
        FADF_HAVEVARTYPE    = 0x0080,
        FADF_BSTR           = 0x0100,
        FADF_UNKNOWN        = 0x0200,
        FADF_DISPATCH       = 0x0400,
        FADF_VARIANT        = 0x0800,
        FADF_RESERVED       = 0xF008
        }

    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    internal struct SAFEARRAY32
        {
        public readonly Int16 cDims;
        public readonly FADF fFeatures;
        public readonly Int32 cbElements;
        public readonly Int32 cLocks;
        public readonly UInt32 pvData;
        }

    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    internal struct SAFEARRAY64
        {
        public readonly Int16 cDims;
        public readonly FADF fFeatures;
        public readonly Int32 cbElements;
        public readonly Int32 cLocks;
        public readonly UInt64 pvData;
        }
    }