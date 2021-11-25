using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("6B86FE2C-2C4F-4F0C-9DA2-174311ACC327"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugSystemObjects
    {
        UInt32 GetEventThread();

        UInt32 GetEventProcess();

        UInt32 GetCurrentThreadId();

        void SetCurrentThreadId(
            [In] UInt32 Id);

        UInt32 GetCurrentProcessId();

        void SetCurrentProcessId(
            [In] UInt32 Id);

        UInt32 GetNumberThreads();

        void GetTotalNumberThreads(
            [Out] out UInt32 Total,
            [Out] out UInt32 LargestProcess);

        void GetThreadIdsByIndex(
            [In] UInt32 Start,
            [In] UInt32 Count,
            [Out] out UInt32 Ids,
            [Out] out UInt32 SysIds);

        UInt32 GetThreadIdByProcessor(
            [In] UInt32 Processor);

        UInt64 GetCurrentThreadDataOffset();

        UInt32 GetThreadIdByDataOffset(
            [In] UInt64 Offset);

        UInt64 GetCurrentThreadTeb();

        UInt32 GetThreadIdByTeb(
            [In] UInt64 Offset);

        UInt32 GetCurrentThreadSystemId();

        UInt32 GetThreadIdBySystemId(
            [In] UInt32 SysId);

        UInt64 GetCurrentThreadHandle();

        UInt32 GetThreadIdByHandle(
            [In] UInt64 Handle);

        UInt32 GetNumberProcesses();

        void GetProcessIdsByIndex(
            [In] UInt32 Start,
            [In] UInt32 Count,
            [Out] out UInt32 Ids,
            [Out] out UInt32 SysIds);

        UInt64 GetCurrentProcessDataOffset();

        UInt32 GetProcessIdByDataOffset(
            [In] UInt64 Offset);

        UInt64 GetCurrentProcessPeb();

        UInt32 GetProcessIdByPeb(
            [In] UInt64 Offset);

        UInt32 GetCurrentProcessSystemId();

        UInt32 GetProcessIdBySystemId(
            [In] UInt32 SysId);

        UInt64 GetCurrentProcessHandle();

        UInt32 GetProcessIdByHandle(
            [In] UInt64 Handle);

        void GetCurrentProcessExecutableName(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 ExeSize);
        }
    }