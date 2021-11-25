using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Content"), ClassInterface(ClassInterfaceType.None), Guid("94AFFFCC-6C05-4814-B123-A941105AA77F"), TypeLibType(2)]
    [ComImport]
    public class SignedDataClass : ISignedData
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
        public virtual extern ISigners Signers
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(2)]
        public virtual extern ICertificates Certificates
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String Sign([MarshalAs(UnmanagedType.Interface)] [In] ISigner pSigner = null, [In] Boolean bDetached = false, [In] CAPICOM_ENCODING_TYPE EncodingType = CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String CoSign([MarshalAs(UnmanagedType.Interface)] [In] ISigner pSigner = null, [In] CAPICOM_ENCODING_TYPE EncodingType = CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);

        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Verify([MarshalAs(UnmanagedType.BStr)] [In] String SignedMessage, [In] Boolean bDetached = false, [In] CAPICOM_SIGNED_DATA_VERIFY_FLAG VerifyFlag = CAPICOM_SIGNED_DATA_VERIFY_FLAG.CAPICOM_VERIFY_SIGNATURE_AND_CERTIFICATE);
        }
    }
