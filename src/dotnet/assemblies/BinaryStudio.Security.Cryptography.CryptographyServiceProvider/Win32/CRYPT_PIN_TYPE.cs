namespace BinaryStudio.Security.Cryptography.Win32
    {
    internal enum CRYPT_PIN_TYPE : byte
        {
        CRYPT_PIN_PASSWD                = 0,
        CRYPT_PIN_ENCRYPTION            = 1,
        CRYPT_PIN_NK                    = 2,
        CRYPT_PIN_UNKNOWN               = 3,
        CRYPT_PIN_QUERY                 = 4,
        CRYPT_PIN_CLEAR                 = 5,
        CRYPT_PIN_HARDWARE_PROTECTION   = 6,
        CRYPT_PIN_FKC_EKE               = 7
        }
    }