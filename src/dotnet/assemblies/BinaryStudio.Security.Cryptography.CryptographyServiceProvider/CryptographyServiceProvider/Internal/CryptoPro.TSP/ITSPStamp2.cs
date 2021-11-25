using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CryptoPro.TSP
    {
    [Guid("9AC30674-D6BC-4FD3-AD10-257B8C09550E")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable|TypeLibTypeFlags.FNonExtensible|TypeLibTypeFlags.FDual)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    public interface ITSPStamp2 : ITSPStamp
        {
        [DispId(23)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Import2([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] [In] Array Stamp, [MarshalAs(UnmanagedType.Struct)] [In] Object Request = null);

        [DispId(24)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
        Array Export2();

        [DispId(25)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        Int32 Verify([MarshalAs(UnmanagedType.Struct)] [In] Object TSACertificate = null, [MarshalAs(UnmanagedType.BStr)] [In] String AllowedCriticalExtensions = "");

        [DispId(26)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        Int32 VerifyCertificate2([MarshalAs(UnmanagedType.Struct)] [In] Object TSACertificate = null, [MarshalAs(UnmanagedType.Struct)] [In] Object Store = null);
        }
    }
