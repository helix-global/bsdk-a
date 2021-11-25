using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using CRYPT_OBJID_BLOB = CRYPT_BLOB;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CERT_EXTENSION
        {
        public readonly IntPtr              pszObjId;
        [MarshalAs(UnmanagedType.Bool)] public readonly Boolean fCritical;
        public CRYPT_OBJID_BLOB    Value;
        }
    }