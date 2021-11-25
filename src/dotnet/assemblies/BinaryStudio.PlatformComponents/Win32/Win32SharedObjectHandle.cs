using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace BinaryStudio.PlatformComponents.Win32
    {
    internal sealed class Win32SharedObjectHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
        internal static Win32SharedObjectHandle InvalidHandle { get {
            return new Win32SharedObjectHandle(IntPtr.Zero);
            }}

        private Win32SharedObjectHandle()
            : base(true)
            {
            }

        internal Win32SharedObjectHandle(IntPtr handle)
            : base(true)
            {
            SetHandle(handle);
            }

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]  private static extern Boolean FreeLibrary(IntPtr module);
        [SecurityCritical]
        protected override Boolean ReleaseHandle()
            {
            return FreeLibrary(handle);
            }

        public static implicit operator IntPtr(Win32SharedObjectHandle source) { return source.handle; }
        public override String ToString()
            {
            return $"${(Int64)handle:X8}";
            }
        }
    }