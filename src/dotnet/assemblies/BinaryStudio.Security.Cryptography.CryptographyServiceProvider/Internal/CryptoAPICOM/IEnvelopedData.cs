using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Content"), Guid("F6CB6A20-CC18-4424-AE57-6F2AA3DC2059")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface IEnvelopedData
        {
        [DispId(0)]
        String Content
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
            }

        [DispId(1)]
        IAlgorithm Algorithm
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(2)]
        IRecipients Recipients
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String Encrypt([In] CAPICOM_ENCODING_TYPE EncodingType = CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Decrypt([MarshalAs(UnmanagedType.BStr)] [In] String EnvelopedMessage);
        }
    }
