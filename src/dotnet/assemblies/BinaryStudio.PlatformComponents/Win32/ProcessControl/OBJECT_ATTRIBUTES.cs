using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct OBJECT_ATTRIBUTES
        {
        public Int32  Length;
        public IntPtr RootDirectory;
        public IntPtr ObjectName;
        public Int32  Attributes;
        public IntPtr SecurityDescriptor;
        public IntPtr SecurityQualityOfService;
        }
    }