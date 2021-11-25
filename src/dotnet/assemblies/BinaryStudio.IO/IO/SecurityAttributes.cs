using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BinaryStudio.IO
    {
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class SecurityAttributes
        {
        internal Int32 Size = SizeOf();
        internal LocalMemoryHandle SecurityDescriptor;
        internal Int32 InheritHandle;

        [SecuritySafeCritical]
        private static Int32 SizeOf()
            {
            return Marshal.SizeOf(typeof(SecurityAttributes));
            }

        [SecuritySafeCritical]
        public SecurityAttributes()
            {
            SecurityDescriptor = new LocalMemoryHandle();
            }

        [SecurityCritical]
        public void Release()
            {
            if (SecurityDescriptor != null)
                {
                SecurityDescriptor.Dispose();
                SecurityDescriptor = new LocalMemoryHandle();
                }
            }
        }
    }
