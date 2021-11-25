using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("0AE9F5FF-1852-4679-B055-494BEE6407EE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugSystemObjects2 : IDebugSystemObjects
        {
        UInt32 GetCurrentProcessUpTime();

        UInt64 GetImplicitThreadDataOffset();

        void SetImplicitThreadDataOffset(
            [In] UInt64 Offset);

        UInt64 GetImplicitProcessDataOffset();

        void SetImplicitProcessDataOffset(
            [In] UInt64 Offset);
        }
    }