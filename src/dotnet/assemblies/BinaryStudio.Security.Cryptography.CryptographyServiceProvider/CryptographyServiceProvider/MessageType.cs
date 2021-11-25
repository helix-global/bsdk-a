using System.Diagnostics.CodeAnalysis;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [SuppressMessage("Design", "CA1028:Enum Storage should be Int32", Justification = "<Pending>")]
    #endif
    public enum MessageType : uint
        {
        NONE = 0,
        CMSG_DATA                    = 1,
        CMSG_SIGNED                  = 2,
        CMSG_ENVELOPED               = 3,
        CMSG_SIGNED_AND_ENVELOPED    = 4,
        CMSG_HASHED                  = 5,
        CMSG_ENCRYPTED               = 6
        }
    }