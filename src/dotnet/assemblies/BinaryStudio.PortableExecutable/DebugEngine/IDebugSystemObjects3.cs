using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("E9676E2F-E286-4EA3-B0F9-DFE5D9FC330E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugSystemObjects3 : IDebugSystemObjects2
        {
        UInt32 GetEventSystem();

        UInt32 GetCurrentSystemId();

        void SetCurrentSystemId(
            [In] UInt32 Id);

        UInt32 GetNumberSystems();

        UInt32 GetSystemIdsByIndex(
            [In] UInt32 Start,
            [In] UInt32 Count);

        void GetTotalNumberThreadsAndProcesses(
            [Out] out UInt32 TotalThreads,
            [Out] out UInt32 TotalProcesses,
            [Out] out UInt32 LargestProcessThreads,
            [Out] out UInt32 LargestSystemThreads,
            [Out] out UInt32 LargestSystemProcesses);

        UInt64 GetCurrentSystemServer();

        UInt32 GetSystemByServer(
            [In] UInt64 Server);

        void GetCurrentSystemServerName(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 NameSize);
        }
    }