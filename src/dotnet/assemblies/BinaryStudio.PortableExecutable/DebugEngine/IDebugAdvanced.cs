using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("F2DF5F53-071F-47BD-9DE6-5734C3FED689"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugAdvanced
        {
        void GetThreadContext([Out] IntPtr Context, [In] UInt32 ContextSize);
        void SetThreadContext([In] IntPtr Context, [In] UInt32 ContextSize);
        }
    }