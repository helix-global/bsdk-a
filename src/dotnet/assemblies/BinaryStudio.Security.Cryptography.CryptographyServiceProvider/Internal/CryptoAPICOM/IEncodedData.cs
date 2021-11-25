using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS1066

namespace Internal.CryptoAPICOM
    {
    [Guid("D3D460F2-E7F3-4AF3-8EC6-8EB68C61C567")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface IEncodedData
        {
        [DispId(0)]
        String this[[In] CAPICOM_ENCODING_TYPE EncodingType = CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64]
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(1)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String Format([In] Boolean bMultiLines = false);

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.IDispatch)]
        Object Decoder();
        }
    }
