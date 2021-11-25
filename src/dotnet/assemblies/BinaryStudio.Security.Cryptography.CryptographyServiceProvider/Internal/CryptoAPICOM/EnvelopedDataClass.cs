using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Content"), ClassInterface(ClassInterfaceType.None), Guid("F3A12E08-EDE9-4160-8B51-334D982A9AD0"), TypeLibType(2)]
    [ComImport]
    public class EnvelopedDataClass : IEnvelopedData
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
        public virtual extern IRecipients Recipients
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String Encrypt([In] CAPICOM_ENCODING_TYPE EncodingType = CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Decrypt([MarshalAs(UnmanagedType.BStr)] [In] String EnvelopedMessage);
        }
    }
