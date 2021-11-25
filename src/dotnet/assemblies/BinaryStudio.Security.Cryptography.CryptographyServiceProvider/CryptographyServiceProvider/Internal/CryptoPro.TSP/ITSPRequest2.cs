using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.CryptoAPICOM;

namespace CryptoPro.TSP
    {
    [Guid("DB3BE9AB-F041-48E0-9864-B088A471A4EB")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible | TypeLibTypeFlags.FDual)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    public interface ITSPRequest2 : ITSPRequest
        {
        [DispId(23)]
        ICertificate ClientCertificate2
            {
            [DispId(23)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            [DispId(23)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.Interface)]
            set;
            }

        [DispId(26)]
        IOID HashAlgorithm2
            {
            [DispId(26)]
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            [DispId(26)]
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            [param: MarshalAs(UnmanagedType.Interface)]
            set;
            }

        [DispId(21)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Import2([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] [In] Array Request);

        [DispId(22)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
        Array Export2();

        [DispId(24)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void AddData([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] [In] Array Data);

        [DispId(25)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void ResetData();
        }
    }
