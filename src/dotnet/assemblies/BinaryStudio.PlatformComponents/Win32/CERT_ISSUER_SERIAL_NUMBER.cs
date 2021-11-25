using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using CERT_NAME_BLOB = CRYPT_BLOB;
    using CRYPT_INTEGER_BLOB = CRYPT_BLOB;

    [StructLayout(LayoutKind.Sequential)]
    public struct CERT_ISSUER_SERIAL_NUMBER
        {
        public CERT_NAME_BLOB Issuer;
        public CRYPT_INTEGER_BLOB  SerialNumber;
        }
    }