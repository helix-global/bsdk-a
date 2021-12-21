using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct PROCESSENTRY32
        {
        private const Int32 MAX_PATH = 260;
        public          Int32  Size;
        public readonly Int32  Usage;
        public readonly Int32  ProcessId;
        public readonly IntPtr DefaultHeapId;
        public readonly Int32  ModuleId;
        public readonly Int32  Threads;
        public readonly Int32  ParentProcessId;
        public readonly Int32  BasePriority;
        public readonly Int32  Flags;
        public unsafe fixed Char ExeFile[MAX_PATH];
        }
    }