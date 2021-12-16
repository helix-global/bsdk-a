using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32.ProcessControl
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PROCESS_BASIC_INFORMATION
        {
        public readonly NTSTATUS ExitStatus;
        public readonly IntPtr PebBaseAddress;
        public readonly UIntPtr AffinityMask;
        public readonly IntPtr BasePriority;
        public readonly UIntPtr UniqueProcessId;
        public readonly UIntPtr InheritedFromUniqueProcessId;
        }
    }