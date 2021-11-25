using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("5BD9D474-5975-423A-B88B-65A8E7110E65"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugBreakpoint
        {
        UInt32 GetId();
 
        void GetType(
            [Out] out UInt32 BreakType,
            [Out] out UInt32 ProcType);
 
        [return: MarshalAs(UnmanagedType.Interface)]
        IDebugClient GetAdder();
 
        UInt32 GetFlags();
 
        void AddFlags(
            [In] UInt32 Flags);
 
        void RemoveFlags(
            [In] UInt32 Flags);
 
        void SetFlags(
            [In] UInt32 Flags);
 
        UInt64 GetOffset();
 
        void SetOffset(
            [In] UInt64 Offset);
 
        void GetDataParameters(
            [Out] out UInt32 Size,
            [Out] out UInt32 AccessType);
 
        void SetDataParameters(
            [In] UInt32 Size,
            [In] UInt32 AccessType);
 
        UInt32 GetPassCount();
 
        void SetPassCount(
            [In] UInt32 Count);
 
        UInt32 GetCurrentPassCount();
 
        UInt32 GetMatchThreadId();
 
        void SetMatchThreadId(
            [In] UInt32 Thread);
 
        void GetCommand(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 CommandSize);
 
        void SetCommand(
            [In, MarshalAs(UnmanagedType.LPStr)] String Command);
 
        void GetOffsetExpression(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 ExpressionSize);
 
        void SetOffsetExpression(
            [In, MarshalAs(UnmanagedType.LPStr)] String Expression);
 
        DEBUG_BREAKPOINT_PARAMETERS GetParameters();
        }
    }