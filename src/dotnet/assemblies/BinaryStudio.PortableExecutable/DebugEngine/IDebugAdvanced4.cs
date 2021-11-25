using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("D1069067-2A65-4BF0-AE97-76184B67856B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugAdvanced4 : IDebugAdvanced3
        {
        void GetSymbolInformationWideEx([In] UInt32 Which, [In] UInt64 Arg64, [In] UInt32 Arg32, [Out] IntPtr Buffer, [In] UInt32 BufferSize, [Out] out UInt32 InfoSize, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder StringBuffer, [In] UInt32 StringBufferSize, [Out] out UInt32 StringSize, [Out] out SYMBOL_INFO_EX pInfoEx);
        }
    }