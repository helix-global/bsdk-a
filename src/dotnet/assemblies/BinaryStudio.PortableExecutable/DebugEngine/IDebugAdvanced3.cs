using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("CBA4ABB4-84C4-444D-87CA-A04E13286739"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugAdvanced3 : IDebugAdvanced2
        {
        void GetSourceFileInformationWide([In] UInt32 Which, [In, MarshalAs(UnmanagedType.LPWStr)] String SourceFile, [In] UInt64 Arg64, [In] UInt32 Arg32, [Out] IntPtr Buffer, [In] UInt32 BufferSize, [Out] out UInt32 InfoSize);
        void FindSourceFileAndTokenWide([In] UInt32 StartElement, [In] UInt64 ModAddr, [In, MarshalAs(UnmanagedType.LPWStr)] String File, [In] UInt32 Flags, [In] IntPtr FileToken, [In] UInt32 FileTokenSize, [Out] out UInt32 FoundElement, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer, [In] UInt32 BufferSize, [Out] out UInt32 FoundSize);
        void GetSymbolInformationWide([In] UInt32 Which, [In] UInt64 Arg64, [In] UInt32 Arg32, [Out] IntPtr Buffer, [In] UInt32 BufferSize, [Out] out UInt32 InfoSize, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder StringBuffer, [In] UInt32 StringBufferSize, [Out] out UInt32 StringSize);
        }
    }