using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Explicit, Pack = 8)]
    public struct SYSTEM_PROCESS_INFORMATION64
        {
        [FieldOffset(0x00)] public readonly Int32 NextEntryOffset;
        [FieldOffset(0x04)] public readonly Int32 NumberOfThreads;
        [FieldOffset(0x38)] public readonly UNICODE_STRING64 ImageName;
        [FieldOffset(0x48)] public readonly Int32 BasePriority;
        [FieldOffset(0x50)] public readonly Int64 UniqueProcessId;
        [FieldOffset(0x60)] public readonly Int32 HandleCount;
        [FieldOffset(0x64)] public readonly Int32 SessionId;
        [FieldOffset(0x70)] public readonly Int64 PeakVirtualSize;
        [FieldOffset(0x78)] public readonly Int64 VirtualSize;
        [FieldOffset(0x88)] public readonly Int64 PeakWorkingSetSize;
        [FieldOffset(0x90)] public readonly Int64 WorkingSetSize;
        [FieldOffset(0xa0)] public readonly Int64 QuotaPagedPoolUsage;
        [FieldOffset(0xb0)] public readonly Int64 QuotaNonPagedPoolUsage;
        [FieldOffset(0xb8)] public readonly Int64 PagefileUsage;
        [FieldOffset(0xc0)] public readonly Int64 PeakPagefileUsage;
        [FieldOffset(0xc8)] public readonly Int64 PrivatePageCount;
        [FieldOffset(0xd0)] public readonly Int64 ReadOperationCount;
        [FieldOffset(0xd8)] public readonly Int64 WriteOperationCount;
        [FieldOffset(0xe0)] public readonly Int64 OtherOperationCount;
        [FieldOffset(0xe8)] public readonly Int64 ReadTransferCount;
        [FieldOffset(0xf0)] public readonly Int64 WriteTransferCount;
        [FieldOffset(0xf8)] public readonly Int64 OtherTransferCount;
        }
    }