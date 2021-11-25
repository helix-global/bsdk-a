using System;

namespace BinaryStudio.IO
    {
    [Flags]
    public enum Base32FormattingFlags
        {
        None   = 0,
        Header = 1,
        Offset = 2,
        }
    }