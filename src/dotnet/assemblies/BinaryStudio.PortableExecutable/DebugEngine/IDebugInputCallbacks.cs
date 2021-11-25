using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [Guid("9F50E42C-F136-499E-9A97-73036C94ED2D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)][ComImport]
    public interface IDebugInputCallbacks
        {
        [MethodImpl(MethodImplOptions.InternalCall)]
        void StartInput([In] UInt32 BufferSize);
 
        [MethodImpl(MethodImplOptions.InternalCall)]
        void EndInput();
        }
    }