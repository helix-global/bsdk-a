using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using CRYPT_INTEGER_BLOB = CRYPT_BLOB;
    using CERT_NAME_BLOB = CRYPT_BLOB;
    using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CERT_INFO
        {
        public UInt32                           Version;
        public CRYPT_INTEGER_BLOB               SerialNumber;
        public CRYPT_ALGORITHM_IDENTIFIER       SignatureAlgorithm;
        public CERT_NAME_BLOB                   Issuer;
        public FILETIME                         NotBefore;
        public FILETIME                         NotAfter;
        public CERT_NAME_BLOB                   Subject;
        public CERT_PUBLIC_KEY_INFO             SubjectPublicKeyInfo;
        public CRYPT_BIT_BLOB                   IssuerUniqueId;
        public CRYPT_BIT_BLOB                   SubjectUniqueId;
        public readonly UInt32                  cExtension;
        public readonly unsafe CERT_EXTENSION*  rgExtension;
        }
    }