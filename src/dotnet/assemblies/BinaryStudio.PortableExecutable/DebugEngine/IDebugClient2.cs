using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("EDBED635-372E-4DAB-BBFE-ED0D2F63BE81"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugClient2 : IDebugClient
        {
        /// <summary>
        /// The WriteDumpFile2 method creates a user-mode or kernel-modecrash dump file.
        /// </summary>
        /// <param name="DumpFile">Specifies the name of the dump file to create. DumpFile must include the file name extension. DumpFile can include a relative or absolute path; relative paths are relative to the directory in which the debugger was started.</param>
        /// <param name="Qualifier">Specifies the type of dump file to create.</param>
        /// <param name="FormatFlags">Specifies flags that determine the format of the dump file and--for user-mode minidumps--what information to include in the file.</param>
        /// <param name="Comment">Specifies a comment string to be included in the crash dump file. This string is displayed in the debugger console when the dump file is loaded. Some dump file formats do not support the storing of comment strings.</param>
        void WriteDumpFile2(
            [In, MarshalAs(UnmanagedType.LPStr)] String DumpFile,
            [In] DEBUG_DUMP Qualifier,
            [In] DEBUG_FORMAT FormatFlags,
            [In, MarshalAs(UnmanagedType.LPStr)] String Comment = null);
 
        /// <summary>
        /// The AddDumpInformationFile method registers additional files containing supporting information that will be used when opening a dump file.
        /// <note>ANSI version.</note>
        /// </summary>
        /// <param name="InfoFile">Specifies the name of the file containing the supporting information.</param>
        /// <param name="Type">Specifies the type of the file InfoFile. Currently, only files containing paging file information are supported, and Type must be set to <see cref="DebugDumpFile.PageFileDump"/>.
        void AddDumpInformationFile(
            [In, MarshalAs(UnmanagedType.LPStr)] String InfoFile,
            [In] DEBUG_DUMP_FILE Type);
 
        /// <summary>
        /// The EndProcessServer method requests that a process server be shut down.
        /// </summary>
        /// <param name="Server">Specifies the process server to shut down. This handle must have been previously returned by <see cref="IDebugClient.ConnectProcessServer"/>.
        void EndProcessServer(
            [In] UInt64 Server);
 
        /// <summary>
        /// The WaitForProcessServerEnd method waits for a local process server to exit.
        /// </summary>
        /// <param name="Timeout">Specifies how long in milliseconds to wait for a process server to exit. If Timeout is uint.MaxValue, this method will not return until a process server has ended.</param>
        void WaitForProcessServerEnd(
            [In] UInt32 Timeout);
 
        /// <summary>
        /// The IsKernelDebuggerEnabled method checks whether kernel debugging is enabled for the local kernel.
        /// </summary>
        /// <returns>S_OK if kernel debugging is enabled for the local kernel; S_FALSE otherwise.</returns>
        [PreserveSig]
        Int32 IsKernelDebuggerEnabled();
 
        /// <summary>
        /// The TerminateCurrentProcess method attempts to terminate the current process.
        /// </summary>
        void TerminateCurrentProcess();
 
        /// <summary>
        /// The DetachCurrentProcess method detaches the debugger engine from the current process, resuming all its threads.
        /// </summary>
        void DetachCurrentProcess();
 
        /// <summary>
        /// The AbandonCurrentProcess method removes the current process from the debugger engine's process list without detaching or terminating the process.
        /// </summary>
        void AbandonCurrentProcess();
        }
    }