using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct ProcessBasicInformation
        {
        public readonly NTSTATUS ExitStatus;
        public readonly IntPtr   PebBaseAddress;
        public readonly IntPtr   AffinityMask;
        public readonly IntPtr   BasePriority;
        public readonly IntPtr   UniqueProcessId;
        public readonly IntPtr   InheritedFromUniqueProcessId;
        }
    }