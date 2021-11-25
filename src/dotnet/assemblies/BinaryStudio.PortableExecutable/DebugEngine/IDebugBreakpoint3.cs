using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("38F5C249-B448-43BB-9835-579D4EC02249"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugBreakpoint3 : IDebugBreakpoint2
        {
        Guid GetGuid();
        }
    }