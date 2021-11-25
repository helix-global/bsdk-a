using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Win32
    {
    [Flags]
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [SuppressMessage("Design", "CA1028:Enum Storage should be Int32", Justification = "<Pending>")]
    #endif
    public enum CRYPT_MESSAGE_FLAGS : uint
        {
        CRYPT_MESSAGE_NONE = 0,
        CRYPT_MESSAGE_BARE_CONTENT_OUT_FLAG         = 0x00000001,
        CRYPT_MESSAGE_ENCAPSULATED_CONTENT_OUT_FLAG = 0x00000002,
        CRYPT_MESSAGE_KEYID_SIGNER_FLAG             = 0x00000004,
        CRYPT_MESSAGE_SILENT_KEYSET_FLAG            = 0x00000040,
        CMSG_CRYPT_RELEASE_CONTEXT_FLAG             = 0x00008000
        }
    }