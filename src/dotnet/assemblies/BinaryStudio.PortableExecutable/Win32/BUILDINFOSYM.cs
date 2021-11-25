using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BUILDINFOSYM
        {
        public readonly DEBUG_TYPE_ENUM TypeIndex;
        }
    }