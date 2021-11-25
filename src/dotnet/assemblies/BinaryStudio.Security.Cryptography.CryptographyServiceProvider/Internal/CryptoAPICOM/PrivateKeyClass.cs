using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("ContainerName"), ClassInterface(ClassInterfaceType.None), ComConversionLoss, Guid("03ACC284-B757-4B8F-9951-86E600D2CD06"), TypeLibType(2)]
    [ComImport]
    public class PrivateKeyClass : IPrivateKey, ICPrivateKey
        {
        [DispId(0)]
        public virtual extern String ContainerName
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(1)]
        public virtual extern String UniqueContainerName
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(2)]
        public virtual extern String ProviderName
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(3)]
        public virtual extern CAPICOM_PROV_TYPE ProviderType
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(4)]
        public virtual extern CAPICOM_KEY_SPEC KeySpec
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern Boolean IsAccessible();

        [DispId(6)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern Boolean IsProtected();

        [DispId(7)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern Boolean IsExportable();

        [DispId(8)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern Boolean IsRemovable();

        [DispId(9)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern Boolean IsMachineKeyset();

        [DispId(10)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern Boolean IsHardwareDevice();

        [DispId(11)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Open([MarshalAs(UnmanagedType.BStr)] [In] String ContainerName, [MarshalAs(UnmanagedType.BStr)] [In] String ProviderName = "Microsoft Enhanced Cryptographic Provider v1.0", [In] CAPICOM_PROV_TYPE ProviderType = CAPICOM_PROV_TYPE.CAPICOM_PROV_RSA_FULL, [In] CAPICOM_KEY_SPEC KeySpec = CAPICOM_KEY_SPEC.CAPICOM_KEY_SPEC_SIGNATURE, [In] CAPICOM_STORE_LOCATION StoreLocation = CAPICOM_STORE_LOCATION.CAPICOM_CURRENT_USER_STORE, [In] Boolean bCheckExistence = false);

        [DispId(12)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Delete();

        [TypeLibFunc(1)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern IntPtr _GetKeyProvInfo();

        [TypeLibFunc(1)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern IntPtr _GetKeyContext();
        }
    }
