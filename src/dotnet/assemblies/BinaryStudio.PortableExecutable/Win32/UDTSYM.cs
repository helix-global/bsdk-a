﻿using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct UDTSYM
        {
        public readonly DEBUG_TYPE_ENUM TypeIndex;
        }
    }