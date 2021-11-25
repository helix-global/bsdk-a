using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("PropID"), Guid("ECB8A5C8-562C-4989-B49D-FA37D40F8FC4")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface IExtendedProperty
        {
        [DispId(0)]
        CAPICOM_PROPID PropID
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(1)]
        String Value
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
