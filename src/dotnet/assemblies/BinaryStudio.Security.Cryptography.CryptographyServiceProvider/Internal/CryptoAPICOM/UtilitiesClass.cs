using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [ClassInterface(ClassInterfaceType.None), Guid("22A85CE1-F011-4231-B9E4-7E7A0438F71B"), TypeLibType(2)]
    [ComImport]
    public class UtilitiesClass : IUtilities
        {
        [DispId(1)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String GetRandom([In] Int32 Length = 8, [In] CAPICOM_ENCODING_TYPE EncodingType = CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BINARY);

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String Base64Encode([MarshalAs(UnmanagedType.BStr)] [In] String SrcString);

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String Base64Decode([MarshalAs(UnmanagedType.BStr)] [In] String EncodedString);

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String BinaryToHex([MarshalAs(UnmanagedType.BStr)] [In] String BinaryString);

        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String HexToBinary([MarshalAs(UnmanagedType.BStr)] [In] String HexString);

        [DispId(6)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Struct)]
        public virtual extern Object BinaryStringToByteArray([MarshalAs(UnmanagedType.BStr)] [In] String BinaryString);

        [DispId(7)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String ByteArrayToBinaryString([MarshalAs(UnmanagedType.Struct)] [In] Object varByteArray);

        [DispId(8)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern DateTime LocalTimeToUTCTime([In] DateTime LocalTime);

        [DispId(9)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern DateTime UTCTimeToLocalTime([In] DateTime UTCTime);
        }
    }
