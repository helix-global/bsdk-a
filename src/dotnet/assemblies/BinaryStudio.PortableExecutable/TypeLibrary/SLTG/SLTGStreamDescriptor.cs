using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class SLTGStreamDescriptor
        {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct STREAM
            {
            private readonly UInt32 Length;             /* Length of the stream. */
            private readonly UInt16 NameOffset;         /* Offset of the name in the name table. */
            private readonly UInt16 NextStream;         /* Index into this array of the next stream. This linked list specifies the order of the actual stream data in the file. */
            }
        }

    internal class SLTGStreamDescriptor<T> : SLTGStreamDescriptor
        {
        
        }
    }