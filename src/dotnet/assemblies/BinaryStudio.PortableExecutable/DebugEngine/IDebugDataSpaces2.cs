using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("7A5E852F-96E9-468F-AC1B-0B3ADDC4A049"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugDataSpaces2 : IDebugDataSpaces
    {
        UInt64 VirtualToPhysical(
            [In] UInt64 Virtual);

        void GetVirtualTranslationPhysicalOffsets(
            [In] UInt64 Virtual,
            [Out] out UInt64 Offsets,
            [In] UInt32 OffsetsSize,
            [Out] out UInt32 Levels);

        void ReadHandleData(
            [In] UInt64 Handle,
            [In] UInt32 DataType,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 DataSize);

        void FillVirtual(
            [In] UInt64 Start,
            [In] UInt32 Size,
            [In] IntPtr Pattern,
            [In] UInt32 PatternSize,
            [Out] out UInt32 Filled);

        void FillPhysical(
            [In] UInt64 Start,
            [In] UInt32 Size,
            [In] IntPtr Pattern,
            [In] UInt32 PatternSize,
            [Out] out UInt32 Filled);

        MEMORY_BASIC_INFORMATION64 QueryVirtual(
            [In] UInt64 Offset);
        }
    }