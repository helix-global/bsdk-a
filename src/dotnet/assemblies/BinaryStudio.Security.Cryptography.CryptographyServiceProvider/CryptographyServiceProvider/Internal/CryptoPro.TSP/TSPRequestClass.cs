using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.CryptoAPICOM;

namespace CryptoPro.TSP
    {
    [ClassInterface(ClassInterfaceType.None), Guid("9519B122-E646-410C-9D6F-B228F81AEFE8"), TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [ComImport]
    public class TSPRequestClass : ITSPRequest
        {
        [DispId(1)]
        public virtual extern String TSAAddress
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
            }

        [DispId(2)]
        public virtual extern Boolean CertReq
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(3)]
        public virtual extern Boolean UseNonce
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(4)]
        public virtual extern String TSAPassword
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
            }

        [DispId(5)]
        public virtual extern String PolicyID
            {
            [DispId(5)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [DispId(5)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
            }

        [DispId(6)]
        public virtual extern String ProxyAddress
            {
            [DispId(6)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [DispId(6)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
            }

        [DispId(7)]
        public virtual extern String ProxyPassword
            {
            [DispId(7)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [DispId(7)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
            }

        [DispId(8)]
        public virtual extern String ProxyUserName
            {
            [DispId(8)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [DispId(8)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
            }

        [DispId(9)]
        public virtual extern String TSAUserName
            {
            [DispId(9)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [DispId(9)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
            }

        [DispId(11)]
        public virtual extern IHashedData Hash
            {
            [DispId(11)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.Interface)]
            set;
            }

        [DispId(13)]
        public virtual extern ICertContext ClientCertificate
            {
            [DispId(13)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            [DispId(13)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.Interface)]
            set;
            }

        [ComAliasName("TSPCOM.TSPCOM_AUTH_TYPE"), DispId(14)]
        public virtual extern TSPCOM_AUTH_TYPE TSAAuthType
            {
            [DispId(14)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: ComAliasName("TSPCOM.TSPCOM_AUTH_TYPE")]
            get;
            [DispId(14)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: ComAliasName("TSPCOM.TSPCOM_AUTH_TYPE")]
            set;
            }

        [ComAliasName("TSPCOM.TSPCOM_AUTH_TYPE"), DispId(15)]
        public virtual extern TSPCOM_AUTH_TYPE ProxyAuthType
            {
            [DispId(15)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: ComAliasName("TSPCOM.TSPCOM_AUTH_TYPE")]
            get;
            [DispId(15)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: ComAliasName("TSPCOM.TSPCOM_AUTH_TYPE")]
            set;
            }

        [DispId(17)]
        public virtual extern String HashValue
            {
            [DispId(17)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(18)]
        public virtual extern IOID HashAlgorithm
            {
            [DispId(18)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(19)]
        public virtual extern Int32 HTTPStatus
            {
            [DispId(19)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(16)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Import([MarshalAs(UnmanagedType.BStr)] [In] String strRequest);

        [DispId(12)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String Export();

        [DispId(10)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern ITSPStamp Send([In] Boolean Verify = false);

        [DispId(20)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Display([In] Int32 hwndParent = 0, [MarshalAs(UnmanagedType.BStr)] [In] String Title = "");
        }
    }
