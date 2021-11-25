using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("ContainerName"), Guid("659DEDC3-6C85-42DB-8527-EFCB21742862")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface IPrivateKey
        {
        [DispId(0)]
        String ContainerName
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(1)]
        String UniqueContainerName
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(2)]
        String ProviderName
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(3)]
        CAPICOM_PROV_TYPE ProviderType
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(4)]
        CAPICOM_KEY_SPEC KeySpec
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        Boolean IsAccessible();

        [DispId(6)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        Boolean IsProtected();

        [DispId(7)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        Boolean IsExportable();

        [DispId(8)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        Boolean IsRemovable();

        [DispId(9)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        Boolean IsMachineKeyset();

        [DispId(10)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        Boolean IsHardwareDevice();

        [DispId(11)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Open([MarshalAs(UnmanagedType.BStr)] [In] String ContainerName, [MarshalAs(UnmanagedType.BStr)] [In] String ProviderName = "Microsoft Enhanced Cryptographic Provider v1.0", [In] CAPICOM_PROV_TYPE ProviderType = CAPICOM_PROV_TYPE.CAPICOM_PROV_RSA_FULL, [In] CAPICOM_KEY_SPEC KeySpec = CAPICOM_KEY_SPEC.CAPICOM_KEY_SPEC_SIGNATURE, [In] CAPICOM_STORE_LOCATION StoreLocation = CAPICOM_STORE_LOCATION.CAPICOM_CURRENT_USER_STORE, [In] Boolean bCheckExistence = false);

        [DispId(12)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Delete();
        }
    }
