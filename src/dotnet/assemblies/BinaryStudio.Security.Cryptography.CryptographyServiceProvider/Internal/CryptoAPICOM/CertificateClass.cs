using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [ClassInterface(ClassInterfaceType.None), Guid("9171C115-7DD9-46BA-B1E5-0ED50AFFC1B8"), TypeLibType(2)]
    [ComImport]
    public class CertificateClass : ICertificate2, ICertContext
        {
        [DispId(1)]
        public virtual extern Int32 Version
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(2)]
        public virtual extern String SerialNumber
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(3)]
        public virtual extern String SubjectName
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(4)]
        public virtual extern String IssuerName
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(5)]
        public virtual extern DateTime ValidFromDate
            {
            [DispId(5)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(6)]
        public virtual extern DateTime ValidToDate
            {
            [DispId(6)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(7)]
        public virtual extern String Thumbprint
            {
            [DispId(7)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(19)]
        public virtual extern Boolean Archived
            {
            [DispId(19)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(19)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(22)]
        public virtual extern IPrivateKey PrivateKey
            {
            [DispId(22)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            [DispId(22)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.Interface)]
            set;
            }

        public virtual extern Int32 CertContext
            {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(10)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern Boolean HasPrivateKey();

        [DispId(11)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String GetInfo([In] CAPICOM_CERT_INFO_TYPE InfoType);

        [DispId(12)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern ICertificateStatus IsValid();

        [DispId(13)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern IKeyUsage KeyUsage();

        [DispId(14)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern IExtendedKeyUsage ExtendedKeyUsage();

        [DispId(15)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern IBasicConstraints BasicConstraints();

        [DispId(16)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String Export([In] CAPICOM_ENCODING_TYPE EncodingType = CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);

        [DispId(17)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Import([MarshalAs(UnmanagedType.BStr)] [In] String EncodedCertificate);

        [DispId(18)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Display();

        [DispId(20)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern ITemplate Template();

        [DispId(21)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern IPublicKey PublicKey();

        [DispId(23)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern IExtensions Extensions();

        [DispId(24)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern IExtendedProperties ExtendedProperties();

        [DispId(25)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Load([MarshalAs(UnmanagedType.BStr)] [In] String FileName, [MarshalAs(UnmanagedType.BStr)] [In] String Password = "", [In] CAPICOM_KEY_STORAGE_FLAG KeyStorageFlag = CAPICOM_KEY_STORAGE_FLAG.CAPICOM_KEY_STORAGE_DEFAULT, [In] CAPICOM_KEY_LOCATION KeyLocation = CAPICOM_KEY_LOCATION.CAPICOM_CURRENT_USER_KEY);

        [DispId(26)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Save([MarshalAs(UnmanagedType.BStr)] [In] String FileName, [MarshalAs(UnmanagedType.BStr)] [In] String Password = "", [In] CAPICOM_CERTIFICATE_SAVE_AS_TYPE SaveAs = CAPICOM_CERTIFICATE_SAVE_AS_TYPE.CAPICOM_CERTIFICATE_SAVE_AS_CER, [In] CAPICOM_CERTIFICATE_INCLUDE_OPTION IncludeOption = CAPICOM_CERTIFICATE_INCLUDE_OPTION.CAPICOM_CERTIFICATE_INCLUDE_END_ENTITY_ONLY);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void FreeContext([In] Int32 pCertContext);
        }
    }
