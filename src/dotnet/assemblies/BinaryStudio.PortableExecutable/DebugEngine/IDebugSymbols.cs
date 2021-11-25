using System;
using System.Runtime.InteropServices;
using System.Text;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("8C31E98C-983A-48A5-9016-6FE5D667A950"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugSymbols
        {
        UInt32 GetSymbolOptions();

        void AddSymbolOptions(
            [In] UInt32 Options);

        void RemoveSymbolOptions(
            [In] UInt32 Options);

        void SetSymbolOptions([In] SYMOPT Options);

        void GetNameByOffset(
            [In] UInt64 Offset,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize,
            [Out] out UInt64 Displacement);

        UInt64 GetOffsetByName(
            [In, MarshalAs(UnmanagedType.LPStr)] String Symbol);

        void GetNearNameByOffset(
            [In] UInt64 Offset,
            [In] Int32 Delta,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize,
            [Out] out UInt64 Displacement);

        void GetLineByOffset(
            [In] UInt64 Offset,
            [Out] out UInt32 Line,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder FileBuffer,
            [In] UInt32 FileBufferSize,
            [Out] out UInt32 FileSize,
            [Out] out UInt64 Displacement);

        UInt64 GetOffsetByLine(
            [In] UInt32 Line,
            [In, MarshalAs(UnmanagedType.LPStr)] String File);

        void GetNumberModules(
            [Out] out UInt32 Loaded,
            [Out] out UInt32 Unloaded);

        UInt64 GetModuleByIndex(
            [In] UInt32 Index);

        void GetModuleByModuleName(
            [In, MarshalAs(UnmanagedType.LPStr)] String Name,
            [In] UInt32 StartIndex,
            [Out] out UInt32 Index,
            [Out] out UInt64 Base);

        void GetModuleByOffset(
            [In] UInt64 Offset,
            [In] UInt32 StartIndex,
            [Out] out UInt32 Index,
            [Out] out UInt64 Base);

        void GetModuleNames(
            [In] UInt32 Index,
            [In] UInt64 Base,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder ImageNameBuffer,
            [In] UInt32 ImageNameBufferSize,
            [Out] out UInt32 ImageNameSize,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder ModuleNameBuffer,
            [In] UInt32 ModuleNameBufferSize,
            [Out] out UInt32 ModuleNameSize,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder LoadedImageNameBuffer,
            [In] UInt32 LoadedImageNameBufferSize,
            [Out] out UInt32 LoadedImageNameSize);

        void GetModuleParameters(
            [In] UInt32 Count,
            [In] ref UInt64 Bases,
            [In] UInt32 Start = default(UInt32),
            [Out] IntPtr Params = default(IntPtr));

        UInt64 GetSymbolModule(
            [In, MarshalAs(UnmanagedType.LPStr)] String Symbol);

        void GetTypeName(
            [In] UInt64 Module,
            [In] UInt32 TypeId,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize);

        UInt32 GetTypeId(
            [In] UInt64 Module,
            [In, MarshalAs(UnmanagedType.LPStr)] String Name);

        UInt32 GetTypeSize(
            [In] UInt64 Module,
            [In] UInt32 TypeId);

        UInt32 GetFieldOffset(
            [In] UInt64 Module,
            [In] UInt32 TypeId,
            [In, MarshalAs(UnmanagedType.LPStr)] String Field);

        void GetSymbolTypeId(
            [In, MarshalAs(UnmanagedType.LPStr)] String Symbol,
            [Out] out UInt32 TypeId,
            [Out] out UInt64 Module);

        void GetOffsetTypeId(
            [In] UInt64 Offset,
            [Out] out UInt32 TypeId,
            [Out] out UInt64 Module);

        void ReadTypedDataVirtual(
            [In] UInt64 Offset,
            [In] UInt64 Module,
            [In] UInt32 TypeId,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesRead);

        void WriteTypedDataVirtual(
            [In] UInt64 Offset,
            [In] UInt64 Module,
            [In] UInt32 TypeId,
            [In] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesWritten);

        void OutputTypedDataVirtual(
            [In] UInt32 OutputControl,
            [In] UInt64 Offset,
            [In] UInt64 Module,
            [In] UInt32 TypeId,
            [In] UInt32 Flags);

        void ReadTypedDataPhysical(
            [In] UInt64 Offset,
            [In] UInt64 Module,
            [In] UInt32 TypeId,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesRead);

        void WriteTypedDataPhysical(
            [In] UInt64 Offset,
            [In] UInt64 Module,
            [In] UInt32 TypeId,
            [In] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BytesWritten);

        void OutputTypedDataPhysical(
            [In] UInt32 OutputControl,
            [In] UInt64 Offset,
            [In] UInt64 Module,
            [In] UInt32 TypeId,
            [In] UInt32 Flags);

        void GetScope(
            [Out] out UInt64 InstructionOffset,
            [Out] out DEBUG_STACK_FRAME ScopeFrame,
            [Out] IntPtr ScopeContext = default(IntPtr),
            [In] UInt32 ScopeContextSize = default(UInt32));

        void SetScope(
            [In] UInt64 InstructionOffset,
            [In] ref DEBUG_STACK_FRAME ScopeFrame,
            [In] IntPtr ScopeContext = default(IntPtr),
            [In] UInt32 ScopeContextSize = default(UInt32));

        void ResetScope();

        void GetScopeSymbolGroup(
            [In] UInt32 Flags,
            [In, MarshalAs(UnmanagedType.Interface)] IDebugSymbolGroup Update,
            [Out, MarshalAs(UnmanagedType.Interface)] out IDebugSymbolGroup Symbols);

        [return: MarshalAs(UnmanagedType.Interface)]
        IDebugSymbolGroup CreateSymbolGroup();

        UInt64 StartSymbolMatch(
            [In, MarshalAs(UnmanagedType.LPStr)] String Pattern);

        void GetNextSymbolMatch(
            [In] UInt64 Handle,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize, [Out] out UInt32 MatchSize,
            [Out] out UInt64 Offset);

        void EndSymbolMatch(
            [In] UInt64 Handle);

        void Reload(
            [In, MarshalAs(UnmanagedType.LPStr)] String Module);

        void GetSymbolPath(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 PathSize);

        void SetSymbolPath(
            [In, MarshalAs(UnmanagedType.LPStr)] String Path);

        void AppendSymbolPath(
            [In, MarshalAs(UnmanagedType.LPStr)] String Addition);

        void GetImagePath(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 PathSize);

        void SetImagePath(
            [In, MarshalAs(UnmanagedType.LPStr)] String Path);

        void AppendImagePath(
            [In, MarshalAs(UnmanagedType.LPStr)] String Addition);

        void GetSourcePath(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 PathSize);

        void GetSourcePathElement(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 ElementSize);

        void SetSourcePath(
            [In, MarshalAs(UnmanagedType.LPStr)] String Path);

        void AppendSourcePath(
            [In, MarshalAs(UnmanagedType.LPStr)] String Addition);

        void FindSourceFile(
            [In] UInt32 StartElement,
            [In, MarshalAs(UnmanagedType.LPStr)] String File,
            [In] UInt32 Flags,
            [Out] out UInt32 FoundElement,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 FoundSize);

        void GetSourceFileLineOffsets(
            [In, MarshalAs(UnmanagedType.LPStr)] String File,
            [Out] out UInt64 Buffer,
            [In] UInt32 BufferLines,
            [Out] out UInt32 FileLines);
        }
    }