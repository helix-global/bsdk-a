using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Value"), Guid("9F7F23E8-06F4-42E8-B965-5CBD044BF27F")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface IHashedData
        {
        [DispId(0)]
        String Value
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(1)]
        CAPICOM_HASH_ALGORITHM Algorithm
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Hash([MarshalAs(UnmanagedType.BStr)] [In] String newVal);
        }
    }
