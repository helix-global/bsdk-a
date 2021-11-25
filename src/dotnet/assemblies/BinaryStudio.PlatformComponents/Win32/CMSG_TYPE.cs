namespace Microsoft.Win32
    {
    public enum CMSG_TYPE
        {
        CMSG_NONE                    = 0,
        CMSG_DATA                    = 1,
        CMSG_SIGNED                  = 2,
        CMSG_ENVELOPED               = 3,
        CMSG_SIGNED_AND_ENVELOPED    = 4,
        CMSG_HASHED                  = 5,
        CMSG_ENCRYPTED               = 6
        }
    }