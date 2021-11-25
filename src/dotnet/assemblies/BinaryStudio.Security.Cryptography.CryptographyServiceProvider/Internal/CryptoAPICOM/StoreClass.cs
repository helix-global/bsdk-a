using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Certificates"), ClassInterface(ClassInterfaceType.None), Guid("91D221C4-0CD4-461C-A728-01D509321556"), TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [ComImport]
    public class StoreClass : IStore3, ICertStore
        {
        [DispId(0)]
        public virtual extern ICertificates Certificates
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(7)]
        public virtual extern String Name
            {
            [DispId(7)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(8)]
        public virtual extern CAPICOM_STORE_LOCATION Location
            {
            [DispId(8)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        public virtual extern Int32 StoreHandle
            {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        public virtual extern CAPICOM_STORE_LOCATION StoreLocation
            {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(1)]
        //[MethodImpl(MethodImplOptions.InternalCall)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void Open([In] CAPICOM_STORE_LOCATION StoreLocation, [MarshalAs(UnmanagedType.BStr)] [In] String StoreName, [In] CAPICOM_STORE_OPEN_MODE OpenMode);

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Add([MarshalAs(UnmanagedType.Interface)] [In] ICertificate pVal);

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Remove([MarshalAs(UnmanagedType.Interface)] [In] ICertificate pVal);

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String Export([In] CAPICOM_STORE_SAVE_AS_TYPE SaveAs = CAPICOM_STORE_SAVE_AS_TYPE.CAPICOM_STORE_SAVE_AS_SERIALIZED, [In] CAPICOM_ENCODING_TYPE EncodingType = CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);

        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Import([MarshalAs(UnmanagedType.BStr)] [In] String EncodedStore);

        [DispId(6)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Load([MarshalAs(UnmanagedType.BStr)] [In] String FileName, [MarshalAs(UnmanagedType.BStr)] [In] String Password = "", [In] CAPICOM_KEY_STORAGE_FLAG KeyStorageFlag = CAPICOM_KEY_STORAGE_FLAG.CAPICOM_KEY_STORAGE_DEFAULT);

        [DispId(9)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern Boolean Delete();

        [DispId(10)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Close();

        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void CloseHandle([In] Int32 hCertStore);
        }
    }
