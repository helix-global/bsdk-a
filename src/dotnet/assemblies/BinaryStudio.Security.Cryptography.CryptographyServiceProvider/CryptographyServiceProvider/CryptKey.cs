using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using BinaryStudio.Diagnostics;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.Certificates;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Design", "CA1060:Move pinvokes to native methods class", Justification = "<Pending>")]
    #endif
    public class CryptKey : CryptographicObject, ICryptKey
        {
        internal CryptKey(SCryptographicContext context, IntPtr handle) {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (handle == null) { throw new ArgumentNullException(nameof(handle)); }
            this.handle = handle;
            Context = context;
            }

        internal CryptKey(SCryptographicContext context, IntPtr handle, String container, X509KeySpec keyspec)
            :this(context, handle)
            {
            Container = container;
            KeySpec = keyspec;
            }

        public CryptKey(IntPtr handle)
            {
            if (handle == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(handle)); }
            this.handle = handle;
            Context = null;
            }

        /// <summary>
        /// Releases the unmanaged resources used by the instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected override void Dispose(Boolean disposing) {
            using (new TraceScope())
                {
                if (handle != IntPtr.Zero) {
                    CryptDestroyKey(handle);
                    handle = IntPtr.Zero;
                    }
                base.Dispose(disposing);
                }
            }

        //#region M:ExportKey(CryptKey,CryptKeyBLOBType,CryptKeyFlags):Byte[]
        //internal CryptKeyBLOB ExportKey(CryptKey key, CryptKeyBLOBType blobtype, CryptKeyFlags flags) {
        //    if ((flags & ~(CryptKeyFlags.CRYPT_BLOB_VER3  | CryptKeyFlags.CRYPT_DESTROYKEY | CryptKeyFlags.CRYPT_OAEP | CryptKeyFlags.CRYPT_SSL2_FALLBACK | CryptKeyFlags.CRYPT_Y_ONLY)) != 0) { throw new ArgumentOutOfRangeException(nameof(flags)); }
        //    var expkey = (key != null) ? key.handle : IntPtr.Zero;
        //    var c = 0;
        //    Validate(CryptExportKey(handle, expkey, blobtype, flags, null, ref c));
        //    var r = new Byte[c];
        //    Validate(CryptExportKey(handle, expkey, blobtype, flags, r, ref c));
        //    return context.DecodeKeyBLOB(r, blobtype);
        //    }
        //#endregion
        #region M:ToString:String
        public override String ToString()
            {
            return handle.ToString();
            }
        #endregion

        public override IntPtr Handle { get { return handle; }}
        public SCryptographicContext Context {  get; }
        protected internal override ILogger Logger { get; }
        public String Container { get; }
        public X509KeySpec KeySpec { get; }

        internal IntPtr handle;

        #if CAPILITE
        [DllImport("capi20")] private static extern Boolean CryptExportKey(IntPtr key, IntPtr expkey, CryptKeyBLOBType blobtype, CryptKeyFlags flags, [MarshalAs(UnmanagedType.LPArray)] Byte[] blob, ref Int32 c);
        [DllImport("capi20")] private static extern Boolean CryptGetKeyParam(IntPtr key, KEY_PARAM param, [MarshalAs(UnmanagedType.LPArray)]Byte[] data, ref Int32 datasize, UInt32 flags);
        #else
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)] private static extern Boolean CryptDestroyKey(IntPtr handle);
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CryptSetKeyParam(IntPtr key, KEY_PARAM param, [MarshalAs(UnmanagedType.LPArray)]Byte[] data, UInt32 flags);
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CryptGetKeyParam(IntPtr key, KEY_PARAM param, [MarshalAs(UnmanagedType.LPArray)]Byte[] data, ref Int32 datasize, UInt32 flags);
        #endif

        internal void SetParameter(KEY_PARAM key, UInt32 flags, Byte[] value) {
            Validate(CryptSetKeyParam(handle, key, value, flags));
            }

        #region M:GetParameter(KEY_PARAM,UInt32):Byte[]
        internal Byte[] GetParameter(KEY_PARAM key, UInt32 flags) {
            for (var i = 0x200;;) {
                var r = new Byte[i];
                if (CryptGetKeyParam(handle, key, r, ref i, flags)) { return r; }
                var e = GetLastWin32Error();
                if ((Win32ErrorCode)e == Win32ErrorCode.ERROR_MORE_DATA) { continue; }
                if ((HRESULT)e == HRESULT.NTE_BAD_KEY) {
                    /*
                     * При вызове метода CryptGetKeyParam с буфером недостаточной длины GetLastError() возвращает ошибку NTE_BAD_KEY (0x80090003 Плохой ключ.),
                     * хотя ожидается ошибка ERROR_MORE_DATA. Стабильно воспроизводится при запросе сертификата (параметра KP_CERTIFICATE).
                     */
                    if (key == KEY_PARAM.KP_CERTIFICATE) {
                        if (Context.Name.StartsWith("Crypto-Pro", StringComparison.OrdinalIgnoreCase) && (Context.Version.Major == 5)) {
                            continue;
                            }
                        }
                    }
                #if DEBUG
                if ((e >= 0xFFFF) || (e < 0))
                    {
                    #if DEBUG
                    Debug.Print($"GetParameter:{$"{FormatMessage(e)} [HRESULT:{(HRESULT)e}];Container:[{Container}];KeySpec:[{KeySpec}]"}");
                    #endif
                    }
                else
                    {
                    #if DEBUG
                    Debug.Print($"GetParameter:{$"{FormatMessage(e)} [{(Win32ErrorCode)e}] Container:[{Container}];KeySpec:[{KeySpec}]"}");
                    #endif
                    }
                #endif
                break;
                }
            return null;
            }
        #endregion
        #region M:GetParameter(KEY_PARAM):Byte[]
        internal Byte[] GetParameter(KEY_PARAM key) {
            return GetParameter(key, 0);
            }
        #endregion
        #region M:GetParameter<T>(KEY_PARAM,UInt32):T
        internal unsafe T GetParameter<T>(KEY_PARAM key, UInt32 flags) {
            var r = GetParameter(key, flags);
            fixed (Byte* i = r)
                {
                if (typeof(T) == typeof(String))
                    {
                    return (T)(Object)Marshal.PtrToStringUni((IntPtr)i);
                    }
                if (typeof(T) == typeof(Int32))  { return (T)(Object)(*(Int32*)i);  }
                if (typeof(T) == typeof(UInt32)) { return (T)(Object)(*(UInt32*)i); }
                }
            return default(T);
            }
        #endregion
        #region M:GetParameter<T>(KEY_PARAM):T
        internal T GetParameter<T>(KEY_PARAM key) {
            return GetParameter<T>(key, 0);
            }
        #endregion

        private ALG_ID? algid;
        public ALG_ID AlgId { get {
            if (algid == null) {
                algid = (ALG_ID)GetParameter<UInt32>(KEY_PARAM.KP_ALGID);
                }
            return algid.Value;
            }}

        private Int32? keylen;
        public Int32 KeyLength { get {
            if (keylen == null) {
                keylen = GetParameter<Int32>(KEY_PARAM.KP_KEYLEN);
                }
            return keylen.Value;
            }}

        private KP_PERMISSIONS? keyperm;
        internal KP_PERMISSIONS KeyPermissions { get {
            if (keyperm == null) {
                keyperm = (KP_PERMISSIONS)(GetParameter<UInt32>(KEY_PARAM.KP_PERMISSIONS) & 0x01FF);
                }
            return keyperm.Value;
            }}

        IX509Certificate ICryptKey.Certificate { get{
            var r = GetParameter(KEY_PARAM.KP_CERTIFICATE);
            if (r != null)
                {
                return new X509Certificate(r,
                    Container, KeySpec, Context.Type, Context.FullQualifiedContainerName,
                    Context.SecurityDescriptor, Context.Name);
                }
            return null;
            }}
        }
    }