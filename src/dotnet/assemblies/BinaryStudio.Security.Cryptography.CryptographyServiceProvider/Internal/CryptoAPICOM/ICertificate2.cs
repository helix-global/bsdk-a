using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [Guid("6FE450DC-AD32-48D4-A366-01EE7E0B1374")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface ICertificate2 : ICertificate
        {
        [DispId(19)]
        Boolean Archived
            {
            [DispId(19)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(19)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(22)]
        IPrivateKey PrivateKey
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

        [DispId(20)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ITemplate Template();

        [DispId(21)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IPublicKey PublicKey();

        [DispId(23)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IExtensions Extensions();

        [DispId(24)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IExtendedProperties ExtendedProperties();

        [DispId(25)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Load([MarshalAs(UnmanagedType.BStr)] [In] String FileName, [MarshalAs(UnmanagedType.BStr)] [In] String Password = "", [In] CAPICOM_KEY_STORAGE_FLAG KeyStorageFlag = CAPICOM_KEY_STORAGE_FLAG.CAPICOM_KEY_STORAGE_DEFAULT, [In] CAPICOM_KEY_LOCATION KeyLocation = CAPICOM_KEY_LOCATION.CAPICOM_CURRENT_USER_KEY);

        [DispId(26)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Save([MarshalAs(UnmanagedType.BStr)] [In] String FileName, [MarshalAs(UnmanagedType.BStr)] [In] String Password = "", [In] CAPICOM_CERTIFICATE_SAVE_AS_TYPE SaveAs = CAPICOM_CERTIFICATE_SAVE_AS_TYPE.CAPICOM_CERTIFICATE_SAVE_AS_CER, [In] CAPICOM_CERTIFICATE_INCLUDE_OPTION IncludeOption = CAPICOM_CERTIFICATE_INCLUDE_OPTION.CAPICOM_CERTIFICATE_INCLUDE_END_ENTITY_ONLY);
        }
    }
