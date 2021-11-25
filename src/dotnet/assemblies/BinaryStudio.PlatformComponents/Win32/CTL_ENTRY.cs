using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using DWORD = UInt32;
    using CRYPT_DATA_BLOB = CRYPT_BLOB;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct CTL_ENTRY
        {
        CRYPT_DATA_BLOB     SubjectIdentifier;
        DWORD               cAttribute;
        CRYPT_ATTRIBUTE     rgAttribute;
        }
    }