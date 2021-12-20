using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Explicit, Pack = 8)]
    public struct SYSTEM_PROCESS_INFORMATION32
        {
        [FieldOffset(0x00)] public readonly Int32 NextEntryOffset;
        [FieldOffset(0x04)] public readonly Int32 NumberOfThreads;
        [FieldOffset(0x38)] public readonly UNICODE_STRING32 ImageName;
        [FieldOffset(0x40)] public readonly Int32 BasePriority;
        [FieldOffset(0x44)] public readonly Int32 UniqueProcessId;
        [FieldOffset(0x4c)] public readonly Int32 HandleCount;
        [FieldOffset(0x50)] public readonly Int32 SessionId;
        [FieldOffset(0x58)] public readonly Int32 PeakVirtualSize;
        [FieldOffset(0x5c)] public readonly Int32 VirtualSize;
        [FieldOffset(0x64)] public readonly Int32 PeakWorkingSetSize;
        [FieldOffset(0x68)] public readonly Int32 WorkingSetSize;
        [FieldOffset(0x70)] public readonly Int32 QuotaPagedPoolUsage;
        [FieldOffset(0x78)] public readonly Int32 QuotaNonPagedPoolUsage;
        [FieldOffset(0x7c)] public readonly Int32 PagefileUsage;
        [FieldOffset(0x80)] public readonly Int32 PeakPagefileUsage;
        [FieldOffset(0x84)] public readonly Int32 PrivatePageCount;
        [FieldOffset(0x88)] public readonly Int64 ReadOperationCount;
        [FieldOffset(0x90)] public readonly Int64 WriteOperationCount;
        [FieldOffset(0x98)] public readonly Int64 OtherOperationCount;
        [FieldOffset(0xa0)] public readonly Int64 ReadTransferCount;
        [FieldOffset(0xa8)] public readonly Int64 WriteTransferCount;
        [FieldOffset(0xb0)] public readonly Int64 OtherTransferCount;
        }
    }