using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("E391BBD8-9D8C-4418-840B-C006592A1752"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugSymbols4 : IDebugSymbols3
        {
        void GetScopeEx(
            [Out] out UInt64 InstructionOffset,
            [Out] out DEBUG_STACK_FRAME_EX ScopeFrame,
            [Out] IntPtr ScopeContext = default(IntPtr),
            [In] UInt32 ScopeContextSize = default(UInt32));

        void SetScopeEx(
            [In] UInt64 InstructionOffset,
            [In] ref DEBUG_STACK_FRAME_EX ScopeFrame,
            [In] IntPtr ScopeContext = default(IntPtr),
            [In] UInt32 ScopeContextSize = default(UInt32));

        void GetNameByInlineContext(
            [In] UInt64 Offset,
            [In] UInt32 InlineContext,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize,
            [Out] out UInt64 Displacement);

        void GetNameByInlineContextWide(
            [In] UInt64 Offset,
            [In] UInt32 InlineContext,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize,
            [Out] out UInt64 Displacement);

        void GetLineByInlineContext(
            [In] UInt64 Offset,
            [In] UInt32 InlineContext,
            [Out] out UInt32 Line,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder FileBuffer,
            [In] UInt32 FileBufferSize,
            [Out] out UInt32 FileSize,
            [Out] out UInt64 Displacement);

        void GetLineByInlineContextWide(
            [In] UInt64 Offset,
            [In] UInt32 InlineContext,
            [Out] out UInt32 Line,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder FileBuffer,
            [In] UInt32 FileBufferSize,
            [Out] out UInt32 FileSize,
            [Out] out UInt64 Displacement);

        void OutputSymbolByInlineContext(
            [In] UInt32 OutputControl,
            [In] UInt32 Flags,
            [In] UInt64 Offset,
            [In] UInt32 InlineContext);
        }
    }