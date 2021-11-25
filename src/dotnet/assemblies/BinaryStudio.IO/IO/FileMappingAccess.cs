using System;

namespace BinaryStudio.IO
    {
    [Flags]
    internal enum FileMappingAccess : uint
        {
        AllAccess               = SectionAllAccess,
        Read                    = SectionRead,
        Copy                    = SectionQuery,
        Write                   = SectionWrite,
        Execute                 = SectionExecuteExplicit,
        SectionQuery            = 0x0001,
        SectionWrite            = 0x0002,
        SectionRead             = 0x0004,
        SectionExecute          = 0x0008,
        SectionExtendSize       = 0x0010,
        SectionExecuteExplicit  = 0x0020,
        StandardRightsRequired  = 0x000F0000,
        SectionAllAccess        = StandardRightsRequired|SectionQuery|SectionWrite|SectionRead|SectionExecute|SectionExtendSize
        }
    }
