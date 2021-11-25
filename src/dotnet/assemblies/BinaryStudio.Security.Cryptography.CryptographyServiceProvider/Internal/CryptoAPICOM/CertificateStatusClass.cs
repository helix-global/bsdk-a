using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Result"), ClassInterface(ClassInterfaceType.None), Guid("0EF24D18-BD9B-47D4-9458-E05B489FB7BA")]
    [ComImport]
    public class CertificateStatusClass : ICertificateStatus3
        {
        [DispId(0)]
        public virtual extern Boolean Result
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(1)]
        public virtual extern CAPICOM_CHECK_FLAG CheckFlag
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(3)]
        public virtual extern DateTime VerificationTime
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(4)]
        public virtual extern Int32 UrlRetrievalTimeout
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(7)]
        public virtual extern ICertificates ValidationCertificates
            {
            [DispId(7)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern IEKU EKU();

        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern IOIDs CertificatePolicies();

        [DispId(6)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern IOIDs ApplicationPolicies();
        }
    }
