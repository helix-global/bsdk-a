using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Algorithm"), Guid("72BF9ADA-6817-4C31-B43E-25F7C7B091F4")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable|TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface IPublicKey
        {
        [DispId(0)]
        IOID Algorithm
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(1)]
        Int32 Length
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(2)]
        IEncodedData EncodedKey
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(3)]
        IEncodedData EncodedParameters
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }
        }
    }
