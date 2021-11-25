using System;

namespace BinaryStudio.IO
    {
    [Flags]
    public enum PageProtection : uint
        {
        #if UBUNTU_16_4
        None      = 0,
        Read      = 1,
        Write     = 2,
        Execute   = 4,
        GrowsDown = 0x01000000,
        GrowSup   = 0x02000000
        #else
        NoAccess                = 0x00000001,
        ReadOnly                = 0x00000002,
        ReadWrite               = 0x00000004,
        WriteCopy               = 0x00000008,
        Execute                 = 0x00000010,
        ExecuteRead             = 0x00000020,
        ExecuteReadWrite        = 0x00000040,
        ExecuteWriteCopy        = 0x00000080,
        Guard                   = 0x00000100,
        NoCache                 = 0x00000200,
        WriteCombine            = 0x00000400,
        SecurityFile            = 0x00800000,
        SecurityImage           = 0x01000000,
        SecurityProtectedImage  = 0x02000000,
        SecurityReserve         = 0x04000000,
        SecurityCommit          = 0x08000000,
        SecurityNoCache         = 0x10000000,
        SecurityWriteCombine    = 0x40000000,
        SecurityLargePages      = 0x80000000
        #endif
        }
    }
