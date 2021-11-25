using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [Guid("0BBA0B86-766C-4755-A443-243FF2BD8D29")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable|TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface ICertificate
        {
        [DispId(1)]
        Int32 Version
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(2)]
        String SerialNumber
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(3)]
        String SubjectName
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(4)]
        String IssuerName
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(5)]
        DateTime ValidFromDate
            {
            [DispId(5)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(6)]
        DateTime ValidToDate
            {
            [DispId(6)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(7)]
        String Thumbprint
            {
            [DispId(7)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(10)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        Boolean HasPrivateKey();

        [DispId(11)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String GetInfo([In] CAPICOM_CERT_INFO_TYPE InfoType);

        [DispId(12)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ICertificateStatus IsValid();

        [DispId(13)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IKeyUsage KeyUsage();

        [DispId(14)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IExtendedKeyUsage ExtendedKeyUsage();

        [DispId(15)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IBasicConstraints BasicConstraints();

        [DispId(16)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String Export([In] CAPICOM_ENCODING_TYPE EncodingType = CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);

        [DispId(17)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Import([MarshalAs(UnmanagedType.BStr)] [In] String EncodedCertificate);

        [DispId(18)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Display();
        }
    }
