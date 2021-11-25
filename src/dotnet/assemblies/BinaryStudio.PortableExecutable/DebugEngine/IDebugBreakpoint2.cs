using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("1B278D20-79F2-426E-A3F9-C1DDF375D48E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugBreakpoint2 : IDebugBreakpoint
        {
        void GetCommandWide(
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 CommandSize);

        void SetCommandWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Command);

        void GetOffsetExpressionWide(
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 ExpressionSize);

        void SetOffsetExpressionWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Expression);
        }
    }