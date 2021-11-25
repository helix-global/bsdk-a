using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [Guid("EB166CF6-2AE6-44DA-BD96-0C1635D183FE")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface IUtilities
        {
        [DispId(1)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String GetRandom([In] Int32 Length = 8, [In] CAPICOM_ENCODING_TYPE EncodingType = CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BINARY);

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String Base64Encode([MarshalAs(UnmanagedType.BStr)] [In] String SrcString);

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String Base64Decode([MarshalAs(UnmanagedType.BStr)] [In] String EncodedString);

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String BinaryToHex([MarshalAs(UnmanagedType.BStr)] [In] String BinaryString);

        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String HexToBinary([MarshalAs(UnmanagedType.BStr)] [In] String HexString);

        [DispId(6)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Struct)]
        Object BinaryStringToByteArray([MarshalAs(UnmanagedType.BStr)] [In] String BinaryString);

        [DispId(7)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String ByteArrayToBinaryString([MarshalAs(UnmanagedType.Struct)] [In] Object varByteArray);

        [DispId(8)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        DateTime LocalTimeToUTCTime([In] DateTime LocalTime);

        [DispId(9)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        DateTime UTCTimeToLocalTime([In] DateTime UTCTime);
        }
    }
