using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("6A7CCC5F-FB5E-4DCC-B41C-6C20307BCCC7"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugSymbolGroup2 : IDebugSymbolGroup
        {
        UInt32 AddSymbolWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Name);

        void RemoveSymbolByNameWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Name);

        void GetSymbolNameWide(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 NameSize);

        void WriteSymbolWide(
            [In] UInt32 Index,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Value);

        void OutputAsTypeWide(
            [In] UInt32 Index,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Type);

        void GetSymbolTypeName(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 NameSize);

        void GetSymbolTypeNameWide(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 NameSize);

        UInt32 GetSymbolSize(
            [In] UInt32 Index);

        UInt64 GetSymbolOffset(
            [In] UInt32 Index);

        UInt32 GetSymbolRegister(
            [In] UInt32 Index);

        void GetSymbolValueText(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 NameSize);

        void GetSymbolValueTextWide(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 NameSize);

        DEBUG_SYMBOL_ENTRY GetSymbolEntryInformation(
            [In] UInt32 Index);
        }
    }