using System;

namespace BinaryStudio.Security.SmartCard
    {
    [Flags]
    public enum SCardProtocol
        {
        Undefined = 0x00000000,
        T0        = 0x00000001,
        T1        = 0x00000002,
        Raw       = 0x00010000
        }
    }