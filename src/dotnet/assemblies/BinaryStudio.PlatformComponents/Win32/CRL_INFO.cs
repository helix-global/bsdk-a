using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using DWORD = UInt32;
    using CERT_NAME_BLOB = CRYPT_BLOB;
    using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CRL_INFO
        {
        DWORD dwVersion;
        CRYPT_ALGORITHM_IDENTIFIER SignatureAlgorithm;
        CERT_NAME_BLOB Issuer;
        FILETIME ThisUpdate;
        FILETIME NextUpdate;
        DWORD cCRLEntry;
        unsafe CRL_ENTRY* rgCRLEntry;
        DWORD cExtension;
        unsafe CERT_EXTENSION* rgExtension;
        }
    }