using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace BinaryStudio.PlatformComponents.Win32
    {
    public sealed class Win32SharedObject : SharedObject
        {
        public Win32SharedObject(String filename)
            : base(filename)
            {
            }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)] private static extern IntPtr LoadLibraryExW([In] String lpwLibFileName, [In] IntPtr hFile, [In] UInt32 dwFlags);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi,    SetLastError = true)] private static extern IntPtr GetProcAddress(SafeHandleZeroOrMinusOneIsInvalid module, String name);

        private const UInt32 LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008;
        private const UInt32 LOAD_IGNORE_CODE_AUTHZ_LEVEL  = 0x00000010;

        #region M:EnsureOverride:SafeHandleZeroOrMinusOneIsInvalid
        protected override SafeHandleZeroOrMinusOneIsInvalid EnsureOverride()
            {
            var r = LoadLibraryExW(FileName, IntPtr.Zero, LOAD_IGNORE_CODE_AUTHZ_LEVEL | LOAD_WITH_ALTERED_SEARCH_PATH);
            if (r == IntPtr.Zero)
                {
                throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            return new Win32SharedObjectHandle(r);
            }
        #endregion
        #region M:Get(String):IntPtr
        public override IntPtr Get(String methodname)
            {
            if (methodname == null) { throw new ArgumentNullException(nameof(methodname)); }
            return GetProcAddress(Handle, methodname);
            }
        #endregion
        }
    }