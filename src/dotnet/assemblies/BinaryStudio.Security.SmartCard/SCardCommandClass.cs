using System;

namespace BinaryStudio.Security.SmartCard
    {
    [Flags]
    public enum SCardCommandClass
        {
        IsNotTheLastChainCommand = 0x10,
        }
    }