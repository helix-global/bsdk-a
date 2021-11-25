using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("D4366723-44DF-4BED-8C7E-4C05424F4588"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugControl2 : IDebugControl
        {
        UInt32 GetCurrentTimeDate();

        UInt32 GetCurrentSystemUpTime();

        UInt32 GetDumpFormatFlags();

        UInt32 GetNumberTextReplacements();

        void GetTextReplacement(
            [In, MarshalAs(UnmanagedType.LPStr)] String SrcText,
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder SrcBuffer,
            [In] UInt32 SrcBufferSize,
            [Out] out UInt32 SrcSize,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder DstBuffer,
            [In] UInt32 DstBufferSize,
            [Out] out UInt32 DstSize);

        void SetTextReplacement(
            [In, MarshalAs(UnmanagedType.LPStr)] String SrcText,
            [In, MarshalAs(UnmanagedType.LPStr)] String DstText);

        void RemoveTextReplacements();

        void OutputTextReplacements(
            [In] UInt32 OutputControl,
            [In] UInt32 Flags);
        }
    }