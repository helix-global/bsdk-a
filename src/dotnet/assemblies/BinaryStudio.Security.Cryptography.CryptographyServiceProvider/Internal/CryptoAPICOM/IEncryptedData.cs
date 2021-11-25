using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Content"), Guid("C4778A66-972F-42E4-87C5-5CC16F7931CA")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface IEncryptedData
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
        [MethodImpl(MethodImplOptions.InternalCall)]
        void SetSecret([MarshalAs(UnmanagedType.BStr)] [In] String newVal, [In] CAPICOM_SECRET_TYPE SecretType = CAPICOM_SECRET_TYPE.CAPICOM_SECRET_PASSWORD);

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String Encrypt([In] CAPICOM_ENCODING_TYPE EncodingType = CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Decrypt([MarshalAs(UnmanagedType.BStr)] [In] String EncryptedMessage);
        }
    }
