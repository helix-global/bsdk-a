using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using CRYPT_DATA_BLOB = CRYPT_BLOB;

    [StructLayout(LayoutKind.Sequential)]
    public struct CRYPT_OID_INFO
        {
        public Int32 cbSize;
        public IntPtr pszOID;
        public IntPtr pwszName;
        public Int32 dwGroupId;
        public ALG_ID Algid;
        public CRYPT_DATA_BLOB ExtraInfo;
        public IntPtr pwszCNGAlgid;
        public IntPtr pwszCNGExtraAlgid;
        }
    }