using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("B2FFE162-2412-429F-8D1D-5BF6DD824696"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugControl5 : IDebugControl4
        {
        void GetStackTraceEx(
            [In] UInt64 FrameOffset,
            [In] UInt64 StackOffset,
            [In] UInt64 InstructionOffset,
            [Out] IntPtr Frames,
            [In] UInt32 FramesSize,
            [Out] out UInt32 FramesFilled);

        void OutputStackTraceEx(
            [In] UInt32 OutputControl,
            [In] IntPtr Frames = default(IntPtr),
            [In] UInt32 FramesSize = default(UInt32),
            [In] UInt32 Flags = default(UInt32));

        void GetContextStackTraceEx(
            [In] IntPtr StartContext,
            [In] UInt32 StartContextSize,
            [Out] IntPtr Frames,
            [In] UInt32 FramesSize,
            [Out] IntPtr FrameContexts,
            [In] UInt32 FrameContextsSize,
            [In] UInt32 FrameContextsEntrySize,
            [Out] out UInt32 FramesFilled);

        void OutputContextStackTraceEx(
            [In] UInt32 OutputControl,
            [In] IntPtr Frames,
            [In] UInt32 FramesSize,
            [In] IntPtr FrameContexts,
            [In] UInt32 FrameContextsSize,
            [In] UInt32 FrameContextsEntrySize,
            [In] UInt32 Flags);

        [return: MarshalAs(UnmanagedType.Interface)]
        IDebugBreakpoint3 GetBreakpointByGuid(
            [In] ref Guid Guid);
        }
    }