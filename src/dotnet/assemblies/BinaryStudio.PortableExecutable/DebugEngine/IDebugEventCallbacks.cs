using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("337BE28B-5036-4D72-B6BF-C45FBB9F2EAA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugEventCallbacks
        {
        UInt32 GetInterestMask();
 
        void Breakpoint(
            [In, MarshalAs(UnmanagedType.Interface)] IDebugBreakpoint Bp);
 
        void Exception(
            [In] ref EXCEPTION_RECORD64 Exception,
            [In] UInt32 FirstChance);
 
        void CreateThread(
            [In] UInt64 Handle,
            [In] UInt64 DataOffset,
            [In] UInt64 StartOffset);
 
        void ExitThread(
            [In] UInt32 ExitCode);
 
        void CreateProcess(
            [In] UInt64 ImageFileHandle,
            [In] UInt64 Handle,
            [In] UInt64 BaseOffset,
            [In] UInt32 ModuleSize,
            [In, MarshalAs(UnmanagedType.LPStr)] String ModuleName = null,
            [In, MarshalAs(UnmanagedType.LPStr)] String ImageName = null,
            [In] UInt32 CheckSum = default(UInt32),
            [In] UInt32 TimeDateStamp = default(UInt32),
            [In] UInt64 InitialThreadHandle = default(UInt64),
            [In] UInt64 ThreadDataOffset = default(UInt64),
            [In] UInt64 StartOffset = default(UInt64));
 
        void ExitProcess(
            [In] UInt32 ExitCode);
 
        void LoadModule(
            [In] UInt64 ImageFileHandle,
            [In] UInt64 BaseOffset,
            [In] UInt32 ModuleSize,
            [In, MarshalAs(UnmanagedType.LPStr)] String ModuleName = null,
            [In, MarshalAs(UnmanagedType.LPStr)] String ImageName = null,
            [In] UInt32 CheckSum = default(UInt32),
            [In] UInt32 TimeDateStamp = default(UInt32));
 
        void UnloadModule(
            [In, MarshalAs(UnmanagedType.LPStr)] String ImageBaseName = null,
            [In] UInt64 BaseOffset = default(UInt64));
 
        void SystemError(
            [In] UInt32 Error,
            [In] UInt32 Level);
 
        void SessionStatus(
            [In] UInt32 Status);
 
        void ChangeDebuggeeState(
            [In] UInt32 Flags,
            [In] UInt64 Argument);
 
        void ChangeEngineState(
            [In] UInt32 Flags,
            [In] UInt64 Argument);
 
        void ChangeSymbolState(
            [In] UInt32 Flags,
            [In] UInt64 Argument);
        }
    }