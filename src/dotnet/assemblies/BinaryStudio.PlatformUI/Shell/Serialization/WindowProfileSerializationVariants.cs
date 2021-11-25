using System;

namespace BinaryStudio.PlatformUI.Shell
    {
    [Flags]
    public enum WindowProfileSerializationVariants
        {
        None = 0,
        Default = 1,
        Restricted = 2,
        All = 2147483647,
        }
    }