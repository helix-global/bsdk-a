using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace BinaryStudio.Security.SmartCard
    {
    internal class SCardContextHandle: SafeHandleZeroOrMinusOneIsInvalid
        {
        [SecurityCritical]
        internal SCardContextHandle()
            : base(true)
            {
            }

        [SecurityCritical]
        internal SCardContextHandle(IntPtr handle, Boolean ownsHandle)
            : base(ownsHandle)
            {
            SetHandle(handle);
            }

        /**
         * <summary>When overridden in a derived class, executes the code required to free the handle.</summary>
         * <returns>true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a releaseHandleFailed MDA Managed Debugging Assistant.</returns>
         * */
        [SecurityCritical]
        protected override Boolean ReleaseHandle()
            {
            return SCardReleaseContext(handle) == SCARD_S_SUCCESS;
            }

        [DllImport("winscard.dll", SetLastError = true)] [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)] private static extern UInt32 SCardReleaseContext(IntPtr context);

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override unsafe String ToString() {
            return $"${(UInt64)(new UIntPtr((void*)handle)):X8}";
            }

        private const UInt32 SCARD_S_SUCCESS = 0;
        }
    }