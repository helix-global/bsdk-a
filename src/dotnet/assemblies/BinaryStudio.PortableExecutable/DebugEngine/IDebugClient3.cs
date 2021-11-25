using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("DD492D7F-71B8-4AD6-A8DC-1C887479FF91"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugClient3 : IDebugClient2
        {
        /// <summary>
        /// The GetRunningProcessSystemIdByExecutableNameWide method searches for a process with a given executable file name and return its process ID.
        /// <note>Unicode version</note>
        /// </summary>
        /// <param name="Server">Specifies the process server to search for the executable name. If Server is zero, the engine will search for the executable name among the processes running on the local computer.</param>
        /// <param name="ExeName">Specifies the executable file name for which to search.</param>
        /// <param name="Flags">Specifies a bit-set that controls how the executable name is matched.</param>
        /// <returns>Returns the process ID of the first process to match <paramref name="ExeName"/>.
        UInt32 GetRunningProcessSystemIdByExecutableNameWide(
            [In] UInt64 Server,
            [In, MarshalAs(UnmanagedType.LPWStr)] String ExeName,
            [In] DEBUG_GET_PROC Flags);
 
        /// <summary>
        /// The GetRunningProcessDescriptionWide method returns a description of the process that includes the executable image name, the service names, the MTS package names, and the command line.
        /// <note>Unicode version</note>
        /// </summary>
        /// <param name="Server">Specifies the process server to query for the process description. If Server is zero, the engine will query information about the local process directly.</param>
        /// <param name="SystemId">Specifies the process ID of the process whose description is desired.</param>
        /// <param name="Flags">Specifies a bit-set containing options that affect the behavior of this method.</param>
        /// <param name="ExeName">Receives the name of the executable file used to start the process. If ExeName is <c>null, this information is not returned.
        /// <param name="ExeNameSize">Specifies the size in characters of the buffer <paramref name="ExeName"/>.
        /// <param name="ActualExeNameSize">Receives the size in characters of the executable file name.</param>
        /// <param name="Description">Receives extra information about the process, including service names, MTS package names, and the command line. If Description is <c>null, this information is not returned.
        /// <param name="DescriptionSize">Specifies the size in characters of the buffer <paramref name="Description"/>.
        /// <param name="ActualDescriptionSize">Receives the size in characters of the extra information.</param>
        void GetRunningProcessDescriptionWide(
            [In] UInt64 Server,
            [In] UInt32 SystemId,
            [In] DEBUG_PROC_DESC Flags,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder ExeName,
            [In] UInt32 ExeNameSize,
            [Out] out UInt32 ActualExeNameSize,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Description,
            [In] UInt32 DescriptionSize,
            [Out] out UInt32 ActualDescriptionSize);
 
        /// <summary>
        /// The CreateProcessWide method creates a process from the specified command line.
        /// <note>Unicode version</note>
        /// </summary>
        /// <param name="Server">Specifies the process server to use when attaching to the process. If Server is zero, the engine will create a local process without using a process server.</param>
        /// <param name="CommandLine">Specifies the command line to execute to create the new process. The CreateProcessWide method might modify the contents of the string that you supply in this parameter. Therefore, this parameter cannot be a pointer to read-only memory (such as a const variable or a literal string). Passing a constant string in this parameter can lead to an access violation.</param>
        /// <param name="CreateFlags">Specifies the flags to use when creating the process.</param>
        void CreateProcessWide(
            [In] UInt64 Server,
            [In, MarshalAs(UnmanagedType.LPWStr)] String CommandLine,
            [In] DEBUG_CREATE_PROCESS CreateFlags);
 
        /// <summary>
        /// The CreateProcessAndAttachWide method creates a process from a specified command line, then attach to another user-mode process. The created process is suspended and only allowed to execute when the attach has completed. This allows rough synchronization when debugging both, client and server processes.
        /// </summary>
        /// <param name="Server">Specifies the process server to use to attach to the process. If Server is zero, the engine will connect to the local process without using a process server.</param>
        /// <param name="CommandLine">Specifies the command line to execute to create the new process. If <paramref name="CommandLine"/> is <c>null</c>, then no process is created and these methods attach to an existing process, as <see cref="IDebugClient.AttachProcess"> does.
        /// <param name="CreateFlags">Specifies the flags to use when creating the process.</param>
        /// <param name="ProcessId">Specifies the process ID of the target process the debugger will attach to. If <paramref name="ProcessId"/> is zero, the debugger will attach to the process it created from <paramref name="CommandLine">.
        /// <param name="AttachFlags">Specifies the flags that control how the debugger attaches to the target process.</param>
        void CreateProcessAndAttachWide(
            [In] UInt64 Server,
            [In, MarshalAs(UnmanagedType.LPWStr)] String CommandLine,
            [In] DEBUG_CREATE_PROCESS CreateFlags,
            [In] UInt32 ProcessId,
            [In] DEBUG_ATTACH AttachFlags);
        }
    }