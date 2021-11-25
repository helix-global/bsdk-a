using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Win32
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1712:Do not prefix enum values with type name", Justification = "<Pending>")]
    #endif
    public enum CRYPT_VERIFY_CERT_SIGN_SUBJECT : uint
        {
        CRYPT_VERIFY_CERT_SIGN_SUBJECT_BLOB         = 1,
        CRYPT_VERIFY_CERT_SIGN_SUBJECT_CERT         = 2,
        CRYPT_VERIFY_CERT_SIGN_SUBJECT_CRL          = 3,
        CRYPT_VERIFY_CERT_SIGN_SUBJECT_OCSP_BASIC_SIGNED_RESPONSE   = 4
        }
    }