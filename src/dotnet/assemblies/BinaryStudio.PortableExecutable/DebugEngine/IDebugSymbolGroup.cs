using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("F2528316-0F1A-4431-AEED-11D096E1E2AB"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugSymbolGroup
        {
        UInt32 GetNumberSymbols();

        UInt32 AddSymbol(
            [In, MarshalAs(UnmanagedType.LPStr)] String Name);

        void RemoveSymbolByName(
            [In, MarshalAs(UnmanagedType.LPStr)] String Name);

        void RemoveSymbolByIndex(
            [In] UInt32 Index);

        void GetSymbolName(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 NameSize);

        IntPtr GetSymbolParameters(
            [In] UInt32 Start,
            [In] UInt32 Count);

        void ExpandSymbol(
            [In] UInt32 Index,
            [In] Int32 Expand);

        void OutputSymbols(
            [In] UInt32 OutputControl,
            [In] UInt32 Flags,
            [In] UInt32 Start,
            [In] UInt32 Count);

        void WriteSymbol(
            [In] UInt32 Index,
            [In, MarshalAs(UnmanagedType.LPStr)] String Value);

        void OutputAsType(
            [In] UInt32 Index,
            [In, MarshalAs(UnmanagedType.LPStr)] String Type);
        }
    }