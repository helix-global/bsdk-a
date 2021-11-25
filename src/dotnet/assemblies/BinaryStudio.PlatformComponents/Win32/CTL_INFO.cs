using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using DWORD = UInt32;
    using CRYPT_DATA_BLOB = CRYPT_BLOB;
    using CRYPT_INTEGER_BLOB = CRYPT_BLOB;
    using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;
    using CTL_USAGE = CERT_ENHKEY_USAGE;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct CTL_INFO
        {
        public readonly DWORD dwVersion;
        public readonly CTL_USAGE SubjectUsage;
        public readonly CRYPT_DATA_BLOB ListIdentifier;
        public readonly CRYPT_INTEGER_BLOB SequenceNumber;
        public readonly FILETIME ThisUpdate;
        public readonly FILETIME NextUpdate;
        public readonly CRYPT_ALGORITHM_IDENTIFIER SubjectAlgorithm;
        public readonly DWORD cCTLEntry;
        public readonly unsafe CTL_ENTRY* rgCTLEntry;
        public readonly DWORD cExtension;
        public readonly unsafe CERT_EXTENSION* rgExtension;
        }
    }