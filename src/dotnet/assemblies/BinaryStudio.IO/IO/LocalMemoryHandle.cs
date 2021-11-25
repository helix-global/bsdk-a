using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace BinaryStudio.IO
    {
    public sealed class LocalMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
        public static LocalMemoryHandle InvalidHandle { get { return new LocalMemoryHandle(IntPtr.Zero); } }

        [SecurityCritical]
        public LocalMemoryHandle()
            : base(true)
            {
            }

        internal LocalMemoryHandle(IntPtr handle)
            : base(true)
            {
            SetHandle(handle);
            }

        [SecurityCritical]
        public LocalMemoryHandle(IntPtr existingHandle, Boolean ownsHandle)
            : base(ownsHandle)
            {
            SetHandle(existingHandle);
            }

        /**
         * <summary>When overridden in a derived class, executes the code required to free the handle.</summary>
         * <returns>true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a releaseHandleFailed MDA Managed Debugging Assistant.</returns>
         * */
        [SecurityCritical]
        protected override Boolean ReleaseHandle()
            {
            return LocalFree(handle) == IntPtr.Zero;
            }

        public static unsafe implicit operator void*(LocalMemoryHandle source) { return (void*)source.handle; }
        public static unsafe implicit operator IntPtr(LocalMemoryHandle source) { return source.handle; }

        [DllImport("kernel32.dll")] [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecurityCritical, SuppressUnmanagedCodeSecurity] private static extern IntPtr LocalFree(IntPtr hMem);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern LocalMemoryHandle LocalAlloc([In] UInt32 flags, [In] IntPtr size);
        internal const UInt32 LMEM_ZEROINIT = 0x0040;

        public static LocalMemoryHandle Alloc(IntPtr size) { return LocalAlloc(LMEM_ZEROINIT, size); }
        public static LocalMemoryHandle Alloc(Int32  size) { return LocalAlloc(LMEM_ZEROINIT, (IntPtr)size); }
        }
    }
