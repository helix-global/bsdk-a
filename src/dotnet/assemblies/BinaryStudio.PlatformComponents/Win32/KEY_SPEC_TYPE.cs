using System;

namespace Microsoft.Win32
    {
    [Flags]
    public enum KEY_SPEC_TYPE : uint
        {
        AT_KEYEXCHANGE = 1,
        AT_SIGNATURE   = 2
        }
    }