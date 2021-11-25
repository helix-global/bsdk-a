using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Certificate"), ClassInterface(ClassInterfaceType.None), Guid("60A9863A-11FD-4080-850E-A8E184FC3A3C"), TypeLibType(2)]
    [ComImport]
    public class SignerClass : ISigner2, ICSigner
        {
        [DispId(0)]
        public virtual extern ICertificate Certificate
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.Interface)]
            set;
            }

        [DispId(1)]
        public virtual extern IAttributes AuthenticatedAttributes
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(2)]
        public virtual extern IChain Chain
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(3)]
        public virtual extern CAPICOM_CERTIFICATE_INCLUDE_OPTION Options
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        public virtual extern Int32 AdditionalStore
            {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Load([MarshalAs(UnmanagedType.BStr)] [In] String FileName, [MarshalAs(UnmanagedType.BStr)] [In] String Password = "");
        }
    }
