using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Result"), Guid("BF95660E-F743-4EAC-9DE5-960787A4606C")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface ICertificateStatus2 : ICertificateStatus
        {
        [DispId(3)]
        DateTime VerificationTime
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(4)]
        Int32 UrlRetrievalTimeout
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IOIDs CertificatePolicies();

        [DispId(6)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IOIDs ApplicationPolicies();
        }
    }
