using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("23F79D6C-8AAF-4F7C-A607-9995F5407E63"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugDataSpaces3 : IDebugDataSpaces2
        {
        IMAGE_NT_HEADERS64 ReadImageNtHeaders(
            [In] UInt64 ImageBase);

        void ReadTagged(
            [In] ref Guid Tag,
            [In] UInt32 Offset,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 TotalSize);

        UInt64 StartEnumTagged();

        void GetNextTagged(
            [In] UInt64 Handle,
            out Guid Tag,
            out UInt32 Size);

        void EndEnumTagged(
            [In] UInt64 Handle);
        }
    }