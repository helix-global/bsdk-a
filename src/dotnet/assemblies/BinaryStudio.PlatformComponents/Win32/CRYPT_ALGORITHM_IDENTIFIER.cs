using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using CRYPT_OBJID_BLOB = CRYPT_BLOB;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CRYPT_ALGORITHM_IDENTIFIER
        {
        public IntPtr           ObjectId;
        public readonly CRYPT_OBJID_BLOB Parameters;
        }
    }