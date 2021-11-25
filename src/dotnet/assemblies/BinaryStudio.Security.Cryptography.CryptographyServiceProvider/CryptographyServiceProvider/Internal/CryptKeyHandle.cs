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
    //internal class CryptKeyHandle : SafeHandleZeroOrMinusOneIsInvalid
    //    {
    //    internal static CryptKeyHandle InvalidHandle { get {
    //        return new CryptKeyHandle(IntPtr.Zero);
    //        }}

    //    private CryptKeyHandle()
    //        : base(true)
    //        {
    //        }

    //    internal CryptKeyHandle(IntPtr handle)
    //        : base(true)
    //        {
    //        SetHandle(handle);
    //        }

    //    #if CAPILITE
    //    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity][DllImport("capi20")] private static extern Boolean CryptDestroyKey(IntPtr handle);
    //    #else
    //    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity][DllImport("advapi32.dll")] private static extern Boolean CryptDestroyKey(IntPtr handle);
    //    #endif

    //    [SecurityCritical]
    //    protected override Boolean ReleaseHandle()
    //        {
    //        return CryptDestroyKey(handle);
    //        }

    //    public static implicit operator IntPtr(CryptKeyHandle source) { return source.handle; }

    //    /**
    //     * <summary>Returns a string that represents the current object.</summary>
    //     * <returns>A string that represents the current object.</returns>
    //     */
    //    public override String ToString()
    //        {
    //        return $"${(Int64)handle:X8}";
    //        }
    //    }
    }