namespace Microsoft.Win32
    {
    public enum CERT_STORE_ADD : uint
        {
        CERT_STORE_ADD_NEW                                  = 1,
        CERT_STORE_ADD_USE_EXISTING                         = 2,
        CERT_STORE_ADD_REPLACE_EXISTING                     = 3,
        CERT_STORE_ADD_ALWAYS                               = 4,
        CERT_STORE_ADD_REPLACE_EXISTING_INHERIT_PROPERTIES  = 5,
        CERT_STORE_ADD_NEWER                                = 6,
        CERT_STORE_ADD_NEWER_INHERIT_PROPERTIES             = 7
        }
    }