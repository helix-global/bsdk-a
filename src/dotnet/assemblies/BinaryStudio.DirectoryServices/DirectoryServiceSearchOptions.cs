using System;

namespace BinaryStudio.DirectoryServices
    {
    [Flags]
    public enum DirectoryServiceSearchOptions
        {
        None = 0,
        Recursive  = 1,
        Containers = 2
        }
    }