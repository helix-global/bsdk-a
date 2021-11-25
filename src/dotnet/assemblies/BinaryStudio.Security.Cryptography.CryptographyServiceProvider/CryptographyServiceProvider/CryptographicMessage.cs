using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Diagnostics;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider.Internal;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    [SuppressMessage("Design", "CA1060:Move pinvokes to native methods class")]
    #endif
    public class CryptographicMessage : CryptographicObject, ICryptographicMessage
        {
        X509ObjectType IX509Object.ObjectType { get { return X509ObjectType.Message; }}
        public override IntPtr Handle { get { return source; }}
        protected internal override ILogger Logger { get; }

        IntPtr IX509Object.Handle
            {
            get { return Handle; }
            }

        private IntPtr source;
        private CMSG_STREAM_INFO so;
        private IList<CryptographicContext> contextes = new List<CryptographicContext>();

        #if CAPILITE
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe CryptographicMessageHandle CryptMsgOpenToEncode([In] CRYPT_MSG_TYPE dwMsgEncodingType, [In] CMSG_FLAGS flags, [In] CMSG_TYPE dwMsgType, [In] IntPtr pvMsgEncodeInfo, [In] IntPtr pszInnerContentObjID, [In] IntPtr pStreamInfo);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe CryptographicMessageHandle CryptMsgOpenToEncode([In] CRYPT_MSG_TYPE dwMsgEncodingType, [In] CMSG_FLAGS flags, [In] MessageType dwMsgType, [In] IntPtr pvMsgEncodeInfo, [MarshalAs(UnmanagedType.LPStr)] [In] String pszInnerContentObjID, [In] IntPtr pStreamInfo);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptMsgUpdate([In] CryptographicMessageHandle hcryptmsg, [In] Byte* pbdata, [In] UInt32 cbdata, [In] Boolean final);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptMsgGetParam(CryptographicMessageHandle msg, CMSG_PARAM parameter, Int32 signerindex, [MarshalAs(UnmanagedType.LPArray)] Byte[] data, ref Int32 size);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptMsgControl(CryptographicMessageHandle msg, CRYPT_MESSAGE_FLAGS flags, CMSG_CTRL ctrltype, IntPtr ctrlpara);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptMsgControl(CryptographicMessageHandle msg, CRYPT_MESSAGE_FLAGS flags, CMSG_CTRL ctrltype, ref CMSG_CTRL_DECRYPT_PARA ctrlpara);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptMsgControl(CryptographicMessageHandle msg, CRYPT_MESSAGE_FLAGS flags, CMSG_CTRL ctrltype, void* ctrlpara);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe CryptographicMessageHandle CryptMsgOpenToEncode([In] CRYPT_MSG_TYPE dwmsgencodingtype, [In] CMSG_FLAGS flags, [In] CMSG_TYPE dwmsgtype, [In] ref CMSG_SIGNED_ENCODE_INFO pvmsgencodeinfo, [In] IntPtr pszinnercontentobjid, [In] ref CMSG_STREAM_INFO pstreaminfo);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe CryptographicMessageHandle CryptMsgOpenToEncode([In] CRYPT_MSG_TYPE dwmsgencodingtype, [In] CMSG_FLAGS flags, [In] CMSG_TYPE dwmsgtype, [In] ref CMSG_SIGNED_ENCODE_INFO pvmsgencodeinfo, [In] IntPtr pszinnercontentobjid, [In] IntPtr pstreaminfo);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe CryptographicMessageHandle CryptMsgOpenToDecode(CRYPT_MSG_TYPE dwMsgEncodingType, CRYPT_OPEN_MESSAGE_FLAGS flags, CMSG_TYPE type, IntPtr hCryptProv, IntPtr pRecipientInfo, ref CMSG_STREAM_INFO si);
        #else
        #endif

        internal CryptographicMessage(IntPtr source)
            {
            if (source == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(source)); }
            this.source = source;
            }

        private CryptographicMessage(IntPtr source, ref CMSG_STREAM_INFO so)
            {
            if (source == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(source)); }
            this.source = source;
            this.so = so;
            }

        #region M:OpenToDecode(Action<Byte[],Boolean>):CryptographicMessage
        public static unsafe CryptographicMessage OpenToDecode(Action<Byte[], Boolean> outputhandler)
            {
            var so = new CMSG_STREAM_INFO(CMSG_INDEFINITE_LENGTH,delegate(IntPtr arg, Byte* data, UInt32 size, Boolean final) {
                var bytes = new Byte[size];
                for (var i = 0; i < size; i++) {
                    bytes[i] = data[i];
                    }
                outputhandler(bytes, final);
                return true;
                }, IntPtr.Zero);
            return new CryptographicMessage(EntryPoint.CryptMsgOpenToDecode(CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING, 0, CMSG_TYPE.CMSG_NONE, IntPtr.Zero, IntPtr.Zero, ref so),ref so);
            }
        #endregion

        protected override void Dispose(Boolean disposing) {
            using (new TraceScope()) {
                if (disposing) {
                    if (source != IntPtr.Zero)
                        {
                        EntryPoint.CryptMsgClose(source);
                        source = IntPtr.Zero;
                        }

                    if (contextes != null) {
                        for (var i = 0; i < contextes.Count; i++) { contextes[i] = null; }
                        contextes.Clear();
                        contextes = null;
                        }
                    so.StreamOutput = null;
                    }
                base.Dispose(disposing);
                }
            }

        #region M:Control(CRYPT_MESSAGE_FLAGS,CMSG_CTRL,IntPtr)
        internal void Control(CRYPT_MESSAGE_FLAGS flags, CMSG_CTRL ctrltype, IntPtr ctrlpara) {
            using (new TraceScope()) {
                Validate(EntryPoint.CryptMsgControl(source, flags, ctrltype, ctrlpara));
                }
            }
        #endregion
        #region M:Control(CRYPT_MESSAGE_FLAGS,CMSG_CTRL,CMSG_CTRL_DECRYPT_PARA)
        internal void Control(CRYPT_MESSAGE_FLAGS flags, CMSG_CTRL ctrltype, ref CMSG_CTRL_DECRYPT_PARA ctrlpara) {
            using (new TraceScope()) {
                Validate(EntryPoint.CryptMsgControl(source, flags, ctrltype, ref ctrlpara));
                }
            }
        #endregion
        #region M:Update(Byte[],Int32,Boolean)
        public unsafe void Update(Byte[] data, Int32 length, Boolean final) {
            fixed (Byte* bytes = data) {
                Update(bytes, length, final);
                }
            }
        #endregion
        #region M:Update(Byte*,Int32,Boolean)
        public unsafe void Update(Byte* data, Int32 length, Boolean final) {
            using (new TraceScope(length)) {
                Validate(EntryPoint.CryptMsgUpdate(source, (IntPtr)data, length, final));
                }
            }
        #endregion
        #region M:Update(Byte[],Boolean)
        public void Update(Byte[] data, Boolean final) {
            if (data == null) { throw new ArgumentNullException(nameof(data)); }
            Update(data, data.Length, final);
            }
        #endregion
        #region M:GetParameter(CMSG_PARAM,Int32,[Out]HRESULT):Byte[]
        internal Byte[] GetParameter(CMSG_PARAM parameter, Int32 signerindex, out HRESULT hr)
            {
            var c = 0;
            if (!EntryPoint.CryptMsgGetParam(source, parameter, signerindex, null, ref c)) {
                hr = (HRESULT)Marshal.GetLastWin32Error();
                return EmptyArray<Byte>.Value;
                }
            var r = new Byte[c];
            if (!EntryPoint.CryptMsgGetParam(source, parameter, signerindex, r, ref c)) {
                hr = (HRESULT)Marshal.GetLastWin32Error();
                return EmptyArray<Byte>.Value;
                }
            hr = 0;
            return r;
            }
        #endregion
        #region M:GetParameter(CMSG_PARAM,Int32):Byte[]
        internal Byte[] GetParameter(CMSG_PARAM parameter, Int32 signerindex)
            {
            HRESULT hr;
            var c = 0;
            if (!EntryPoint.CryptMsgGetParam(source, parameter, signerindex, null, ref c)) {
                hr = (HRESULT)Marshal.GetLastWin32Error();
                if (hr != HRESULT.CRYPT_E_INVALID_INDEX) { Marshal.ThrowExceptionForHR((Int32)hr); }
                return EmptyArray<Byte>.Value;
                }
            var r = new Byte[c];
            if (!EntryPoint.CryptMsgGetParam(source, parameter, signerindex, r, ref c)) {
                hr = (HRESULT)Marshal.GetLastWin32Error();
                if (hr != HRESULT.CRYPT_E_INVALID_INDEX) { Marshal.ThrowExceptionForHR((Int32)hr); }
                return EmptyArray<Byte>.Value;
                }
            return r;
            }
        #endregion
        #region M:GetParameterSize(CMSG_PARAM,Int32):Int32
        internal Int32 GetParameterSize(CMSG_PARAM parameter, Int32 signerindex)
            {
            var c = 0;
            if (!EntryPoint.CryptMsgGetParam(source, parameter, signerindex, null, ref c)) {
                var hr = (HRESULT)Marshal.GetLastWin32Error();
                if (hr != HRESULT.CRYPT_E_INVALID_INDEX) { Marshal.ThrowExceptionForHR((Int32)hr); }
                return 0;
                }
            return c;
            }
        #endregion
        internal unsafe T GetParameter<T>(CMSG_PARAM parameter, Int32 signerindex) {
            var r = GetParameter(parameter, signerindex);
            fixed (Byte* bytes = r) {
                if (typeof(T) == typeof(Int32))  { return (T)(Object)(*((Int32*)bytes));  }
                if (typeof(T) == typeof(UInt32)) { return (T)(Object)(*((UInt32*)bytes)); }
                if (typeof(T) == typeof(Byte[])) { return (T)(Object)r; }
                if (typeof(T) == typeof(String)) { return (T)(Object)Marshal.PtrToStringAnsi((IntPtr)bytes); }
                if (typeof(T) == typeof(Oid))    { return (T)(Object)(new Oid(Marshal.PtrToStringAnsi((IntPtr)bytes))); }
                throw new NotImplementedException();
                }
            }

        protected const UInt32 CMSG_INDEFINITE_LENGTH = 0xFFFFFFFF;
        }
    }