using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Win32
    {
    [Flags]
    #if CODE_ANALYSIS
    [SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "<Pending>")]
    #endif
    public enum CRYPT_MSG_TYPE
        {
        CRYPT_ASN_ENCODING   = 0x00000001,
        CRYPT_NDR_ENCODING   = 0x00000002,
        X509_ASN_ENCODING    = 0x00000001,
        X509_NDR_ENCODING    = 0x00000002,
        PKCS_7_ASN_ENCODING  = 0x00010000,
        PKCS_7_NDR_ENCODING  = 0x00020000
        }
    }