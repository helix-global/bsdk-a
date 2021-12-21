using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.PlatformComponents.Win32
    {
    public class Process : IJsonSerializable
        {
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

        private unsafe Process(ref SYSTEM_PROCESS_INFORMATION64 source)
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
            if (source.ImageName.Length > 0) { ImageName = new String((char*)(IntPtr)source.ImageName.Buffer); }
            if ((ns = NtQueryInformationProcess(
                (IntPtr)UniqueProcessId, ProcessBasicInformation, out PROCESS_BASIC_INFORMATION bi,
                sizeof(PROCESS_BASIC_INFORMATION), out var rsz)) == NTSTATUS.STATUS_SUCCESS) {
                if (bi.PebBaseAddress != IntPtr.Zero) {
                    var peb = (PEB64*)bi.PebBaseAddress;
                    BeingDebugged = peb->BeingDebugged > 0;
                    }
                }
            }

        private unsafe Process(ref SYSTEM_PROCESS_INFORMATION64 source, PROCESSENTRY32* e)
            :this(ref source)
            {
            UniqueParentProcessId = e->ParentProcessId;
            if (ImageName == null) {
                ImageName = new String(e->ExeFile);
                }
            }

        private unsafe Process(ref SYSTEM_PROCESS_INFORMATION32 source)
            {
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
            if (source.ImageName.Length > 0) { ImageName = new String((char*)(IntPtr)source.ImageName.Buffer); }
            if (NtQueryInformationProcess(
                (IntPtr)UniqueProcessId, ProcessBasicInformation, out PROCESS_BASIC_INFORMATION bi,
                sizeof(PROCESS_BASIC_INFORMATION), out var rsz) == NTSTATUS.STATUS_SUCCESS) {
                MergeFrom(ref bi);
                }
            }

        private unsafe Process(ref SYSTEM_PROCESS_INFORMATION32 source, PROCESSENTRY32* e)
            :this(ref source)
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
                (IntPtr)UniqueProcessId, ProcessBasicInformation, out PROCESS_BASIC_INFORMATION bi,
                sizeof(PROCESS_BASIC_INFORMATION), out var rsz) == NTSTATUS.STATUS_SUCCESS) {
                MergeFrom(ref bi);
                }
            if (SessionId == null) {
                if (ProcessIdToSessionId((Int32)UniqueProcessId, out var r)) {
                    SessionId = r;
                    }
                }
            }

        public static unsafe IList<Process> Processes { get {
            var r = new List<Process>();
            var snapshotH = CreateToolhelp32Snapshot(TH32CS_FLAGS.TH32CS_SNAPPROCESS, 0);
            var snapshotP = new Dictionary<Int64,PROCESSENTRY32>();
            var snapshotR = new Dictionary<Int64,Process>();
            var Is64 = Environment.Is64BitProcess;
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
            Byte* pspi;
            Process process;
            for (var size = 0x800; (pspi = (Byte*)LocalAlloc(LMEM_FIXED, (IntPtr)size)) != null; size += 0x800) {
                try
                    {
                    var ns = NtQuerySystemInformation(SystemInformationType.SystemProcessInformation, pspi, size, IntPtr.Zero);
                    if (ns == NTSTATUS.STATUS_SUCCESS) {
                        if (Is64)
                            {
                            var i = (SYSTEM_PROCESS_INFORMATION64*)pspi;
                            while (i->NextEntryOffset != 0) {
                                r.Add(process = snapshotP.TryGetValue(i->UniqueProcessId, out var e)
                                    ? new Process(ref *i, &e)
                                    : new Process(ref *i));
                                snapshotP.Remove(i->UniqueProcessId);
                                snapshotR.Add(process.UniqueProcessId, process);
                                i = (SYSTEM_PROCESS_INFORMATION64*)((Byte*)i + i->NextEntryOffset);
                                }
                            }
                        else
                            {
                            var i = (SYSTEM_PROCESS_INFORMATION32*)pspi;
                            while (i->NextEntryOffset != 0) {
                                r.Add(process = snapshotP.TryGetValue(i->UniqueProcessId, out var e)
                                    ? new Process(ref *i, &e)
                                    : new Process(ref *i));
                                snapshotP.Remove(i->UniqueProcessId);
                                snapshotR.Add(process.UniqueProcessId, process);
                                i = (SYSTEM_PROCESS_INFORMATION32*)((Byte*)i + i->NextEntryOffset);
                                }
                            }
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

            foreach (var i in snapshotP) {
                var j = i.Value;
                r.Add(process = new Process(&j));
                snapshotR.Add(process.UniqueProcessId, process);
                }

            if (NtQueryInformationProcess(
                GetCurrentProcess(), ProcessBasicInformation, out PROCESS_BASIC_INFORMATION bi,
                sizeof(PROCESS_BASIC_INFORMATION), out var rsz) == NTSTATUS.STATUS_SUCCESS) {
                snapshotR[(Int64)bi.UniqueProcessId].MergeFrom(ref bi);
                }
            return new ReadOnlyCollection<Process>(r);
            }}

        private unsafe void MergeFrom(ref PROCESS_BASIC_INFORMATION source) {
            if (source.PebBaseAddress != IntPtr.Zero) {
                if (Environment.Is64BitProcess) {
                    var peb = (PEB64*)source.PebBaseAddress;
                    BeingDebugged = peb->BeingDebugged > 0;
                    }
                else
                    {
                    var peb = (PEB32*)source.PebBaseAddress;
                    BeingDebugged = peb->BeingDebugged > 0;
                    }
                }
            return;
            }

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)] private static extern unsafe void* LocalAlloc(Int32 flags, IntPtr size);
        [DllImport("kernel32.dll", SetLastError = true)] internal static extern unsafe IntPtr LocalFree(void* data);
        [DllImport("kernel32.dll")] private static extern Boolean CloseHandle(IntPtr o);
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe NTSTATUS NtQueryInformationProcess(IntPtr process, Int32 @class, out PROCESS_BASIC_INFORMATION pi, Int32 pisz, out Int32 r);
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe NTSTATUS NtQueryInformationProcess(IntPtr process, Int32 @class, void* pi, Int32 pisz, out Int32 r);
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe NTSTATUS NtQuerySystemInformation(SystemInformationType query, void* dataPtr, Int32 size, IntPtr r);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] public static extern IntPtr CreateToolhelp32Snapshot(TH32CS_FLAGS flags, Int32 processId);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Process32FirstW")] private static extern unsafe Boolean Process32First(IntPtr snapshot, [In][Out] void* r);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Process32NextW" )] private static extern unsafe Boolean Process32Next (IntPtr snapshot, [In][Out] void* r);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr GetCurrentProcess();
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool ProcessIdToSessionId([In] int dwProcessId, out int pSessionId);

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