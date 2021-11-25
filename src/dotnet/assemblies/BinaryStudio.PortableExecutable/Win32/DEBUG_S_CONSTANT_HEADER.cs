using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DEBUG_S_CONSTANT_HEADER
        {
        public readonly DEBUG_TYPE_ENUM FieldTypeIndex;
        public readonly LEAF_ENUM FieldValue;
        }
    }