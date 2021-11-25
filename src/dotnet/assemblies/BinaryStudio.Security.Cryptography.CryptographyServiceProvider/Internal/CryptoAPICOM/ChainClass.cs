using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Certificates"), ClassInterface(ClassInterfaceType.None), Guid("550C8FFB-4DC0-4756-828C-862E6D0AE74F"), TypeLibType(2)]
    [ComImport]
    public class ChainClass : IChain2, IChainContext
        {
        [DispId(0)]
        public virtual extern ICertificates Certificates
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(1)]
        public virtual extern Int32 Status
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        public virtual extern Int32 ChainContext
            {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern Boolean Build([MarshalAs(UnmanagedType.Interface)] [In] ICertificate pICertificate);

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern IOIDs CertificatePolicies();

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern IOIDs ApplicationPolicies();

        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String ExtendedErrorInfo([In] Int32 Index = 1);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void FreeContext([In] Int32 pChainContext);
        }
    }
