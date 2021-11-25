using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using CRYPT_OBJID_BLOB = CRYPT_BLOB;

    [StructLayout(LayoutKind.Sequential)]
    internal struct CRYPT_ATTRIBUTE_TYPE_VALUE
        {
        unsafe Byte*        pszObjId;
        CRYPT_OBJID_BLOB    Value;
        }
    }