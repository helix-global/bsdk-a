using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("OID"), Guid("ED4E4ED4-FDD8-476E-AED9-5239E7948257")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable|TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface IExtension
        {
        [DispId(0)]
        IOID OID
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(1)]
        Boolean IsCritical
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(2)]
        IEncodedData EncodedData
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }
        }
    }
