using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("27FE5639-8407-4F47-8364-EE118FB08AC8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugClient
        {
        /// <summary>
        /// The AttachKernel methods connect the debugger engine to a kernel target.
        /// </summary>
        /// <param name="Flags">Specifies the flags that control how the debugger attaches to the kernel target.</param>
        /// <param name="ConnectOptions">Specifies the connection settings for communicating with the computer running the kernel target.</param>
        void AttachKernel(
            [In] DEBUG_ATTACH_KERNEL Flags,
            [In, MarshalAs(UnmanagedType.LPStr)] String ConnectOptions = null);
 
        /// <summary>
        /// The GetKernelConnectionOptions method returns the connection options for the current kernel target.
        /// </summary>
        /// <param name="Buffer">Specifies the buffer to receive the connection options.</param>
        /// <param name="BufferSize">Specifies the size in characters of the buffer Buffer.</param>
        /// <param name="OptionsSize">Receives the size in characters of the connection options.</param>
        void GetKernelConnectionOptions(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 OptionsSize);
 
        /// <summary>
        /// The SetKernelConnectionOptions method updates some of the connection options for a live kernel target.
        /// </summary>
        /// <param name="Options">
        /// Specifies the connection options to update. The possible values are:
        /// <para><c>"resync" Re-synchronize the connection between the debugger engine and the kernel.For more information, see Synchronizing with the Target Computer.
        /// <para><c>"cycle_speed" For kernel connections through a COM port, cycle through the supported baud rates; for other connections, do nothing.
        /// </param>
        void SetKernelConnectionOptions(
            [In, MarshalAs(UnmanagedType.LPStr)] String Options);
 
        /// <summary>
        /// The StartProcessServer method starts a process server.
        /// </summary>
        /// <param name="Flags">Specifies the class of the targets that will be available through the process server. This must be set to <see cref="DEBUG_CLASS.UserWindows"/>.
        /// <param name="Options">Specifies the connections options for this process server. These are the same options given to the -t option of the DbgSrv command line.</param>
        /// <param name="Reserved">Set to <see cref="IntPtr.Zero"/>.
        void StartProcessServer(
            [In] DEBUG_CLASS Flags,
            [In, MarshalAs(UnmanagedType.LPStr)] String Options,
            [In] IntPtr Reserved = default(IntPtr));
 
        /// <summary>
        /// The ConnectProcessServer methods connect to a process server.
        /// </summary>
        /// <param name="RemoteOptions">Specifies how the debugger engine will connect with the process server. These are the same options passed to the -premote option on the WinDbg and CDB command lines.</param>
        /// <returns>Returns a handle for the process server. This handle is used when creating or attaching to processes by using the process server.</returns>
        UInt64 ConnectProcessServer(
            [In, MarshalAs(UnmanagedType.LPStr)] String RemoteOptions);
 
        /// <summary>
        /// The DisconnectProcessServer method disconnects the debugger engine from a process server.
        /// </summary>
        /// <param name="Server">Specifies the server from which to disconnect. This handle must have been previously returned by <see cref="ConnectProcessServer"/>.
        void DisconnectProcessServer(
            [In] UInt64 Server);
 
        /// <summary>
        /// The GetRunningProcessSystemIds method returns the process IDs for each running process.
        /// </summary>
        /// <param name="Server">Specifies the process server to query for process IDs. If Server is zero, the engine will return the process IDs of the processes running on the local computer.</param>
        /// <param name="Ids">Receives the process IDs. The size of this array is <paramref name="Count"/>. If <paramref name="Ids"> is <c>null</c>, this information is not returned.
        /// <param name="Count">Specifies the number of process IDs the array <paramref name="Ids"/> can hold.
        /// <param name="ActualCount">Receives the actual number of process IDs returned in <paramref name="Ids"/>.
        void GetRunningProcessSystemIds(
            [In] UInt64 Server,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] UInt32[] Ids,
            [In] UInt32 Count,
            [Out] out UInt32 ActualCount);
 
        /// <summary>
        /// The GetRunningProcessSystemIdByExecutableName method searches for a process with a given executable file name and return its process ID.
        /// </summary>
        /// <param name="Server">Specifies the process server to search for the executable name. If Server is zero, the engine will search for the executable name among the processes running on the local computer.</param>
        /// <param name="ExeName">Specifies the executable file name for which to search.</param>
        /// <param name="Flags">Specifies a bit-set that controls how the executable name is matched.</param>
        /// <returns>Returns the process ID of the first process to match <paramref name="ExeName"/>.
        UInt32 GetRunningProcessSystemIdByExecutableName(
            [In] UInt64 Server,
            [In, MarshalAs(UnmanagedType.LPStr)] String ExeName,
            [In] DEBUG_GET_PROC Flags);
 
        /// <summary>
        /// The GetRunningProcessDescription method returns a description of the process that includes the executable image name, the service names, the MTS package names, and the command line.
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
        void GetRunningProcessDescription(
            [In] UInt64 Server,
            [In] UInt32 SystemId,
            [In] DEBUG_PROC_DESC Flags,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder ExeName,
            [In] UInt32 ExeNameSize,
            [Out] out UInt32 ActualExeNameSize,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Description,
            [In] UInt32 DescriptionSize,
            [Out] out UInt32 ActualDescriptionSize);
 
        /// <summary>
        /// The AttachProcess method connects the debugger engine to a user-modeprocess.
        /// </summary>
        /// <param name="Server">Specifies the process server to use to attach to the process. If Server is zero, the engine will connect to a local process without using a process server.</param>
        /// <param name="ProcessId">Specifies the process ID of the target process the debugger will attach to.</param>
        /// <param name="AttachFlags">Specifies the flags that control how the debugger attaches to the target process.</param>
        void AttachProcess(
            [In] Int64 Server,
            [In] Int32 ProcessId,
            [In] DEBUG_ATTACH AttachFlags);
 
        /// <summary>
        /// The CreateProcess method creates a process from the specified command line.
        /// </summary>
        /// <param name="Server">Specifies the process server to use to attach to the process. If Server is zero, the engine will create a local process without using a process server.</param>
        /// <param name="CommandLine">Specifies the command line to execute to create the new process.</param>
        /// <param name="CreateFlags">Specifies the flags to use when creating the process. When creating and attaching to a process through the debugger engine, set one of the Platform SDK's process creation flags: <see cref="DebugCreateProcess.DebugProcess"/> or <see cref="DebugCreateProcess.DebugOnlyThisProcess">.
        void CreateProcess(
            [In] UInt64 Server,
            [In, MarshalAs(UnmanagedType.LPStr)] String CommandLine,
            [In] DEBUG_CREATE_PROCESS CreateFlags);
 
        /// <summary>
        /// The CreateProcessAndAttach method creates a process from a specified command line, then attach to another user-mode process. The created process is suspended and only allowed to execute when the attach has completed. This allows rough synchronization when debugging both, client and server processes.
        /// </summary>
        /// <param name="Server">Specifies the process server to use to attach to the process. If Server is zero, the engine will connect to the local process without using a process server.</param>
        /// <param name="CommandLine">Specifies the command line to execute to create the new process. If CommandLine is <c>null, then no process is created and these methods attach to an existing process, as <see cref="AttachProcess"> does.
        /// <param name="CreateFlags">Specifies the flags to use when creating the process. When creating and attaching to a process through the debugger engine, set one of the Platform SDK's process creation flags: <see cref="DebugCreateProcess.DebugProcess"/> or <see cref="DebugCreateProcess.DebugOnlyThisProcess">.
        /// <param name="ProcessId">Specifies the process ID of the target process the debugger will attach to. If ProcessId is zero, the debugger will attach to the process it created from CommandLine.</param>
        /// <param name="AttachFlags">Specifies the flags that control how the debugger attaches to the target process.</param>
        void CreateProcessAndAttach(
            [In] UInt64 Server,
            [In, MarshalAs(UnmanagedType.LPStr)] String CommandLine,
            [In] DEBUG_CREATE_PROCESS CreateFlags,
            [In] UInt32 ProcessId,
            [In] DEBUG_ATTACH AttachFlags);
 
        /// <summary>
        /// The GetProcessOptions method retrieves the process options affecting the current process.
        /// </summary>
        /// <returns>Receives a set of flags representing the process options for the current process.</returns>
        DEBUG_PROCESS GetProcessOptions();
 
        /// <summary>
        /// The AddProcessOptions method adds the process options to those options that affect the current process.
        /// </summary>
        /// <param name="Options">Specifies the process options to add to those affecting the current process.</param>
        void AddProcessOptions(
            [In] DEBUG_PROCESS Options);
 
        /// <summary>
        /// The RemoveProcessOptions method removes process options from those options that affect the current process.
        /// </summary>
        /// <param name="Options">Specifies the process options to remove from those affecting the current process.</param>
        void RemoveProcessOptions(
            [In] DEBUG_PROCESS Options);
 
        /// <summary>
        /// The SetProcessOptions method sets the process options affecting the current process.
        /// </summary>
        /// <param name="Options">Specifies a set of flags that will become the new process options for the current process.</param>
        void SetProcessOptions(
            [In] DEBUG_PROCESS Options);
 
        /// <summary>
        /// The OpenDumpFile method opens a dump file as a debugger target.
        /// <note> The engine doesn't completely attach to the dump file until the <see cref="IDebugControl.WaitForEvent"/> method has been called. When a dump file is created from a process or kernel, information about the last event is stored in the dump file. After the dump file is opened, the next time execution is attempted, the engine will generate this event for the event callbacks. Only then does the dump file become available in the debugging session.
        /// </summary>
        /// <param name="DumpFile">Specifies the name of the dump file to open. DumpFile must include the file name extension. DumpFile can include a relative or absolute path; relative paths are relative to the directory in which the debugger was started. DumpFile can have the form of a file URL, starting with "file://". If DumpFile specifies a cabinet (.cab) file, the cabinet file is searched for the first file with extension .kdmp, then .hdmp, then .mdmp, and finally .dmp.</param>
        void OpenDumpFile(
            [In, MarshalAs(UnmanagedType.LPStr)] String DumpFile);
 
        /// <summary>
        /// The WriteDumpFile method creates a user-mode or kernel-modecrash dump file.
        /// </summary>
        /// <param name="DumpFile">Specifies the name of the dump file to create. DumpFile must include the file name extension. DumpFile can include a relative or absolute path; relative paths are relative to the directory in which the debugger was started.</param>
        /// <param name="Qualifier">Specifies the type of dump file to create.</param>
        void WriteDumpFile(
            [In, MarshalAs(UnmanagedType.LPStr)] String DumpFile,
            [In] DEBUG_DUMP Qualifier);
 
        /// <summary>
        /// The ConnectSession method joins the client to an existing debugger session.
        /// </summary>
        /// <param name="Flags">Specifies a bit-set of option flags for connecting to the session.</param>
        /// <param name="HistoryLimit">Specifies the maximum number of characters from the session's history to send to this client's output upon connection.</param>
        void ConnectSession(
            [In] DEBUG_CONNECT_SESSION Flags,
            [In] UInt32 HistoryLimit);
 
        /// <summary>
        /// The StartServer method starts a debugging server.
        /// </summary>
        /// <param name="Options">Specifies the connections options for this server. These are the same options given to the .server debugger command or the WinDbg and CDB -server command-line option. For details on the syntax of this string, see Activating a Debugging Server.</param>
        /// <remarks>The server that is started will be accessible by other debuggers through the transport specified in the Options parameter.</remarks>
        void StartServer(
            [In, MarshalAs(UnmanagedType.LPStr)] String Options);
 
        /// <summary>
        /// The OutputServers method lists the servers running on a given computer.
        /// </summary>
        /// <param name="OutputControl">Specifies the output control to use while outputting the servers.</param>
        /// <param name="Machine">Specifies the name of the computer whose servers will be listed. Machine has the following form: \\computername </param>
        /// <param name="Flags">Specifies a bit-set that determines which servers to output.</param>
        void OutputServers(
            [In] DEBUG_OUTCTL OutputControl,
            [In, MarshalAs(UnmanagedType.LPStr)] String Machine,
            [In] DEBUG_SERVERS Flags);
 
        /// <summary>
        /// Attempts to terminate all processes in all targets.
        /// </summary>
        void TerminateProcesses();
 
        /// <summary>
        /// Detaches the debugger engine from all processes in all targets, resuming all their threads.
        /// </summary>
        void DetachProcesses();
 
        /// <summary>
        /// The EndSession method ends the current debugger session.
        /// </summary>
        /// <param name="Flags">Specifies how to end the session.</param>
        void EndSession(
            [In] DEBUG_END Flags);
 
        /// <summary>
        /// The GetExitCode method returns the exit code of the current process if that process has already run through to completion.
        /// </summary>
        /// <returns>Returns the exit code of the process. If the process is still running, Code will be set to STILL_ACTIVE.</returns>
        UInt32 GetExitCode();
 
        /// <summary>
        /// The DispatchCallbacks method lets the debugger engine use the current thread for callbacks.
        /// </summary>
        /// <param name="Timeout">Specifies how many milliseconds to wait before this method will return. If Timeout is uint.MaxValue, this method will not return until <see cref="IDebugClient.ExitDispatch"/> is called or an error occurs.
        void DispatchCallbacks(
            [In] UInt32 Timeout);
 
        /// <summary>
        /// The ExitDispatch method causes the <see cref="DispatchCallbacks"/> method to return.
        /// </summary>
        /// <param name="Client">Specifies the client whose <see cref="DispatchCallbacks"/> method should return.
        void ExitDispatch(
            [In, MarshalAs(UnmanagedType.Interface)] IDebugClient Client);
 
        /// <summary>
        /// The CreateClient method creates a new client object for the current thread.
        /// </summary>
        /// <returns>Returns an interface pointer for the new client.</returns>
        [return: MarshalAs(UnmanagedType.Interface)]
        IDebugClient CreateClient();
 
        /// <summary>
        /// The GetInputCallbacks method returns the input callbacks object registered with this client.
        /// </summary>
        /// <returns>Returns an interface pointer for the <see cref="IDebugInputCallbacks"/> object registered with the client.
        [return: MarshalAs(UnmanagedType.Interface)]
        IDebugInputCallbacks GetInputCallbacks();
 
        /// <summary>
        /// The SetInputCallbacks method registers an input callbacks object with the client.
        /// </summary>
        /// <param name="Callbacks">Specifies the interface pointer to the input callbacks object to register with this client.</param>
        void SetInputCallbacks(
            [In, MarshalAs(UnmanagedType.Interface)] IDebugInputCallbacks Callbacks = null);
 
        /// <summary>
        /// The GetOutputCallbacks method returns the output callbacks object registered with the client.
        /// </summary>
        /// <returns>Returns an interface pointer to the <see cref="IDebugOutputCallbacks"/> object registered with the client.
        [return: MarshalAs(UnmanagedType.Interface)]
        IDebugOutputCallbacks GetOutputCallbacks();
 
        /// <summary>
        /// The SetOutputCallbacks method registers an output callbacks object with this client.
        /// </summary>
        /// <param name="Callbacks">Specifies the interface pointer to the output callbacks object to register with this client.</param>
        void SetOutputCallbacks(
            [In, MarshalAs(UnmanagedType.Interface)] IDebugOutputCallbacks Callbacks = null);
 
        /// <summary>
        /// The GetOutputMask method returns the output mask currently set for the client.
        /// </summary>
        /// <returns>Returns the output mask for the client.</returns>
        DEBUG_OUTPUT GetOutputMask();
 
        /// <summary>
        /// The SetOutputMask method sets the output mask for the client.
        /// </summary>
        /// <param name="Mask">Specifies the new output mask for the client.</param>
        void SetOutputMask(
            [In] DEBUG_OUTPUT Mask);
 
        /// <summary>
        /// The GetOtherOutputMask method returns the output mask for another client.
        /// </summary>
        /// <param name="Client">Specifies the client whose output mask is desired.</param>
        /// <returns>Returns the output mask for the client.</returns>
        DEBUG_OUTPUT GetOtherOutputMask(
            [In, MarshalAs(UnmanagedType.Interface)] IDebugClient Client);
 
        /// <summary>
        /// The SetOtherOutputMask method sets the output mask for another client.
        /// </summary>
        /// <param name="Client">Specifies the client whose output mask will be set.</param>
        /// <param name="Mask">Specifies the new output mask for the client.</param>
        void SetOtherOutputMask(
            [In, MarshalAs(UnmanagedType.Interface)] IDebugClient Client,
            [In] DEBUG_OUTPUT Mask);
 
        /// <summary>
        /// Undocumented on MSDN.
        /// </summary>
        /// <returns></returns>
        UInt32 GetOutputWidth();
 
        /// <summary>
        /// Undocumented on MSDN.
        /// </summary>
        /// <param name="Columns"></param>
        void SetOutputWidth(
            [In] UInt32 Columns);
 
        /// <summary>
        /// Undocumented on MSDN.
        /// </summary>
        /// <param name="Buffer"></param>
        /// <param name="BufferSize"></param>
        /// <param name="PrefixSize"></param>
        void GetOutputLinePrefix(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 PrefixSize);
 
        /// <summary>
        /// Undocumented on MSDN.
        /// </summary>
        /// <param name="Prefix"></param>
        void SetOutputLinePrefix(
            [In, MarshalAs(UnmanagedType.LPStr)] String Prefix = null);
 
        /// <summary>
        /// The GetIdentity method returns a string describing the computer and user this client represents.
        /// </summary>
        /// <param name="Buffer">Specifies the buffer to receive the string. If <paramref name="Buffer"/> is <c>null</c>, this information is not returned.
        /// <param name="BufferSize">Specifies the size of the buffer <paramref name="Buffer"/>.
        /// <param name="IdentitySize">Receives the size of the string.</param>
        void GetIdentity(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 IdentitySize);
 
        /// <summary>
        /// The OutputIdentity method formats and outputs a string describing the computer and user this client represents.
        /// </summary>
        /// <param name="OutputControl">Specifies where to send the output.</param>
        /// <param name="Flags">Set to zero.</param>
        /// <param name="Format">Specifies a format string similar to the printf format string. However, this format string must only contain one formatting directive, %s, which will be replaced by a description of the computer and user this client represents.</param>
        void OutputIdentity(
            [In] DEBUG_OUTCTL OutputControl,
            [In] UInt32 Flags,
            [In, MarshalAs(UnmanagedType.LPStr)] String Format);
 
        /// <summary>
        /// The GetEventCallbacks method returns the event callbacks object registered with this client.
        /// </summary>
        /// <returns>Returns an interface pointer to the event callbacks object registered with this client.</returns>
        [return: MarshalAs(UnmanagedType.Interface)]
        IDebugEventCallbacks GetEventCallbacks();
 
        /// <summary>
        /// The SetEventCallbacks method registers an event callbacks object with this client.
        /// </summary>
        /// <param name="Callbacks">Specifies the interface pointer to the event callbacks object to register with this client.</param>
        void SetEventCallbacks(
            [In, MarshalAs(UnmanagedType.Interface)] IDebugEventCallbacks Callbacks = null);
 
        /// <summary>
        /// Forces any remaining buffered output to be delivered to the <see cref="IDebugOutputCallbacks"/> object registered with this client.
        /// </summary>
        void FlushCallbacks();
        }
    }