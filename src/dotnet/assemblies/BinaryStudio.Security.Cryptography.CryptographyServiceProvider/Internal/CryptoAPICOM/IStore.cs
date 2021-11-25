using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Certificates"), Guid("E860EF75-1B63-4254-AF47-960DAA3DD337")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    public interface IStore
        {
        [DispId(0)]
        ICertificates Certificates
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(1)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Open([In] CAPICOM_STORE_LOCATION StoreLocation = CAPICOM_STORE_LOCATION.CAPICOM_CURRENT_USER_STORE, [MarshalAs(UnmanagedType.BStr)] [In] String StoreName = "My", [In] CAPICOM_STORE_OPEN_MODE OpenMode = CAPICOM_STORE_OPEN_MODE.CAPICOM_STORE_OPEN_READ_ONLY);

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Add([MarshalAs(UnmanagedType.Interface)] [In] ICertificate pVal);

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Remove([MarshalAs(UnmanagedType.Interface)] [In] ICertificate pVal);

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String Export([In] CAPICOM_STORE_SAVE_AS_TYPE SaveAs = CAPICOM_STORE_SAVE_AS_TYPE.CAPICOM_STORE_SAVE_AS_SERIALIZED, [In] CAPICOM_ENCODING_TYPE EncodingType = CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);

        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Import([MarshalAs(UnmanagedType.BStr)] [In] String EncodedStore);
        }
    }
