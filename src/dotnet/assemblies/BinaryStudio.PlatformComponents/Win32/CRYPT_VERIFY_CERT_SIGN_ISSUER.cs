using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Win32
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1712:Do not prefix enum values with type name", Justification = "<Pending>")]
    #endif
    public enum CRYPT_VERIFY_CERT_SIGN_ISSUER : uint
        {
        CRYPT_VERIFY_CERT_SIGN_ISSUER_PUBKEY        = 1,
        CRYPT_VERIFY_CERT_SIGN_ISSUER_CERT          = 2,
        CRYPT_VERIFY_CERT_SIGN_ISSUER_CHAIN         = 3,
        CRYPT_VERIFY_CERT_SIGN_ISSUER_NULL          = 4
        }
    }