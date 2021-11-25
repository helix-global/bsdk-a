using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Content"), ClassInterface(ClassInterfaceType.None), Guid("A440BD76-CFE1-4D46-AB1F-15F238437A3D"), TypeLibType(2)]
    [ComImport]
    public class EncryptedDataClass : IEncryptedData
        {
        [DispId(0)]
        public virtual extern String Content
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
        public virtual extern IAlgorithm Algorithm
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void SetSecret([MarshalAs(UnmanagedType.BStr)] [In] String newVal, [In] CAPICOM_SECRET_TYPE SecretType = CAPICOM_SECRET_TYPE.CAPICOM_SECRET_PASSWORD);

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String Encrypt([In] CAPICOM_ENCODING_TYPE EncodingType = CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Decrypt([MarshalAs(UnmanagedType.BStr)] [In] String EncryptedMessage);
        }
    }
