using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("94E60CE9-9B41-4B19-9FC0-6D9EB35272B3"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugControl4 : IDebugControl3
        {
        void GetLogFileWide(
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 FileSize,
            [Out] out Int32 Append);

        void OpenLogFileWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String File,
            [In] Int32 Append);

        void InputWide(
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 InputSize);

        void ReturnInputWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Buffer);

        void OutputWide(
            [In] UInt32 Mask,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Format,
            [In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] params Object[] parameters);

        void OutputVaListWide(
            [In] UInt32 Mask,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Format,
            [In] ref SByte Args);

        void ControlledOutputWide(
            [In] UInt32 OutputControl,
            [In] UInt32 Mask,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Format,
            [In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] params Object[] parameters);

        void ControlledOutputVaListWide(
            [In] UInt32 OutputControl,
            [In] UInt32 Mask,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Format,
            [In] ref SByte Args);

        void OutputPromptWide(
            [In] UInt32 OutputControl,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Format,
            [In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] params Object[] parameters);

        void OutputPromptVaListWide(
            [In] UInt32 OutputControl,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Format,
            [In] ref SByte Args);

        void GetPromptTextWide(
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 TextSize);

        UInt64 AssembleWide(
            [In] UInt64 Offset,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Instr);

        void DisassembleWide(
            [In] Int64 Offset,
            [In] DEBUG_DISASM Flags,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] Int32 BufferSize,
            [Out] out Int32 DisassemblySize,
            [Out] out Int64 EndOffset);

        void GetProcessorTypeNamesWide(
            [In] UInt32 Type,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder FullNameBuffer,
            [In] UInt32 FullNameBufferSize,
            [Out] out UInt32 FullNameSize,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder AbbrevNameBuffer,
            [In] UInt32 AbbrevNameBufferSize,
            [Out] out UInt32 AbbrevNameSize);

        void GetTextMacroWide(
            [In] UInt32 Slot,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 MacroSize);

        void SetTextMacroWide(
            [In] UInt32 Slot,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Macro);

        void EvaluateWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Expression,
            [In] UInt32 DesiredType,
            [Out] out DEBUG_VALUE Value,
            [Out] out UInt32 RemainderIndex);

        void ExecuteWide(
            [In] UInt32 OutputControl,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Command,
            [In] UInt32 Flags);

        void ExecuteCommandFileWide(
            [In] UInt32 OutputControl,
            [In, MarshalAs(UnmanagedType.LPWStr)] String CommandFile,
            [In] UInt32 Flags);

        [return: MarshalAs(UnmanagedType.Interface)]
        IDebugBreakpoint2 GetBreakpointByIndex2(
            [In] UInt32 Index);

        [return: MarshalAs(UnmanagedType.Interface)]
        IDebugBreakpoint2 GetBreakpointById2(
            [In] UInt32 Id);

        [return: MarshalAs(UnmanagedType.Interface)]
        IDebugBreakpoint2 AddBreakpoint2(
            [In] UInt32 Type,
            [In] UInt32 DesiredId);

        void RemoveBreakpoint2(
            [In, MarshalAs(UnmanagedType.Interface)] IDebugBreakpoint2 Bp);

        UInt64 AddExtensionWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Path,
            [In] UInt32 Flags);

        UInt64 GetExtensionByPathWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Path);

        void CallExtensionWide(
            [In] UInt64 Handle,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Function,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Arguments = null);

        IntPtr GetExtensionFunctionWide(
            [In] UInt64 Handle,
            [In, MarshalAs(UnmanagedType.LPWStr)] String FuncName);

        void GetEventFilterTextWide(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 TextSize);

        void GetEventFilterCommandWide(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 CommandSize);

        void SetEventFilterCommandWide(
            [In] UInt32 Index,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Command);

        void GetSpecificFilterArgumentWide(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 ArgumentSize);

        void SetSpecificFilterArgumentWide(
            [In] UInt32 Index,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Argument);

        void GetExceptionFilterSecondCommandWide(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 CommandSize);

        void SetExceptionFilterSecondCommandWide(
            [In] UInt32 Index,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Command);

        void GetLastEventInformationWide(
            [Out] out UInt32 Type,
            [Out] out UInt32 ProcessId,
            [Out] out UInt32 ThreadId,
            [Out] IntPtr ExtraInformation,
            [In] UInt32 ExtraInformationSize,
            [Out] out UInt32 ExtraInformationUsed,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Description,
            [In] UInt32 DescriptionSize,
            [Out] out UInt32 DescriptionUsed);

        void GetTextReplacementWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String SrcText,
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder SrcBuffer,
            [In] UInt32 SrcBufferSize,
            [Out] out UInt32 SrcSize,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder DstBuffer,
            [In] UInt32 DstBufferSize,
            [Out] out UInt32 DstSize);

        void SetTextReplacementWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String SrcText,
            [In, MarshalAs(UnmanagedType.LPWStr)] String DstText = null);

        void SetExpressionSyntaxByNameWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String AbbrevName);

        void GetExpressionSyntaxNamesWide(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder FullNameBuffer,
            [In] UInt32 FullNameBufferSize,
            [Out] out UInt32 FullNameSize,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder AbbrevNameBuffer,
            [In] UInt32 AbbrevNameBufferSize,
            [Out] out UInt32 AbbrevNameSize);

        void GetEventIndexDescriptionWide(
            [In] UInt32 Index,
            [In] UInt32 Which,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 DescSize);

        void GetLogFile2(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 FileSize,
            [Out] out UInt32 Flags);

        void OpenLogFile2(
            [In, MarshalAs(UnmanagedType.LPStr)] String File,
            [In] UInt32 Flags);

        void GetLogFile2Wide(
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 FileSize,
            [Out] out UInt32 Flags);

        void OpenLogFile2Wide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String File,
            [In] UInt32 Flags);

        void GetSystemVersionValues(
            [Out] out UInt32 PlatformId,
            [Out] out UInt32 Win32Major,
            [Out] out UInt32 Win32Minor,
            [Out] out UInt32 KdMajor,
            [Out] out UInt32 KdMinor);

        void GetSystemVersionString(
            [In] UInt32 Which,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 StringSize);

        void GetSystemVersionStringWide(
            [In] UInt32 Which,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 StringSize);

        void GetContextStackTrace(
            [In] IntPtr StartContext,
            [In] UInt32 StartContextSize,
            [Out] IntPtr Frames,
            [In] UInt32 FramesSize,
            [Out] IntPtr FrameContexts,
            [In] UInt32 FrameContextsSize,
            [In] UInt32 FrameContextsEntrySize,
            [Out] out UInt32 FramesFilled);

        void OutputContextStackTrace(
            [In] UInt32 OutputControl,
            [In] IntPtr Frames,
            [In] UInt32 FramesSize,
            [In] IntPtr FrameContexts,
            [In] UInt32 FrameContextsSize,
            [In] UInt32 FrameContextsEntrySize,
            [In] UInt32 Flags);

        void GetStoredEventInformation(
            [Out] out UInt32 Type,
            [Out] out UInt32 ProcessId,
            [Out] out UInt32 ThreadId,
            [Out] IntPtr Context,
            [In] UInt32 ContextSize,
            [Out] out UInt32 ContextUsed,
            [Out] IntPtr ExtraInformation,
            [In] UInt32 ExtraInformationSize,
            [Out] out UInt32 ExtraInformationUsed);

        void GetManagedStatus(
            [Out] out UInt32 Flags,
            [In] UInt32 WhichString,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder String,
            [In] UInt32 StringSize,
            [Out] out UInt32 StringNeeded);

        void GetManagedStatusWide(
            [Out] out UInt32 Flags,
            [In] UInt32 WhichString,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder String,
            [In] UInt32 StringSize,
            [Out] out UInt32 StringNeeded);

        void ResetManagedStatus(
            [In] UInt32 Flags);
        }
    }