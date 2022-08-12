using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.PlatformComponents.Win32
    {
    public class Process : IJsonSerializable
        {
        private SystemProcessInformation pi;
        private String processname;

        public Int64 UniqueProcessId { get; }
        public Int64 UniqueParentProcessId { get; }
        public Int32 NumberOfThreads { get; }
        public Int32 HandleCount { get; }
        public Int64 PeakVirtualSize { get; }
        public Int64 VirtualSize { get; }
        public Int64 PeakWorkingSetSize { get; }
        public Int64 WorkingSetSize { get; }
        public Int64 QuotaPagedPoolUsage { get; }
        public Int64 QuotaNonPagedPoolUsage { get; }
        public Int64 QuotaPeakPagedPoolUsage { get; }
        public Int64 QuotaPeakNonPagedPoolUsage { get; }
        public Int64 PagefileUsage { get; }
        public Int64 PeakPagefileUsage { get; }
        public Int64 PrivatePageCount { get; }
        public Int64 PageFaultCount { get; }
        public String ImageName { get; }
        public Int32 BasePriority { get; }
        public ProcessPriorityClass PriorityClass { get; }
        public Int32? SessionId { get; }
        public TimeSpan CycleTime { get; }
        public Int64 ReadOperationCount { get; }
        public Int64 WriteOperationCount { get; }
        public Int64 OtherOperationCount { get; }
        public Int64 ReadTransferCount { get; }
        public Int64 WriteTransferCount { get; }
        public Int64 OtherTransferCount { get; }
        public Int64 JobObjectId { get; }
        public String ImageFullPath { get; }
        public Int32 GDIObjectCount { get; }
        public Int32 UserObjectCount { get; }
        public String CommandLine { get; }
        public Boolean IsWow64 { get; }
        public Boolean? BeingDebugged { get;private set; }

        public String ProcessName { get {
            if (processname == null) {
                
                }
            return processname;
            }}

        static Process()
            {
            if (OpenProcessToken(GetCurrentProcess(), TokenAccessLevels.AdjustPrivileges|TokenAccessLevels.Query, out var tokenhandle)) {
                AdjustTokenPrivilege(tokenhandle, SE_DEBUG_NAME, true);
                }
            }

        private unsafe Process(SystemProcessInformation source)
            {
            NTSTATUS ns;
            UniqueProcessId = source.UniqueProcessId;
            NumberOfThreads = source.NumberOfThreads;
            HandleCount = source.HandleCount;
            PeakVirtualSize = source.PeakVirtualSize;
            VirtualSize = source.VirtualSize;
            PeakWorkingSetSize = source.PeakWorkingSetSize;
            WorkingSetSize = source.WorkingSetSize;
            QuotaPagedPoolUsage = source.QuotaPagedPoolUsage;
            QuotaNonPagedPoolUsage = source.QuotaNonPagedPoolUsage;
            PagefileUsage = source.PagefileUsage;
            PeakPagefileUsage = source.PeakPagefileUsage;
            PrivatePageCount = source.PrivatePageCount;
            BasePriority = source.BasePriority;
            SessionId = source.SessionId;
            if (source.ImageName.Length > 0) { ImageName = source.ImageName; }
            if ((ns = NtQueryInformationProcess(
                (IntPtr)UniqueProcessId, ProcessBasicInformation, out ProcessBasicInformation bi,
                sizeof(ProcessBasicInformation), out var rsz)) == NTSTATUS.STATUS_SUCCESS) {
                if (bi.PebBaseAddress != IntPtr.Zero) {
                    var peb = (PEB64*)bi.PebBaseAddress;
                    BeingDebugged = peb->BeingDebugged > 0;
                    }
                }
            }

        private unsafe Process(SystemProcessInformation source, PROCESSENTRY32* e)
            :this(source)
            {
            UniqueParentProcessId = e->ParentProcessId;
            if (ImageName == null) {
                ImageName = new String(e->ExeFile);
                }
            }

        private unsafe Process(PROCESSENTRY32* e)
            {
            UniqueProcessId = e->ProcessId;
            UniqueParentProcessId = e->ParentProcessId;
            ImageName = new String(e->ExeFile);
            if (NtQueryInformationProcess(
                (IntPtr)UniqueProcessId, ProcessBasicInformation, out ProcessBasicInformation bi,
                sizeof(ProcessBasicInformation), out var rsz) == NTSTATUS.STATUS_SUCCESS) {
                MergeFrom(ref bi);
                }
            if (SessionId == null) {
                if (ProcessIdToSessionId((Int32)UniqueProcessId, out var r)) {
                    SessionId = r;
                    }
                }
            }

        private Process(Int64 id)
            {
            UniqueProcessId = id;
            }

        public unsafe Process ParentProcess { get {
            var client = new CLIENT_ID{
                UniqueProcess = (IntPtr)UniqueProcessId
                };
            var attributes = new OBJECT_ATTRIBUTES {
                Length = sizeof(OBJECT_ATTRIBUTES)
                };
            Validate(NtOpenProcess(out var r, ProcessSpecificAccessRights.ProcessQueryLimitedInformation, ref attributes, ref client));
            Validate(NtQueryInformationProcess(r, ProcessBasicInformation, out var bi, sizeof(ProcessBasicInformation), out var rsz));
            return new Process((Int64)bi.InheritedFromUniqueProcessId);
            }}

        /// <summary>Gets a new <see cref="Process" /> component and associates it with the currently active process.</summary>
        /// <returns>A new <see cref="Process" /> component associated with the process resource that is running the calling application.</returns>
        public static Process CurrentProcess { get {
            Build(out var processes, out var currentprocess);
            return currentprocess;
            }}

        public static IList<Process> Processes { get {
            Build(out var processes, out var currentprocess);
            return processes;
            }}

        #region M:CopyTo(IntPtr,SystemProcessInformation):SystemProcessInformation
        private static unsafe SystemProcessInformation CopyTo(IntPtr source, SystemProcessInformation target) {
            #if NET35
            if (IntPtr.Size == 8)
            #else
            if (Environment.Is64BitProcess)
            #endif
                {
                var r = (SYSTEM_PROCESS_INFORMATION64*)source;
                target.NextEntryOffset = r->NextEntryOffset;
                target.NumberOfThreads = r->NumberOfThreads;
                target.ImageName = new String((Char*)(IntPtr)r->ImageName.Buffer);
                target.BasePriority = r->BasePriority;
                target.UniqueProcessId = r->UniqueProcessId;
                target.HandleCount = r->HandleCount;
                target.SessionId = r->SessionId;
                target.PeakVirtualSize = r->PeakVirtualSize;
                target.VirtualSize = r->VirtualSize;
                target.PeakWorkingSetSize = r->PeakWorkingSetSize;
                target.WorkingSetSize = r->WorkingSetSize;
                target.QuotaPagedPoolUsage = r->QuotaPagedPoolUsage;
                target.QuotaNonPagedPoolUsage = r->QuotaNonPagedPoolUsage;
                target.PagefileUsage = r->PagefileUsage;
                target.PeakPagefileUsage = r->PeakPagefileUsage;
                target.PrivatePageCount = r->PrivatePageCount;
                target.ReadOperationCount = r->ReadOperationCount;
                target.WriteOperationCount = r->WriteOperationCount;
                target.OtherOperationCount = r->OtherOperationCount;
                target.ReadTransferCount = r->ReadTransferCount;
                target.WriteTransferCount = r->WriteTransferCount;
                target.OtherTransferCount = r->OtherTransferCount;
                }
            else
                {
                var r = (SYSTEM_PROCESS_INFORMATION32*)source;
                target.NextEntryOffset = r->NextEntryOffset;
                target.NumberOfThreads = r->NumberOfThreads;
                target.ImageName = new String((Char*)(IntPtr)r->ImageName.Buffer);
                target.BasePriority = r->BasePriority;
                target.UniqueProcessId = r->UniqueProcessId;
                target.HandleCount = r->HandleCount;
                target.SessionId = r->SessionId;
                target.PeakVirtualSize = r->PeakVirtualSize;
                target.VirtualSize = r->VirtualSize;
                target.PeakWorkingSetSize = r->PeakWorkingSetSize;
                target.WorkingSetSize = r->WorkingSetSize;
                target.QuotaPagedPoolUsage = r->QuotaPagedPoolUsage;
                target.QuotaNonPagedPoolUsage = r->QuotaNonPagedPoolUsage;
                target.PagefileUsage = r->PagefileUsage;
                target.PeakPagefileUsage = r->PeakPagefileUsage;
                target.PrivatePageCount = r->PrivatePageCount;
                target.ReadOperationCount = r->ReadOperationCount;
                target.WriteOperationCount = r->WriteOperationCount;
                target.OtherOperationCount = r->OtherOperationCount;
                target.ReadTransferCount = r->ReadTransferCount;
                target.WriteTransferCount = r->WriteTransferCount;
                target.OtherTransferCount = r->OtherTransferCount;
                }
            return target;
            }
        #endregion
        #region M:QuerySystemProcessInformation:IEnumerable<SystemProcessInformation>
        private static IEnumerable<SystemProcessInformation> QuerySystemProcessInformation()
            {
            IntPtr pspi;
            for (var size = 0x800; (pspi = (IntPtr)LocalAlloc(LMEM_FIXED, (IntPtr)size)) != IntPtr.Zero; size += 0x800) {
                try
                    {
                    var ns = NtQuerySystemInformation(SystemInformationType.SystemProcessInformation, pspi, size, IntPtr.Zero);
                    if (ns == NTSTATUS.STATUS_SUCCESS) {
                        var r = pspi;
                        do
                            {
                            var i = CopyTo(r, new SystemProcessInformation());
                            yield return i;
                            if (i.NextEntryOffset == 0) { break; }
                            r = (IntPtr)((Int64)r + i.NextEntryOffset);
                            }
                        while (true);
                        break;
                        }
                    if (ns != NTSTATUS.STATUS_INFO_LENGTH_MISMATCH)
                        {
                        Validate(ns);
                        }
                    }
                finally
                    {
                    LocalFree(pspi);
                    }
                }
            }
        #endregion

        private static unsafe void Build(out IList<Process> processes, out Process currentprocess)
            {
            processes = EmptyArray<Process>.Value;
            currentprocess = null;
            var r = new List<Process>();
            var snapshotH = CreateToolhelp32Snapshot(TH32CS_FLAGS.TH32CS_SNAPPROCESS, 0);
            var snapshotP = new Dictionary<Int64,PROCESSENTRY32>();
            var snapshotR = new Dictionary<Int64,Process>();
            try
                {
                var e = new PROCESSENTRY32 { Size = sizeof(PROCESSENTRY32) };
                if (Process32First(snapshotH, &e)) {
                    do
                        {
                        snapshotP[e.ProcessId] = e;
                        }
                    while (Process32Next(snapshotH, &e));
                    }
                }
            finally
                {
                CloseHandle(snapshotH);
                }
            Process process;
            foreach (var i in QuerySystemProcessInformation()) {
                r.Add(process = snapshotP.TryGetValue(i.UniqueProcessId, out var e)
                    ? new Process(i, &e)
                    : new Process(i));
                snapshotP.Remove(i.UniqueProcessId);
                snapshotR.Add(process.UniqueProcessId, process);
                }

            foreach (var i in snapshotP) {
                var j = i.Value;
                r.Add(process = new Process(&j));
                snapshotR.Add(process.UniqueProcessId, process);
                }

            if (NtQueryInformationProcess(
                GetCurrentProcess(), ProcessBasicInformation, out ProcessBasicInformation bi,
                sizeof(ProcessBasicInformation), out var rsz) == NTSTATUS.STATUS_SUCCESS) {
                currentprocess = snapshotR[(Int64)bi.UniqueProcessId].MergeFrom(ref bi);
                }
            processes = new ReadOnlyCollection<Process>(r);
            }


        private unsafe Process MergeFrom(ref ProcessBasicInformation source) {
            if (source.PebBaseAddress != IntPtr.Zero) {
                #if NET35
                if (IntPtr.Size == sizeof(Int64)) {
                #else
                if (Environment.Is64BitProcess) {
                #endif
                    var peb = (PEB64*)source.PebBaseAddress;
                    BeingDebugged = peb->BeingDebugged > 0;
                    }
                else
                    {
                    var peb = (PEB32*)source.PebBaseAddress;
                    BeingDebugged = peb->BeingDebugged > 0;
                    }
                }
            return this;
            }

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)] private static extern IntPtr LocalAlloc(Int32 flags, IntPtr size);
        [DllImport("kernel32.dll", SetLastError = true)] internal static extern IntPtr LocalFree(IntPtr data);
        [DllImport("kernel32.dll")] private static extern Boolean CloseHandle(IntPtr o);
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe NTSTATUS NtQueryInformationProcess(IntPtr process, Int32 @class, out ProcessBasicInformation pi, Int32 pisz, out Int32 r);
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe NTSTATUS NtQueryInformationProcess(ProcessHandle process, Int32 @class, out ProcessBasicInformation pi, Int32 pisz, out Int32 r);
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe NTSTATUS NtQueryInformationProcess(IntPtr process, Int32 @class, void* pi, Int32 pisz, out Int32 r);
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe NTSTATUS NtQuerySystemInformation(SystemInformationType query, void* dataPtr, Int32 size, IntPtr r);
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe NTSTATUS NtQuerySystemInformation(SystemInformationType query, IntPtr dataPtr, Int32 size, IntPtr r);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] public static extern IntPtr CreateToolhelp32Snapshot(TH32CS_FLAGS flags, Int32 processId);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Process32FirstW")] private static extern unsafe Boolean Process32First(IntPtr snapshot, [In][Out] void* r);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Process32NextW" )] private static extern unsafe Boolean Process32Next (IntPtr snapshot, [In][Out] void* r);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr GetCurrentProcess();
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool ProcessIdToSessionId([In] int dwProcessId, out int pSessionId);
        [DllImport("advapi32.dll", SetLastError = true)] private static extern Boolean OpenThreadToken(IntPtr threadhandle, TokenAccessLevels desiredaccess, Boolean openasself, out IntPtr tokenhandle);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)] private static extern Boolean LookupPrivilegeValue(String systemname, String name, out LUID luid);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern Boolean AdjustTokenPrivileges(IntPtr tokenhandle, Boolean disableallprivileges, ref TOKEN_PRIVILEGE newstate, Int32 bufferlength, ref TOKEN_PRIVILEGE previousstate, ref Int32 returnlength);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern unsafe Boolean AdjustTokenPrivileges(IntPtr tokenhandle, Boolean disableallprivileges, ref TOKEN_PRIVILEGE newstate, Int32 bufferlength, void* previousstate, Int32* returnlength);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern Boolean OpenProcessToken(IntPtr ProcessHandle, TokenAccessLevels DesiredAccess, out IntPtr TokenHandle);
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe NTSTATUS NtOpenProcess(out IntPtr r, Int32 desiredaccess, ref OBJECT_ATTRIBUTES objectattributes, ref CLIENT_ID client);
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe NTSTATUS NtOpenProcess(out ProcessHandle r, ProcessSpecificAccessRights desiredaccess, ref OBJECT_ATTRIBUTES objectattributes, ref CLIENT_ID client);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern unsafe Boolean QueryFullProcessImageName(ProcessHandle process, UInt32 flags, char* name, ref Int32 sz);

        private const String SE_DEBUG_NAME          = "SeDebugPrivilege";
        private const UInt32 SE_PRIVILEGE_ENABLED   = 0x00000002;
        private const Int32 LMEM_FIXED = 0x0000;
        public const Int32 ProcessBasicInformation   =  0;
        public const Int32 ProcessDebugPort          =  7;
        public const Int32 ProcessWow64Information   = 26;
        public const Int32 ProcessImageFileName      = 27;
        public const Int32 ProcessBreakOnTermination = 29;

        #region M:BeginInvoke(Action[])
        private static void BeginInvoke(params Action[] actions) {
            if (actions != null) {
                var r = new List<WaitHandle>();
                for (var i = 0; i < actions.Length; ++i) {
                    if (actions[i] != null) {
                        r.Add(actions[i].BeginInvoke(null, null).AsyncWaitHandle);
                        }
                    }
                WaitHandle.WaitAll(r.ToArray());
                }
            }
        #endregion
        #region M:Validate(NTSTATUS)
        private static void Validate(NTSTATUS status)
            {
            if (status == NTSTATUS.STATUS_SUCCESS) { return; }
            var e = new NTStatusException(status);
            #if DEBUG
            Debug.Print($"NTStatusException:{e.Message}");
            #endif
            throw e;
            }
        #endregion
        #region M:Validate([out]Exception,NTSTATUS)
        private static Boolean Validate(out Exception e, NTSTATUS status)
            {
            e = null;
            if (status == NTSTATUS.STATUS_SUCCESS) { return true; }
            e = new NTStatusException(status);
            return false;
            }
        #endregion

        private static unsafe Boolean AdjustTokenPrivilege(IntPtr tokenhandle, String privilege, Boolean value) {
            if (tokenhandle == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(tokenhandle)); }
            if(!LookupPrivilegeValue(null, privilege, out var luid )) { return false; }
            var tokenprivileges = new TOKEN_PRIVILEGE();
            var previous = new TOKEN_PRIVILEGE();
            tokenprivileges.PrivilegeCount = 1;
            tokenprivileges.Privilege.Luid = luid;
            tokenprivileges.Privilege.Attributes = 0;
            var sz = Marshal.SizeOf(previous);
            if (AdjustTokenPrivileges(tokenhandle, false, ref tokenprivileges, sizeof(TOKEN_PRIVILEGE), ref previous, ref sz)) {
                previous.PrivilegeCount = 1;
                previous.Privilege.Luid = luid;
                if (value)
                    {
                    previous.Privilege.Attributes |= SE_PRIVILEGE_ENABLED;
                    }
                else
                    {
                    previous.Privilege.Attributes &= ~SE_PRIVILEGE_ENABLED;
                    }
                return AdjustTokenPrivileges(tokenhandle, false, ref previous, sz, null, null);
                }
            return false;
            }

        #region M:QueryFullProcessImageName(IntPtr):String
        public static unsafe String QueryFullProcessImageName(ProcessHandle source) {
            if (source == null) { return null; }
            Char* pspi;
            for (var size = 0x800; (pspi = (Char*)LocalAlloc(LMEM_FIXED, (IntPtr)size)) != null; size += 0x800) {
                try
                    {
                    var rsz = size;
                    if (QueryFullProcessImageName(source, 0, pspi, ref rsz)) {
                        if (rsz < size) {
                            return new String(pspi);
                            }
                        }
                    else
                        {
                        break;
                        }
                    }
                finally
                    {
                    LocalFree((IntPtr)pspi);
                    }
                }
            return null;
            }
        #endregion

        public void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                writer.WriteValue(serializer, nameof(UniqueProcessId), UniqueProcessId);
                writer.WriteValue(serializer, nameof(UniqueParentProcessId), UniqueParentProcessId);
                writer.WriteValue(serializer, nameof(ImageName), ImageName);
                writer.WriteValue(serializer, nameof(SessionId), SessionId);
                writer.WriteValue(serializer, nameof(BeingDebugged), BeingDebugged);
                }
            }
        }
    }