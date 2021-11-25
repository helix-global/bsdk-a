using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("D98ADA1F-29E9-4EF5-A6C0-E53349883212"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugDataSpaces4 : IDebugDataSpaces3
        {
        void GetOffsetInformation(
            [In] UInt32 Space,
            [In] UInt32 Which,
            [In] UInt64 Offset,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 InfoSize);

        UInt64 GetNextDifferentlyValidOffsetVirtual(
            [In] UInt64 Offset);

        void GetValidRegionVirtual(
            [In] UInt64 Base,
            [In] UInt32 Size,
            [Out] out UInt64 ValidBase,
            [Out] out UInt32 ValidSize);

        UInt64 SearchVirtual2(
            [In] UInt64 Offset,
            [In] UInt64 Length,
            [In] UInt32 Flags,
            [In] IntPtr Pattern,
            [In] UInt32 PatternSize,
            [In] UInt32 PatternGranularity);

        void ReadMultiByteStringVirtual(
            [In] UInt64 Offset,
            [In] UInt32 MaxBytes,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 StringBytes);

        void ReadMultiByteStringVirtualWide(
            [In] UInt64 Offset,
            [In] UInt32 MaxBytes,
            [In] UInt32 CodePage,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 StringBytes);

        void ReadUnicodeStringVirtual(
            [In] UInt64 Offset,
            [In] UInt32 MaxBytes,
            [In] UInt32 CodePage,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 StringBytes);

        void ReadUnicodeStringVirtualWide(
            [In] UInt64 Offset,
            [In] UInt32 MaxBytes,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 StringBytes);

        void ReadPhysical2(
            [In] UInt64 Offset,
            [In] UInt32 Flags,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesRead);

        void WritePhysical2(
            [In] UInt64 Offset,
            [In] UInt32 Flags,
            [In] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesWritten);
        }
    }