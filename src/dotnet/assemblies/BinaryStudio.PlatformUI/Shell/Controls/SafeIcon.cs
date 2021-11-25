using System;
using Microsoft.Win32.SafeHandles;

namespace BinaryStudio.PlatformUI
    {
    public class SafeIcon : SafeHandleZeroOrMinusOneIsInvalid
        {
        private SafeIcon() : base(true) {
            }

        public SafeIcon(IntPtr icon, Boolean ownsHandle) : base(ownsHandle) {
            SetHandle(icon);
            }

        protected override Boolean ReleaseHandle() {
            return NativeMethods.DestroyIcon(handle);
            }
        }
    }