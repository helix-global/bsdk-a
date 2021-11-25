using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("CA83C3DE-5089-4CF8-93C8-D892387F2A5E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugClient4 : IDebugClient3
        {
        /// <summary>
        /// The OpenDumpFileWide method opens a dump file as a debugger target.
        /// <note>Unicode version</note>
        /// </summary>
        /// <param name="FileName">Specifies the name of the dump file to open -- unless <paramref name="FileHandle"/> is not zero, in which case <paramref name="FileName"> is used only when the engine is queried for the name of the dump file. <paramref name="FileName"> must include the file name extension. <paramref name="FileName"> can include a relative or absolute path; relative paths are relative to the directory in which the debugger was started. <paramref name="FileName"> can also be in the form of a file URL, starting with "file://". If <paramref name="FileName"> specifies a cabinet (.cab) file, the cabinet file is searched for the first file with extension .kdmp, then .hdmp, then .mdmp, and finally .dmp.
        /// <param name="FileHandle">Specifies the file handle of the dump file to open. If <paramref name="FileHandle"/> is zero, <paramref name="FileName"> is used to open the dump file. Otherwise, if <paramref name="FileName"> is not <c>null</c>, the engine returns it when queried for the name of the dump file. If <paramref name="FileHandle"> is not zero and <paramref name="FileName"> is <c>null</c>, the engine will return <HandleOnly> for the file name.
        void OpenDumpFileWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String FileName = null,
            [In] UInt64 FileHandle = default(UInt64));
 
        /// <summary>
        /// The WriteDumpFileWide method creates a user-mode or kernel-modecrash dump file.
        /// <note>Unicode version.</note>
        /// </summary>
        /// <param name="FileName">Specifies the name of the dump file to create. <paramref name="FileName"/> must include the file name extension. <paramref name="FileName"> can include a relative or absolute path; relative paths are relative to the directory in which the debugger was started. If <paramref name="FileHandle"> is not 0, <paramref name="FileName"> is ignored (except when writing status messages to the debugger console).
        /// <param name="FileHandle">Specifies the file handle of the file to write the crash dump to. If <paramref name="FileHandle"/> is 0, the file specified in <paramref name="FileName"> is used instead.
        /// <param name="Qualifier">Specifies the type of dump to create.</param>
        /// <param name="FormatFlags">Specifies flags that determine the format of the dump file and--for user-mode minidumps--what information to include in the file.</param>
        /// <param name="Comment">Specifies a comment string to be included in the crash dump file. This string is displayed in the debugger console when the dump file is loaded.</param>
        void WriteDumpFileWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String FileName,
            [In] UInt64 FileHandle = default(UInt64),
            [In] DEBUG_DUMP Qualifier = DEBUG_DUMP.DEBUG_DUMP_DEFAULT,
            [In] DEBUG_FORMAT FormatFlags = DEBUG_FORMAT.DEBUG_FORMAT_DEFAULT,
            [In, MarshalAs(UnmanagedType.LPWStr)] String Comment = null);
 
        /// <summary>
        /// The AddDumpInformationFileWide method registers additional files containing supporting information that will be used when opening a dump file. The ASCII version of this method is <see cref="IDebugClient2.AddDumpInformationFile"/>.
        /// </summary>
        /// <param name="FileName">Specifies the name of the file containing the supporting information. If <paramref name="FileHandle"/> is not zero, <paramref name="FileName"> is used only for informational purposes.
        /// <param name="FileHandle">Specifies the handle of the file containing the supporting information. If <paramref name="FileHandle"/> is zero, the file named in <paramref name="FileName"> is used.
        /// <param name="Type">Specifies the type of the file in <paramref name="FileName"/> or <paramref name="FileHandle">. Currently, only files containing paging file information are supported, and Type must be set to <see cref="DebugDumpFile.PageFileDump">.
        void AddDumpInformationFileWide(
            [In, Optional, MarshalAs(UnmanagedType.LPWStr)] String FileName,
            [In] UInt64 FileHandle = default(UInt64),
            [In] DEBUG_DUMP_FILE Type = DEBUG_DUMP_FILE.DEBUG_DUMP_FILE_PAGE_FILE_DUMP);
 
        /// <summary>
        /// The GetNumberDumpFiles method returns the number of files containing supporting information that were used when opening the current dump target.
        /// </summary>
        /// <returns>Returns the number of files.</returns>
        UInt32 GetNumberDumpFiles();
 
        /// <summary>
        /// The GetDumpFileWide method describes the files containing supporting information that were used when opening the current dump target.
        /// <note type="note">ANSI version</note>
        /// </summary>
        /// <param name="Index">Specifies which file to describe. Index can take values between zero and the number of files minus one; the number of files can be found by using <see cref="IDebugClient4.GetNumberDumpFiles"/>.
        /// <param name="Buffer">Receives the file name. If <paramref name="Buffer"/> is <c>null</c>, this information is not returned.
        /// <param name="BufferSize">Specifies the size in characters of the buffer <paramref name="Buffer"/>.
        /// <param name="NameSize">Receives the size of the file name.</param>
        /// <param name="Handle">Receives the file handle of the file.</param>
        /// <param name="Type">Receives the type of the file.</param>
        void GetDumpFile(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 NameSize,
            [Out] out UInt64 Handle,
            [Out] out DEBUG_DUMP_FILE Type);
 
        /// <summary>
        /// The GetDumpFileWide method describes the files containing supporting information that were used when opening the current dump target.
        /// <note type="note">Unicode version</note>
        /// </summary>
        /// <param name="Index">Specifies which file to describe. Index can take values between zero and the number of files minus one; the number of files can be found by using <see cref="IDebugClient4.GetNumberDumpFiles"/>.
        /// <param name="Buffer">Receives the file name. If <paramref name="Buffer"/> is <c>null</c>, this information is not returned.
        /// <param name="BufferSize">Specifies the size in characters of the buffer <paramref name="Buffer"/>.
        /// <param name="NameSize">Receives the size of the file name.</param>
        /// <param name="Handle">Receives the file handle of the file.</param>
        /// <param name="Type">Receives the type of the file.</param>
        void GetDumpFileWide(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 NameSize,
            [Out] out UInt64 Handle,
            [Out] out DEBUG_DUMP_FILE Type);
        }
    }