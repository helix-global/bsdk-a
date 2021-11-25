using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Sequential)]
    public struct CMINVOKECOMMANDINFO
        {
        public Int32 cbSize;
        public Int32 fMask;
        public IntPtr hwnd;
        [MarshalAs(UnmanagedType.LPStr)] public unsafe char* lpVerb;
        [MarshalAs(UnmanagedType.LPStr)] public unsafe char* lpParameters;
        [MarshalAs(UnmanagedType.LPStr)] public unsafe char* lpDirectory;
        public Int32 nShow;
        public Int32 dwHotKey;
        public IntPtr hIcon;
        }
    }