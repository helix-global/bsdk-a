using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    //[SecurityCritical]
    //internal class CertificateContextHandle : SafeHandleZeroOrMinusOneIsInvalid
    //    {
    //    internal static CertificateContextHandle InvalidHandle { get {
    //        return new CertificateContextHandle(IntPtr.Zero);
    //        }}

    //    private CertificateContextHandle()
    //        : base(true)
    //        {
    //        #if DEBUG
    //        StackTrace = new StackTrace(true);
    //        #endif
    //        }

    //    internal CertificateContextHandle(IntPtr handle)
    //        : base(true)
    //        {
    //        SetHandle(handle);
    //        #if DEBUG
    //        StackTrace = new StackTrace(true);
    //        #endif
    //        }

    //    #if CAPILITE
    //    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity][DllImport("capi20")] private static extern Boolean CertFreeCertificateContext(IntPtr pCertContext);
    //    #else
    //    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity][DllImport("crypt32.dll", SetLastError = true)] private static extern Boolean CertFreeCertificateContext(IntPtr pCertContext);
    //    #endif
    //    [SecurityCritical]
    //    protected override Boolean ReleaseHandle() {
    //        return CertFreeCertificateContext(handle);
    //        }

    //    public static implicit operator IntPtr(CertificateContextHandle source) { return source.handle; }

    //    /**
    //     * <summary>Returns a string that represents the current object.</summary>
    //     * <returns>A string that represents the current object.</returns>
    //     */
    //    public override String ToString()
    //        {
    //        return $"${(Int64)handle:X8}";
    //        }

    //    #if DEBUG
    //    private StackTrace StackTrace;
    //    #endif
    //    }
    }
