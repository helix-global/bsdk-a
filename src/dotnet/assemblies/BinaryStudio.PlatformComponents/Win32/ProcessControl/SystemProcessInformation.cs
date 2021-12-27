using System;

namespace BinaryStudio.PlatformComponents.Win32
    {
    internal class SystemProcessInformation
        {
        public Int32  NextEntryOffset;
        public Int32  NumberOfThreads;
        public String ImageName;
        public Int32  BasePriority;
        public Int64  UniqueProcessId;
        public Int32  HandleCount;
        public Int32  SessionId;
        public Int64  PeakVirtualSize;
        public Int64  VirtualSize;
        public Int64  PeakWorkingSetSize;
        public Int64  WorkingSetSize;
        public Int64  QuotaPagedPoolUsage;
        public Int64  QuotaNonPagedPoolUsage;
        public Int64  PagefileUsage;
        public Int64  PeakPagefileUsage;
        public Int64  PrivatePageCount;
        public Int64  ReadOperationCount;
        public Int64  WriteOperationCount;
        public Int64  OtherOperationCount;
        public Int64  ReadTransferCount;
        public Int64  WriteTransferCount;
        public Int64  OtherTransferCount;
        }
    }