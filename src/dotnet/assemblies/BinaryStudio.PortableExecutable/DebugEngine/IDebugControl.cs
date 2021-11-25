using System;
using System.Runtime.InteropServices;
using System.Text;
using BinaryStudio.PlatformComponents.Win32;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("5182E668-105E-416E-AD92-24EF800424BA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugControl
        {
        void GetInterrupt();
 
        void SetInterrupt(
            [In] UInt32 Flags);
 
        UInt32 GetInterruptTimeout();
 
        void SetInterruptTimeout(
            [In] UInt32 Seconds);
 
        void GetLogFile(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 FileSize,
            [Out] out Int32 Append);
 
        void OpenLogFile(
            [In, MarshalAs(UnmanagedType.LPStr)] String File,
            [In] Int32 Append);
 
        void CloseLogFile();
 
        UInt32 GetLogMask();
 
        void SetLogMask(
            [In] UInt32 Mask);
 
        void Input(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 InputSize);
 
        void ReturnInput(
            [In, MarshalAs(UnmanagedType.LPStr)] String Buffer);
 
        void Output(
            [In] UInt32 Mask,
            [In, MarshalAs(UnmanagedType.LPStr)] String Format,
            [In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] params Object[] parameters);
 
        void OutputVaList(
            [In] UInt32 Mask,
            [In, MarshalAs(UnmanagedType.LPStr)] String Format,
            [In] ref SByte Args);
 
        void ControlledOutput(
            [In] UInt32 OutputControl,
            [In] UInt32 Mask,
            [In, MarshalAs(UnmanagedType.LPStr)] String Format,
            [In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] params Object[] parameters);
 
        void ControlledOutputVaList(
            [In] UInt32 OutputControl,
            [In] UInt32 Mask,
            [In, MarshalAs(UnmanagedType.LPStr)] String Format,
            [In] ref SByte Args);
 
        void OutputPrompt(
            [In] UInt32 OutputControl,
            [In, MarshalAs(UnmanagedType.LPStr)] String Format,
            [In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] params Object[] parameters);
 
        void OutputPromptVaList(
            [In] UInt32 OutputControl,
            [In, MarshalAs(UnmanagedType.LPStr)] String Format,
            [In] ref SByte Args);
 
        void GetPromptText(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 TextSize);
 
        void OutputCurrentState(
            [In] UInt32 OutputControl,
            [In] UInt32 Flags);
 
        void OutputVersionInformation(
            [In] UInt32 OutputControl);
 
        UInt64 GetNotifyEventHandle();
 
        void SetNotifyEventHandle(
            [In] UInt64 Handle);
 
        UInt64 Assemble(
            [In] UInt64 Offset,
            [In, MarshalAs(UnmanagedType.LPStr)] String Instr);
 
        /// <summary>
        /// The <see cref="Disassemble"/> method disassembles a processor instruction in the target's memory.
        /// </summary>
        /// <param name="Offset">Specifies the location in the target's memory of the instruction to disassemble.</param>
        /// <param name="Flags">Specifies the bit-flags that affect the behavior of this method.</param>
        /// <param name="Buffer">Receives the disassembled instruction. If <paramref name="Buffer"/> is NULL, this information is not returned.</param>
        /// <param name="BufferSize">Specifies the size, in characters, of the <paramref name="Buffer"/> buffer.</param>
        /// <param name="DisassemblySize">Receives the size, in characters, of the disassembled instruction. If <paramref name="DisassemblySize"/> is NULL, this information is not returned.</param>
        /// <param name="EndOffset">Receives the location in the target's memory of the instruction following the disassembled instruction.</param>
        /// <returns>This method can also return error values.</returns>
        [PreserveSig] HRESULT Disassemble(
            [In] Int64 Offset,
            [In] DEBUG_DISASM Flags,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] Int32 BufferSize,
            [Out] out Int32 DisassemblySize,
            [Out] out Int64 EndOffset);
 
        UInt64 GetDisassembleEffectiveOffset();
 
        UInt64 OutputDisassembly(
            [In] UInt32 OutputControl,
            [In] UInt64 Offset,
            [In] UInt32 Flags);
 
        void OutputDisassemblyLines(
            [In] UInt32 OutputControl,
            [In] UInt32 PreviousLines,
            [In] UInt32 TotalLines,
            [In] UInt64 Offset,
            [In] UInt32 Flags,
            [Out] out UInt32 OffsetLine,
            [Out] out UInt64 StartOffset,
            [Out] out UInt64 EndOffset,
            [Out] out UInt64 LineOffsets);
 
        UInt64 GetNearInstruction(
            [In] UInt64 Offset,
            [In] Int32 Delta);
 
        void GetStackTrace(
            [In] UInt64 FrameOffset,
            [In] UInt64 StackOffset,
            [In] UInt64 InstructionOffset,
            [Out] IntPtr Frames,
            [In] UInt32 FramesSize,
            [Out] out UInt32 FramesFilled);
 
        UInt64 GetReturnOffset();
 
        void OutputStackTrace(
            [In] UInt32 OutputControl,
            [In] IntPtr Frames = default(IntPtr),
            [In] UInt32 FramesSize = default(UInt32),
            [In] UInt32 Flags = default(UInt32));
 
        void GetDebuggeeType(
            [Out] out UInt32 Class,
            [Out] out UInt32 Qualifier);
 
        UInt32 GetActualProcessorType();
 
        UInt32 GetExecutingProcessorType();
 
        UInt32 GetNumberPossibleExecutingProcessorTypes();
 
        UInt32 GetPossibleExecutingProcessorTypes(
            [In] UInt32 Start,
            [In] UInt32 Count);
 
        UInt32 GetNumberProcessors();
 
        void GetSystemVersion(
            [Out] out UInt32 PlatformId,
            [Out] out UInt32 Major,
            [Out] out UInt32 Minor,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder ServicePackString,
            [In] UInt32 ServicePackStringSize,
            [Out] out UInt32 ServicePackStringUsed,
            [Out] out UInt32 ServicePackNumber,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder BuildString,
            [In] UInt32 BuildStringSize,
            [Out] out UInt32 BuildStringUsed);
 
        UInt32 GetPageSize();
 
        void IsPointer64Bit();
 
        void ReadBugCheckData(
            [Out] out UInt32 Code,
            [Out] out UInt64 Arg1,
            [Out] out UInt64 Arg2,
            [Out] out UInt64 Arg3,
            [Out] out UInt64 Arg4);
 
        UInt32 GetNumberSupportedProcessorTypes();
 
        UInt32 GetSupportedProcessorTypes(
            [In] UInt32 Start,
            [In] UInt32 Count);
 
        void GetProcessorTypeNames(
            [In] UInt32 Type,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder FullNameBuffer,
            [In] UInt32 FullNameBufferSize,
            [Out] out UInt32 FullNameSize,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder AbbrevNameBuffer,
            [In] UInt32 AbbrevNameBufferSize,
            [Out] out UInt32 AbbrevNameSize);
 
        UInt32 GetEffectiveProcessorType();
 
        void SetEffectiveProcessorType(
            [In] UInt32 Type);
 
        UInt32 GetExecutionStatus();
 
        void SetExecutionStatus(
            [In] UInt32 Status);
 
        UInt32 GetCodeLevel();
 
        void SetCodeLevel(
            [In] UInt32 Level);
 
        UInt32 GetEngineOptions();
 
        void AddEngineOptions(
            [In] UInt32 Options);
 
        void RemoveEngineOptions(
            [In] UInt32 Options);
 
        void SetEngineOptions(
            [In] UInt32 Options);
 
        void GetSystemErrorControl(
            [Out] out UInt32 OutputLevel,
            [Out] out UInt32 BreakLevel);
 
        void SetSystemErrorControl(
            [In] UInt32 OutputLevel,
            [In] UInt32 BreakLevel);
 
        void GetTextMacro(
            [In] UInt32 Slot,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 MacroSize);
 
        void SetTextMacro(
            [In] UInt32 Slot,
            [In, MarshalAs(UnmanagedType.LPStr)] String Macro);
 
        UInt32 GetRadix();
 
        void SetRadix(
            [In] UInt32 Radix);
 
        void Evaluate(
            [In, MarshalAs(UnmanagedType.LPStr)] String Expression,
            [In] UInt32 DesiredType,
            [Out] out DEBUG_VALUE Value,
            [Out] out UInt32 RemainderIndex);
 
        DEBUG_VALUE CoerceValue(
            [In] ref DEBUG_VALUE In,
            [In] UInt32 OutType);
 
        IntPtr CoerceValues(
            [In] UInt32 Count,
            [In] IntPtr In,
            [In] ref UInt32 OutTypes);
 
        void Execute(
            [In] UInt32 OutputControl,
            [In, MarshalAs(UnmanagedType.LPStr)] String Command,
            [In] UInt32 Flags);
 
        void ExecuteCommandFile(
            [In] UInt32 OutputControl,
            [In, MarshalAs(UnmanagedType.LPStr)] String CommandFile,
            [In] UInt32 Flags);
 
        UInt32 GetNumberBreakpoints();
 
        [return: MarshalAs(UnmanagedType.Interface)]
        IDebugBreakpoint GetBreakpointByIndex(
            [In] UInt32 Index);
 
        [return: MarshalAs(UnmanagedType.Interface)]
        IDebugBreakpoint GetBreakpointById(
            [In] UInt32 Id);
 
        void GetBreakpointParameters(
            [In] UInt32 Count,
            [In] ref UInt32 Ids,
            [In] UInt32 Start = default(UInt32),
            [Out] IntPtr Params = default(IntPtr));
 
        [return: MarshalAs(UnmanagedType.Interface)]
        IDebugBreakpoint AddBreakpoint(
            [In] UInt32 Type,
            [In] UInt32 DesiredId);
 
        void RemoveBreakpoint(
            [In, MarshalAs(UnmanagedType.Interface)] IDebugBreakpoint Bp);
 
        UInt64 AddExtension(
            [In, MarshalAs(UnmanagedType.LPStr)] String Path,
            [In] UInt32 Flags);
 
        void RemoveExtension(
            [In] UInt64 Handle);
 
        UInt64 GetExtensionByPath(
            [In, MarshalAs(UnmanagedType.LPStr)] String Path);
 
        void CallExtension(
            [In] UInt64 Handle,
            [In, MarshalAs(UnmanagedType.LPStr)] String Function,
            [In, MarshalAs(UnmanagedType.LPStr)] String Arguments = null);
 
        IntPtr GetExtensionFunction(
            [In] UInt64 Handle,
            [In, MarshalAs(UnmanagedType.LPStr)] String FuncName);
 
        WINDBG_EXTENSION_APIS32 GetWindbgExtensionApis32();
 
        WINDBG_EXTENSION_APIS64 GetWindbgExtensionApis64();
 
        void GetNumberEventFilters(
            [Out] out UInt32 SpecificEvents,
            [Out] out UInt32 SpecificExceptions,
            [Out] out UInt32 ArbitraryExceptions);
 
        void GetEventFilterText(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 TextSize);
 
        void GetEventFilterCommand(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 CommandSize);
 
        void SetEventFilterCommand(
            [In] UInt32 Index,
            [In, MarshalAs(UnmanagedType.LPStr)] String Command);
 
        IntPtr GetSpecificFilterParameters(
            [In] UInt32 Start,
            [In] UInt32 Count);
 
        void SetSpecificFilterParameters(
            [In] UInt32 Start,
            [In] UInt32 Count,
            [In] IntPtr Params);
 
        void GetSpecificFilterArgument(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 ArgumentSize);
 
        void SetSpecificFilterArgument(
            [In] UInt32 Index,
            [In, MarshalAs(UnmanagedType.LPStr)] String Argument);
 
        void GetExceptionFilterParameters(
            [In] UInt32 Count,
            [In] ref UInt32 Codes,
            [In] UInt32 Start = default(UInt32),
            [Out] IntPtr Params = default(IntPtr));
 
        void SetExceptionFilterParameters(
            [In] UInt32 Count,
            [In] IntPtr Params);
 
        void GetExceptionFilterSecondCommand(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 CommandSize);
 
        void SetExceptionFilterSecondCommand(
            [In] UInt32 Index,
            [In, MarshalAs(UnmanagedType.LPStr)] String Command);
 
        void WaitForEvent(
            [In] Int32 Flags,
            [In] Int32 Timeout);
 
        void GetLastEventInformation(
            [Out] out UInt32 Type,
            [Out] out UInt32 ProcessId,
            [Out] out UInt32 ThreadId,
            [Out] IntPtr ExtraInformation,
            [In] UInt32 ExtraInformationSize,
            [Out] out UInt32 ExtraInformationUsed,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Description,
            [In] UInt32 DescriptionSize,
            [Out] out UInt32 DescriptionUsed);
        }
    }