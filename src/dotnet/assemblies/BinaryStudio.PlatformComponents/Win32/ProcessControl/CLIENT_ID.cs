using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Sequential)]
    public struct CLIENT_ID
        {
        public  IntPtr UniqueProcess;
        private IntPtr UniqueThread;
        }
    }