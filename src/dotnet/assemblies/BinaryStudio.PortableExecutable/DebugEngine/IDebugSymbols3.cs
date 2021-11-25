using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("F02FBECC-50AC-4F36-9AD9-C975E8F32FF8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugSymbols3 : IDebugSymbols2
    {
        void GetNameByOffsetWide(
            [In] UInt64 Offset,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize,
            [Out] out UInt64 Displacement);

        UInt64 GetOffsetByNameWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Symbol);

        void GetNearNameByOffsetWide(
            [In] UInt64 Offset,
            [In] Int32 Delta,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize,
            [Out] out UInt64 Displacement);

        void GetLineByOffsetWide(
            [In] UInt64 Offset,
            [Out] out UInt32 Line,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder FileBuffer,
            [In] UInt32 FileBufferSize,
            [Out] out UInt32 FileSize,
            [Out] out UInt64 Displacement);

        UInt64 GetOffsetByLineWide(
            [In] UInt32 Line,
            [In, MarshalAs(UnmanagedType.LPWStr)] String File);

        void GetModuleByModuleNameWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Name,
            [In] UInt32 StartIndex,
            [Out] out UInt32 Index,
            [Out] out UInt64 Base);

        UInt64 GetSymbolModuleWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Symbol);

        void GetTypeNameWide(
            [In] UInt64 Module,
            [In] UInt32 TypeId,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize);

        UInt32 GetTypeIdWide(
            [In] UInt64 Module,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Name);

        UInt32 GetFieldOffsetWide(
            [In] UInt64 Module,
            [In] UInt32 TypeId,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Field);

        void GetSymbolTypeIdWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Symbol,
            [Out] out UInt32 TypeId,
            [Out] out UInt64 Module);

        void GetScopeSymbolGroup2(
            [In] UInt32 Flags,
            [In, MarshalAs(UnmanagedType.Interface)] IDebugSymbolGroup2 Update,
            [Out, MarshalAs(UnmanagedType.Interface)] out IDebugSymbolGroup2 Symbols);

        [return: MarshalAs(UnmanagedType.Interface)]
        IDebugSymbolGroup2 CreateSymbolGroup2();

        UInt64 StartSymbolMatchWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Pattern);

        void GetNextSymbolMatchWide(
            [In] UInt64 Handle,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 MatchSize,
            [Out] out UInt64 Offset);

        void ReloadWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Module);

        void GetSymbolPathWide(
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 PathSize);

        void SetSymbolPathWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Path);

        void AppendSymbolPathWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Addition);

        void GetImagePathWide(
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 PathSize);

        void SetImagePathWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Path);

        void AppendImagePathWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Addition);

        void GetSourcePathWide(
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 PathSize);

        void GetSourcePathElementWide(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 ElementSize);

        void SetSourcePathWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Path);

        void AppendSourcePathWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Addition);

        void FindSourceFileWide(
            [In] UInt32 StartElement,
            [In, MarshalAs(UnmanagedType.LPWStr)] String File,
            [In] UInt32 Flags,
            [Out] out UInt32 FoundElement,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 FoundSize);

        void GetSourceFileLineOffsetsWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String File,
            [Out] out UInt64 Buffer,
            [In] UInt32 BufferLines,
            [Out] out UInt32 FileLines);

        void GetModuleVersionInformationWide(
            [In] UInt32 Index,
            [In] UInt64 Base,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Item,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 VerInfoSize);

        void GetModuleNameStringWide(
            [In] UInt32 Which,
            [In] UInt32 Index,
            [In] UInt64 Base,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 NameSize);

        void GetConstantNameWide(
            [In] UInt64 Module,
            [In] UInt32 TypeId,
            [In] UInt64 Value,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize);

        void GetFieldNameWide(
            [In] UInt64 Module,
            [In] UInt32 TypeId,
            [In] UInt32 FieldIndex,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize);

        void IsManagedModule(
            [In] UInt32 Index,
            [In] UInt64 Base);

        void GetModuleByModuleName2(
            [In, MarshalAs(UnmanagedType.LPStr)] String Name,
            [In] UInt32 StartIndex,
            [In] UInt32 Flags,
            [Out] out UInt32 Index,
            [Out] out UInt64 Base);

        void GetModuleByModuleName2Wide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Name,
            [In] UInt32 StartIndex,
            [In] UInt32 Flags,
            [Out] out UInt32 Index,
            [Out] out UInt64 Base);

        void GetModuleByOffset2(
            [In] UInt64 Offset,
            [In] UInt32 StartIndex,
            [In] UInt32 Flags,
            [Out] out UInt32 Index,
            [Out] out UInt64 Base);

        void AddSyntheticModule(
            [In] UInt64 Base,
            [In] UInt32 Size,
            [In, MarshalAs(UnmanagedType.LPStr)] String ImagePath,
            [In, MarshalAs(UnmanagedType.LPStr)] String ModuleName,
            [In] UInt32 Flags);

        void AddSyntheticModuleWide(
            [In] UInt64 Base,
            [In] UInt32 Size,
            [In, MarshalAs(UnmanagedType.LPWStr)] String ImagePath,
            [In, MarshalAs(UnmanagedType.LPWStr)] String ModuleName,
            [In] UInt32 Flags);

        void RemoveSyntheticModule(
            [In] UInt64 Base);

        UInt32 GetCurrentScopeFrameIndex();

        void SetScopeFrameByIndex(
            [In] UInt32 Index);

        void SetScopeFromJitDebugInfo(
            [In] UInt32 OutputControl,
            [In] UInt64 InfoOffset);

        void SetScopeFromStoredEvent();

        void OutputSymbolByOffset(
            [In] UInt32 OutputControl,
            [In] UInt32 Flags,
            [In] UInt64 Offset);

        void GetFunctionEntryByOffset(
            [In] UInt64 Offset,
            [In] UInt32 Flags,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 BufferNeeded);

        void GetFieldTypeAndOffset(
            [In] UInt64 Module,
            [In] UInt32 ContainerTypeId,
            [In, MarshalAs(UnmanagedType.LPStr)] String Field,
            [Out] out UInt32 FieldTypeId,
            [Out] out UInt32 Offset);

        void GetFieldTypeAndOffsetWide(
            [In] UInt64 Module,
            [In] UInt32 ContainerTypeId,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Field,
            [Out] out UInt32 FieldTypeId,
            [Out] out UInt32 Offset);

        void AddSyntheticSymbol(
            [In] UInt64 Offset,
            [In] UInt32 Size,
            [In, MarshalAs(UnmanagedType.LPStr)] String Name,
            [In] UInt32 Flags,
            [Out] out DEBUG_MODULE_AND_ID Id);

        void AddSyntheticSymbolWide(
            [In] UInt64 Offset,
            [In] UInt32 Size,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Name,
            [In] UInt32 Flags,
            [Out] out DEBUG_MODULE_AND_ID Id);

        void RemoveSyntheticSymbol(
            [In] ref DEBUG_MODULE_AND_ID Id);

        void GetSymbolEntriesByOffset(
            [In] UInt64 Offset,
            [In] UInt32 Flags,
            [Out] IntPtr Ids,
            [Out] out UInt64 Displacements,
            [In] UInt32 IdsCount,
            [Out] out UInt32 Entries);

        void GetSymbolEntriesByName(
            [In, MarshalAs(UnmanagedType.LPStr)] String Symbol,
            [In] UInt32 Flags,
            [Out] IntPtr Ids,
            [In] UInt32 IdsCount,
            [Out] out UInt32 Entries);

        void GetSymbolEntriesByNameWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Symbol,
            [In] UInt32 Flags,
            [Out] IntPtr Ids,
            [In] UInt32 IdsCount,
            [Out] out UInt32 Entries);

        DEBUG_MODULE_AND_ID GetSymbolEntryByToken(
            [In] UInt64 ModuleBase,
            [In] UInt32 Token);

        DEBUG_SYMBOL_ENTRY GetSymbolEntryInformation(
            [In] ref DEBUG_MODULE_AND_ID Id);

        void GetSymbolEntryString(
            [In] ref DEBUG_MODULE_AND_ID Id,
            [In] UInt32 Which,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 StringSize);

        void GetSymbolEntryStringWide(
            [In] ref DEBUG_MODULE_AND_ID Id,
            [In] UInt32 Which,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 StringSize);

        void GetSymbolEntryOffsetRegions(
            [In] ref DEBUG_MODULE_AND_ID Id,
            [In] UInt32 Flags,
            [Out] IntPtr Regions,
            [In] UInt32 RegionsCount,
            [Out] out UInt32 RegionsAvail);

        DEBUG_MODULE_AND_ID GetSymbolEntryBySymbolEntry(
            [In] ref DEBUG_MODULE_AND_ID FromId,
            [In] UInt32 Flags);

        void GetSourceEntriesByOffset(
            [In] UInt64 Offset,
            [In] UInt32 Flags,
            [Out] IntPtr Entries,
            [In] UInt32 EntriesCount,
            [Out] out UInt32 EntriesAvail);

        void GetSourceEntriesByLine(
            [In] UInt32 Line,
            [In, MarshalAs(UnmanagedType.LPStr)] String File,
            [In] UInt32 Flags,
            [Out] IntPtr Entries,
            [In] UInt32 EntriesCount,
            [Out] out UInt32 EntriesAvail);

        void GetSourceEntriesByLineWide(
            [In] UInt32 Line,
            [In, MarshalAs(UnmanagedType.LPWStr)] String File,
            [In] UInt32 Flags,
            [Out] IntPtr Entries,
            [In] UInt32 EntriesCount,
            [Out] out UInt32 EntriesAvail);

        void GetSourceEntryString(
            [In] ref DEBUG_SYMBOL_SOURCE_ENTRY Entry,
            [In] UInt32 Which,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 StringSize);

        void GetSourceEntryStringWide(
            [In] ref DEBUG_SYMBOL_SOURCE_ENTRY Entry,
            [In] UInt32 Which,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 StringSize);

        void GetSourceEntryOffsetRegions(
            [In] ref DEBUG_SYMBOL_SOURCE_ENTRY Entry,
            [In] UInt32 Flags,
            [Out] IntPtr Regions,
            [In] UInt32 RegionsCount,
            [Out] out UInt32 RegionsAvail);

        DEBUG_SYMBOL_SOURCE_ENTRY GetSourceEntryBySourceEntry(
            [In] ref DEBUG_SYMBOL_SOURCE_ENTRY FromEntry,
            [In] UInt32 Flags);
        }
    }