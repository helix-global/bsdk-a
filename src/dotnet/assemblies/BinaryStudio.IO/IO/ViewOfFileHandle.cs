using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace BinaryStudio.IO
    {
    [DebuggerDisplay("{ToString(),nq}")]
    internal sealed class ViewOfFileHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
        internal unsafe void* Memory {
            [SecurityCritical]
            get { return (void*)handle; }
            }

        [SecurityCritical]
        internal ViewOfFileHandle()
            : base(true) {
            }

        [SecurityCritical]
        internal ViewOfFileHandle(IntPtr handle, Boolean ownsHandle)
            : base(ownsHandle) {
            SetHandle(handle);
            }

        #if UBUNTU_16_4
        public Int64 Length { get;set; }
        #endif

        /**
         * <summary>When overridden in a derived class, executes the code required to free the handle.</summary>
         * <returns>true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a releaseHandleFailed MDA Managed Debugging Assistant.</returns>
         * */
        [SecurityCritical]
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        protected override Boolean ReleaseHandle() {
            #if UBUNTU_16_4
            UnmapViewOfFile(handle, (IntPtr)Length);
            Length = 0;
            handle = IntPtr.Zero;
            return true;
            #else
            if (UnmapViewOfFile(handle))
                {
                handle = IntPtr.Zero;
                return true;
                }
            return false;
            #endif
            }

        #if UBUNTU_16_4
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)] [DllImport("c", EntryPoint = "munmap")] private static extern Int32 UnmapViewOfFile(IntPtr lpBaseAddress, IntPtr length);
        #else
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)] [DllImport("kernel32.dll", ExactSpelling = true)] private static extern Boolean UnmapViewOfFile(IntPtr lpBaseAddress);
        #endif

        public static unsafe explicit operator void*(ViewOfFileHandle source)
            {
            return (source.handle != IntPtr.Zero)
                ? (void*)source.handle
                : null;
            }

        public override String ToString()
            {
            return String.Format("${{{0}}}", ((Int64)handle).ToString("X8"));
            }
        }
    }