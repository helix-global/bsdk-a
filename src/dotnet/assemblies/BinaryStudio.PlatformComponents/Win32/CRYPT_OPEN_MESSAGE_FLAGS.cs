using System;

namespace Microsoft.Win32
    {
    [Flags]
    public enum CRYPT_OPEN_MESSAGE_FLAGS : uint
        {
        CMSG_NONE                           = 0x00000000,
        CMSG_BARE_CONTENT_FLAG              = 0x00000001,
        CMSG_LENGTH_ONLY_FLAG               = 0x00000002,
        CMSG_DETACHED_FLAG                  = 0x00000004,
        CMSG_AUTHENTICATED_ATTRIBUTES_FLAG  = 0x00000008,
        CMSG_CONTENTS_OCTETS_FLAG           = 0x00000010,
        CMSG_MAX_LENGTH_FLAG                = 0x00000020,
        CMSG_CMS_ENCAPSULATED_CONTENT_FLAG  = 0x00000040,
        CMSG_CRYPT_RELEASE_CONTEXT_FLAG     = 0x00008000
        }
    }