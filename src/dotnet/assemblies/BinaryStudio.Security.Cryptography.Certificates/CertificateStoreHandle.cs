using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    //internal class CertificateStoreHandle : SafeHandleZeroOrMinusOneIsInvalid
    //    {
    //    public CertificateStoreHandle()
    //        : base(true)
    //        {
    //        }

    //    public CertificateStoreHandle(IntPtr handle)
    //        : base(true)
    //        {
    //        SetHandle(handle);
    //        }

    //    protected override Boolean ReleaseHandle()
    //        {
    //        return (handle != IntPtr.Zero) && CertCloseStore(handle, CERT_CLOSE_STORE_NONE_FLAG);
    //        }

    //    private const UInt32 CERT_CLOSE_STORE_NONE_FLAG  = 0x00000000;
    //    private const UInt32 CERT_CLOSE_STORE_FORCE_FLAG = CERT_CLOSE_STORE_NONE_FLAG  + 1;
    //    private const UInt32 CERT_CLOSE_STORE_CHECK_FLAG = CERT_CLOSE_STORE_FORCE_FLAG + 1;

    //    [DllImport("crypt32.dll", SetLastError = true)] internal static extern Boolean CertCloseStore(IntPtr handle, UInt32 flags);

    //    /**
    //     * <summary>Returns a string that represents the current object.</summary>
    //     * <returns>A string that represents the current object.</returns>
    //     */
    //    public override String ToString()
    //        {
    //        return $"${(Int64)handle:X8}";
    //        }

    //    public static implicit operator IntPtr(CertificateStoreHandle source) { return source.handle; }
    //    }
    }
