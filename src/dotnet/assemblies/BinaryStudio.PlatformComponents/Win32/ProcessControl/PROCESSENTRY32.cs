using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct PROCESSENTRY32
        {
        private const Int32 MAX_PATH = 260;
        public Int32    Size;
        public readonly Int32  cntUsage;
        public readonly Int32  ProcessId;
        public readonly IntPtr th32DefaultHeapID;
        public readonly Int32  th32ModuleID;
        public readonly Int32  cntThreads;
        public readonly Int32  th32ParentProcessID;
        public readonly Int32  pcPriClassBase;
        public readonly Int32  dwFlags;
        public unsafe fixed Char ExeFile[MAX_PATH];
        }
    }