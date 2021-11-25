using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    //#if CODE_ANALYSIS
    //[SuppressMessage("Design", "CA1060:Move pinvokes to native methods class", Justification = "<Pending>")]
    //#endif
    //internal class CryptHashAlgorithmHandle : SafeHandleZeroOrMinusOneIsInvalid
    //    {
    //    internal static CryptHashAlgorithmHandle InvalidHandle { get {
    //        return new CryptHashAlgorithmHandle(IntPtr.Zero);
    //        }}

    //    private CryptHashAlgorithmHandle()
    //        : base(true)
    //        {
    //        }

    //    internal CryptHashAlgorithmHandle(IntPtr handle)
    //        : base(true)
    //        {
    //        SetHandle(handle);
    //        }

    //    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity][DllImport("advapi32.dll")] private static extern Boolean CryptDestroyHash(IntPtr handle);
    //    [SecurityCritical]
    //    protected override Boolean ReleaseHandle() {
    //        using (TraceManager.Instance.Trace(nameof(CryptDestroyHash), TRACE_CRYPTDESTROYHASH)) {
    //            return CryptDestroyHash(handle);
    //            }
    //        }

    //    private static readonly String TRACE_CRYPTDESTROYHASH = $"CSP::{nameof(CryptDestroyHash)}(HCRYPTHASH):BOOL";

    //    public static implicit operator IntPtr(CryptHashAlgorithmHandle source) { return source.handle; }
    //    public override String ToString()
    //        {
    //        return $"${(Int64)handle:X8}";
    //        }
    //    }
    }