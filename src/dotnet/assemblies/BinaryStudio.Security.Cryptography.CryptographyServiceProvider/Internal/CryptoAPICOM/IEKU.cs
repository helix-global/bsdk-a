using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Name"), Guid("976B7E6D-1002-4051-BFD4-824A74BD74E2")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface IEKU
        {
        [DispId(0)]
        CAPICOM_EKU Name
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(1)]
        String OID
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
        }
    }
