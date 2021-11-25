using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("716D14C9-119B-4BA5-AF1F-0890E672416A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugAdvanced2 : IDebugAdvanced
        {
        void Request([In] UInt32 Request, [In] IntPtr InBuffer, [In] UInt32 InBufferSize, [Out] IntPtr OutBuffer, [In] UInt32 OutBufferSize, [Out] out UInt32 OutSize);
        void GetSourceFileInformation([In] UInt32 Which, [In, MarshalAs(UnmanagedType.LPStr)] String SourceFile, [In] UInt64 Arg64, [In] UInt32 Arg32, [Out] IntPtr Buffer, [In] UInt32 BufferSize, [Out] out UInt32 InfoSize);
        void FindSourceFileAndToken([In] UInt32 StartElement, [In] UInt64 ModAddr, [In, MarshalAs(UnmanagedType.LPStr)] String File, [In] UInt32 Flags, [In] IntPtr FileToken, [In] UInt32 FileTokenSize, [Out] out UInt32 FoundElement, [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer, [In] UInt32 BufferSize, [Out] out UInt32 FoundSize);
        void GetSymbolInformation([In] UInt32 Which, [In] UInt64 Arg64, [In] UInt32 Arg32, [Out] IntPtr Buffer, [In] UInt32 BufferSize, [Out] out UInt32 InfoSize, [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder StringBuffer, [In] UInt32 StringBufferSize, [Out] out UInt32 StringSize);
        void GetSystemObjectInformation([In] UInt32 Which, [In] UInt64 Arg64, [In] UInt32 Arg32, [Out] IntPtr Buffer, [In] UInt32 BufferSize, [Out] out UInt32 InfoSize);
        }
    }