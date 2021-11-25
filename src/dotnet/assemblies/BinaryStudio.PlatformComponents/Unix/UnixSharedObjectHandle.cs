using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace BinaryStudio.PlatformComponents.Unix
    {
    internal class UnixSharedObjectHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
        internal static UnixSharedObjectHandle InvalidHandle { get {
            return new UnixSharedObjectHandle(IntPtr.Zero);
            }}

        private UnixSharedObjectHandle()
            : base(true)
            {
            }

        internal UnixSharedObjectHandle(IntPtr handle)
            : base(true)
            {
            SetHandle(handle);
            }

        [DllImport("ld", CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "dlclose")]  private static extern Int32 FreeLibrary(IntPtr module);
        [SecurityCritical]
        protected override Boolean ReleaseHandle()
            {
            return FreeLibrary(handle) == 0;
            }

        public static implicit operator IntPtr(UnixSharedObjectHandle source) { return source.handle; }
        public override String ToString()
            {
            return (IntPtr.Size == 4)
                ? $"${(Int64)handle:X8}"
                : $"${(Int64)handle:X16}";
            }
        }

    }