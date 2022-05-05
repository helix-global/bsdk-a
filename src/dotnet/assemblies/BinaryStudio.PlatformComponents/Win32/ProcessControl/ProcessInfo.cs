using System;
using System.Diagnostics;

namespace BinaryStudio.PlatformComponents.Win32
    {
    public class ProcessInfo
        {
        public Int64 UniqueProcessId { get; }
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
        public String Name { get; }
        public Int32 BasePriority { get; }
        public ProcessPriorityClass PriorityClass { get; }
        public Int32 SessionId { get; }
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
        internal Boolean IsFailedOpeningProcess { get; }
        public Boolean IsWow64 { get; }

        internal ProcessInfo(ref SYSTEM_PROCESS_INFORMATION64 source)
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
            }

        internal ProcessInfo(ref SYSTEM_PROCESS_INFORMATION32 source)
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
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return $"{{{SessionId:x8}}}";
            }
        }
    }