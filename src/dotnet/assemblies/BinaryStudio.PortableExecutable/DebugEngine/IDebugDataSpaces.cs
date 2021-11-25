using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("88F7DFAB-3EA7-4C3A-AEFB-C4E8106173AA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugDataSpaces
    {
        void ReadVirtual(
            [In] UInt64 Offset,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesRead);

        void WriteVirtual(
            [In] UInt64 Offset,
            [In] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesWritten);

        UInt64 SearchVirtual(
            [In] UInt64 Offset,
            [In] UInt64 Length,
            [In] IntPtr Pattern,
            [In] UInt32 PatternSize,
            [In] UInt32 PatternGranularity);

        void ReadVirtualUncached(
            [In] UInt64 Offset,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesRead);

        void WriteVirtualUncached(
            [In] UInt64 Offset,
            [In] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesWritten);

        UInt64 ReadPointersVirtual(
            [In] UInt32 Count,
            [In] UInt64 Offset);

        void WritePointersVirtual(
            [In] UInt32 Count,
            [In] UInt64 Offset,
            [In] ref UInt64 Ptrs);

        void ReadPhysical(
            [In] UInt64 Offset,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesRead);

        void WritePhysical(
            [In] UInt64 Offset,
            [In] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesWritten);

        void ReadControl(
            [In] UInt32 Processor,
            [In] UInt64 Offset,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesRead);

        void WriteControl(
            [In] UInt32 Processor,
            [In] UInt64 Offset,
            [In] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesWritten);

        void ReadIo(
            [In] UInt32 InterfaceType,
            [In] UInt32 BusNumber,
            [In] UInt32 AddressSpace,
            [In] UInt64 Offset,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesRead);

        void WriteIo(
            [In] UInt32 InterfaceType,
            [In] UInt32 BusNumber,
            [In] UInt32 AddressSpace,
            [In] UInt64 Offset,
            [In] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesWritten);

        UInt64 ReadMsr(
            [In] UInt32 Msr);

        void WriteMsr(
            [In] UInt32 Msr,
            [In] UInt64 Value);

        void ReadBusData(
            [In] UInt32 BusDataType,
            [In] UInt32 BusNumber,
            [In] UInt32 SlotNumber,
            [In] UInt32 Offset,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesRead);

        void WriteBusData(
            [In] UInt32 BusDataType,
            [In] UInt32 BusNumber,
            [In] UInt32 SlotNumber,
            [In] UInt32 Offset,
            [In] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesWritten);

        void CheckLowMemory();

        void ReadDebuggerData(
            [In] UInt32 Index,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 DataSize);

        void ReadProcessorSystemData(
            [In] UInt32 Processor,
            [In] UInt32 Index,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 DataSize);
    }
    }