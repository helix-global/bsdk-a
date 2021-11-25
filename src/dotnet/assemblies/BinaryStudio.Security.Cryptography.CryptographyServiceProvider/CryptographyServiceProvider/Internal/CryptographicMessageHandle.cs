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
    //internal class CryptographicMessageHandle : SafeHandleZeroOrMinusOneIsInvalid
    //    {
    //    internal static CryptographicMessageHandle InvalidHandle { get {
    //        return new CryptographicMessageHandle(IntPtr.Zero);
    //        }}

    //    private CryptographicMessageHandle()
    //        : base(true)
    //        {
    //        }

    //    internal CryptographicMessageHandle(IntPtr handle)
    //        : base(true)
    //        {
    //        SetHandle(handle);
    //        }

    //    public CryptographicMessageHandle Clone() { return new CryptographicMessageHandle(CryptMsgDuplicate(handle)); }

    //    #if CAPILITE
    //    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity][DllImport("capi20")]private static extern Boolean CryptMsgClose(IntPtr handle);
    //    [DllImport("capi20")] private static extern IntPtr CryptMsgDuplicate(IntPtr handle);
    //    #else
    //    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity][DllImport("crypt32.dll", SetLastError = true)]private static extern Boolean CryptMsgClose(IntPtr handle);
    //    [DllImport("crypt32.dll", SetLastError = true)] private static extern IntPtr CryptMsgDuplicate(IntPtr handle);
    //    #endif

    //    [SecurityCritical]
    //    protected override Boolean ReleaseHandle()
    //        {
    //        return CryptMsgClose(handle);
    //        }

    //    /**
    //     * <summary>Returns a string that represents the current object.</summary>
    //     * <returns>A string that represents the current object.</returns>
    //     */
    //    public override String ToString()
    //        {
    //        return $"${(Int64)handle:X8}";
    //        }

    //    public static implicit operator IntPtr(CryptographicMessageHandle source)
    //        {
    //        return (source != null)
    //            ? source.handle
    //            : IntPtr.Zero;
    //        }
    //    }
    }