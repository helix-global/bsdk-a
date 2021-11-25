using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.CryptoAPICOM;

namespace CryptoPro.TSP
    {
    [Guid("D493E84E-4055-4691-AE63-8B6309AAB3AB")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible | TypeLibTypeFlags.FDual)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    #if CODE_ANALYSIS
    [SuppressMessage("Design", "CA1044:Properties should not be write only", Justification = "<Pending>")]
    #endif
    public interface ITSPRequest
        {
        [DispId(1)]
        String TSAAddress
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
        Boolean CertReq
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(3)]
        Boolean UseNonce
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(4)]
        String TSAPassword
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
        String PolicyID
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
        String ProxyAddress
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
        String ProxyPassword
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
        String ProxyUserName
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
        String TSAUserName
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
        IHashedData Hash
            {
            [DispId(11)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.Interface)]
            set;
            }

        [DispId(13)]
        ICertContext ClientCertificate
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
        TSPCOM_AUTH_TYPE TSAAuthType
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
        TSPCOM_AUTH_TYPE ProxyAuthType
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
        String HashValue
            {
            [DispId(17)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(18)]
        IOID HashAlgorithm
            {
            [DispId(18)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(19)]
        Int32 HTTPStatus
            {
            [DispId(19)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(16)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Import([MarshalAs(UnmanagedType.BStr)] [In] String strRequest);

        [DispId(12)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String Export();

        [DispId(10)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ITSPStamp Send([In] Boolean Verify = false);

        [DispId(20)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Display([In] Int32 hwndParent = 0, [MarshalAs(UnmanagedType.BStr)] [In] String Title = "");
        }
    }
