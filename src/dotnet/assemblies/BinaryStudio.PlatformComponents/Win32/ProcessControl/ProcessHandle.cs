using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace BinaryStudio.PlatformComponents.Win32
    {
    public sealed class ProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
        public unsafe Boolean IsWow64Process { get {
            if (iswow64 == null) {
                if (!IsInvalid) {
                    var r = UIntPtr.Zero;
                    var ns = NtQueryInformationProcess(this, ProcessWow64Information, &r, (UInt32)IntPtr.Size, null);
                    if (ns == STATUS_SUCCESS) {
                        iswow64 = (r != UIntPtr.Zero);
                        }
                    }
                }
            return iswow64.GetValueOrDefault();
            }}
        public IntPtr Handle { get { return handle; }}
        internal static ProcessHandle InvalidHandle { get {
            return new ProcessHandle(IntPtr.Zero);
            }}

        private ProcessHandle()
            : base(true)
            {
            }

        internal unsafe ProcessHandle(IntPtr handle)
            : base(true)
            {
            SetHandle(handle);
            }

        [SecurityCritical]
        protected override Boolean ReleaseHandle()
            {
            return CloseHandle(handle);
            }

        private Boolean? iswow64;
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)][DllImport("kernel32.dll", SetLastError = true)] internal static extern Boolean CloseHandle(IntPtr handle);
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe UInt32 NtQueryInformationProcess(ProcessHandle process, UInt32 iclass, void* pi, UInt32 pisz, UInt32* r);

        private const UInt32 ProcessWow64Information = 26;
        private const UInt32 STATUS_SUCCESS = 0;
        }

    }