using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Diagnostics;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.Security.Cryptography.Certificates;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Design", "CA1060:Move pinvokes to native methods class", Justification = "<Pending>")]
    #endif
    public class CryptHashAlgorithm : HashAlgorithm, IHashAlgorithm, IHashOperation
    {
        private const Int32 BLOCK_SIZE_64K = 64*1024;
        public CryptHashAlgorithm(SCryptographicContext context, ALG_ID algorithm) {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            HashSizeValue = -1;
            this.context = context;
            this.algorithm = algorithm;
            }

        /// <summary>Releases the unmanaged resources used by the <see cref="HashAlgorithm" /> and optionally releases the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
        protected override void Dispose(Boolean disposing) {
            using (new TraceScope())
                {
                if (handle != IntPtr.Zero) {
                    CryptDestroyHash(handle);
                    handle = IntPtr.Zero;
                    }
                context = null;
                }
            base.Dispose(disposing);
            }

        #region M:Initialize
        /// <summary>Initializes an implementation of the <see cref="HashAlgorithm"/> class.</summary>
        public override void Initialize()
            {
            }
        #endregion

        protected void EnsureCore() {
            using (new TraceScope()) {
                if (handle == IntPtr.Zero) {
                    Validate(CryptCreateHash(context.Handle, algorithm, IntPtr.Zero, 0, out handle));
                    }
                }
            }

        #region M:HashCore(Byte[],Int32,Int32)
        /// <summary>When overridden in a derived class, routes data written to the object into the hash algorithm for computing the hash.</summary>
        /// <param name="array">The input to compute the hash code for.</param>
        /// <param name="startindex">The offset into the byte array from which to begin using data.</param>
        /// <param name="size">The number of bytes in the byte array to use as data.</param>
        protected override unsafe void HashCore(Byte[] array, Int32 startindex, Int32 size) {
            using (new TraceScope(size)) {
                if (array == null) { throw new ArgumentNullException(nameof(array)); }
                if (startindex < 0 || startindex > array.Length - size) { throw new ArgumentOutOfRangeException(nameof(startindex)); }
                if (size < 0 || size > array.Length) { throw new ArgumentOutOfRangeException(nameof(size)); }
                if (size == 0) { return; }
                EnsureCore();
                #if DEBUG
                Debug.Print($"HashCore:[{Encoding.ASCII.GetString(array, startindex, size)}]");
                #endif
                fixed (Byte* block = array) {
                    Validate(CryptHashData(handle, (IntPtr)(block + startindex), size, 0));
                    }
                }
            }
        #endregion
        #region M:HashCore(Stream)
        public void HashCore(Stream stream) {
            using (new TraceScope()) {
                if (stream == null) { throw new ArgumentNullException(nameof(stream)); }
                var r = new Byte[BLOCK_SIZE_64K];
                for (;;) {
                    var n = stream.Read(r, 0, r.Length);
                    if (n == 0) { break; }
                    Yield();
                    HashCore(r, 0, n);
                    }
                }
            }
        #endregion
        #region M:HashFinal:Byte[]
        /// <summary>When overridden in a derived class, finalizes the hash computation after the last data is processed by the cryptographic stream object.</summary>
        /// <returns>The computed hash code.</returns>
        protected override Byte[] HashFinal() {
            using (new TraceScope()) {
                Int32 c;
                var sz = sizeof(Int32);
                Validate(CryptGetHashParam(handle, HP_HASHSIZE, out c, ref sz, 0));
                var r = new Byte[c];
                sz = c;
                Validate(CryptGetHashParam(handle, HP_HASHVAL, r, ref sz, 0));
                #if DEBUG
                Debug.Print($"HashFinal:[{Convert.ToBase64String(r)}]");
                #endif
                return r;
                }
            }
        #endregion
        #region M:Compute(Stream):Byte[]
        public Byte[] Compute(Stream stream) {
            using (new TraceScope()) {
                HashCore(stream);
                return HashFinal();
                }
            }
        #endregion
        #region M:Compute(Byte[]):Byte[]
        public Byte[] Compute(Byte[] bytes) {
            if (bytes == null) { throw new ArgumentNullException(nameof(bytes)); }
            using (new TraceScope(bytes.Length)) {
                using (var input = new MemoryStream(bytes)) {
                    HashCore(input);
                    return HashFinal();
                    }
                }
            }
        #endregion

        #region M:CopyTo(Stream,Stream,Int64)
        private static void CopyTo(Stream source, Stream target) {
            var buffersize = 16*1024*1024;
            var buffer = new Byte[buffersize];
            for (;;) {
                Yield();
                var sz = source.Read(buffer, 0, buffersize);
                if (sz == 0) { break; }
                target.Write(buffer, 0, sz);
                }
            }
        #endregion
        #region M:VerifySignature([Out]Exception,Byte[],CryptKey):Boolean
        public Boolean VerifySignature(out Exception e, Byte[] signature, CryptKey key) {
            if (signature == null) { throw new ArgumentNullException(nameof(signature)); }
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            if (!Validate(out e, CryptVerifySignature(handle, signature, signature.Length, key.handle, IntPtr.Zero, 0)))
                {
                return Validate(out e, CryptVerifySignature(handle, signature.Reverse().ToArray(), signature.Length, key.handle, IntPtr.Zero, 0));
                }
            return false;
            }
        #endregion
        #region M:VerifySignature(Byte[],CryptKey)
        public void VerifySignature(Byte[] signature, CryptKey key) {
            if (signature == null) { throw new ArgumentNullException(nameof(signature)); }
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            if (CryptVerifySignature(handle, signature, signature.Length, key.handle, IntPtr.Zero, 0))
                {
                Validate(CryptVerifySignature(handle, signature.Reverse().ToArray(), signature.Length, key.handle, IntPtr.Zero, 0));
                }
            }
        #endregion
        #region M:VerifySignature(Byte[],CryptKey)
        internal HRESULT VerifySignatureInternal(Byte[] signature, ICryptKey key) {
            if (signature == null) { throw new ArgumentNullException(nameof(signature)); }
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            if (!CryptVerifySignature(handle, signature, signature.Length, key.Handle, IntPtr.Zero, 0)) {
                #if FEATURE_ROTATE_SIGNATURE
                if (!EntryPoint.CryptVerifySignature(handle, signature.Reverse().ToArray(), signature.Length, key.Handle, IntPtr.Zero, 0)) {
                    return (HRESULT)(Marshal.GetHRForLastWin32Error());
                    }
                #else
                return (HRESULT)(Marshal.GetHRForLastWin32Error());
                #endif
                }
            return (HRESULT)(0);
            }
        #endregion
        #region M:VerifySignature(Byte[],Byte[],CryptKey)
        internal unsafe HRESULT VerifySignatureInternal(Byte[] signature, Byte[] digest, CryptKey key) {
            if (digest == null) { throw new ArgumentNullException(nameof(digest)); }
            EnsureCore();
            Int32 c;
            var sz = sizeof(Int32);
            Validate(CryptGetHashParam(handle, HP_HASHSIZE, out c, ref sz, 0));
            if (c != digest.Length) { throw new ArgumentOutOfRangeException(nameof(digest)); }
            Validate(CryptSetHashParam(handle, HP_HASHVAL, digest, 0));
            return VerifySignatureInternal(signature, key);
            }
        #endregion
        #region M:VerifySignature([Out]Exception,Stream,CryptKey)
        public Boolean VerifySignature(out Exception e, Stream signature, CryptKey key) {
            if (signature == null) { throw new ArgumentNullException(nameof(signature)); }
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            using (var memory = new MemoryStream()) {
                CopyTo(signature, memory);
                return VerifySignature(out e, memory.ToArray(), key);
                }
            }
        #endregion
        #region M:VerifySignature(Byte[],Byte[],CryptKey)
        public Boolean VerifySignature(out Exception e, Byte[] signature, Byte[] digest, ICryptKey key) {
            if (digest == null) { throw new ArgumentNullException(nameof(digest)); }
            EnsureCore();
            var sz = sizeof(Int32);
            Validate(CryptGetHashParam(handle, HP_HASHSIZE, out var c, ref sz, 0));
            if (c != digest.Length) { throw new ArgumentOutOfRangeException(nameof(digest)); }
            Validate(CryptSetHashParam(handle, HP_HASHVAL, digest, 0));
            var r = VerifySignatureInternal(signature, key);
            e = (r != HRESULT.S_OK)
                ? GetExceptionForHR(r)
                : null;
            return (e == null);
            }
        #endregion

        #region M:CreateSignature(Stream,UInt32,CRYPT_FLAGS)
        private void CreateSignature(Stream signature, UInt32 keyspec, CRYPT_FLAGS flags) {
            if (signature == null) { throw new ArgumentNullException(nameof(signature)); }
            var c = 0;
            L:
            SetLastError(0);
            if (CryptSignHash(handle, keyspec, IntPtr.Zero, flags, null, ref c)) {
                var r = new Byte[c];
                Validate(CryptSignHash(handle, keyspec, IntPtr.Zero, flags, r, ref c));
                signature.Write(r, 0, c);
                return;
                }
            var hr = GetHRForLastWin32Error();
            if (hr == HRESULT.SCARD_W_WRONG_CHV) {
                if (TryRequestPIN())
                    {
                    goto L;
                    }
                }
            throw new COMException(hr.ToString(), (Int32)hr);
            }
        #endregion
        #region M:CreateSignature(Stream,KeySpec,CRYPT_FLAGS)
        internal void CreateSignature(Stream signature, KeySpec keyspec, CRYPT_FLAGS flags)
            {
            if (signature == null) { throw new ArgumentNullException(nameof(signature)); }
                 if (keyspec == KeySpec.Signature) { CreateSignature(signature, AT_SIGNATURE,   flags); }
            else if (keyspec == KeySpec.Exchange)  { CreateSignature(signature, AT_KEYEXCHANGE, flags); }
            else if (keyspec == (KeySpec.Exchange|KeySpec.Signature))
                {
                try
                    {
                    CreateSignature(signature, AT_SIGNATURE, flags);
                    }
                catch (COMException e)
                    {
                    if ((HRESULT)e.ErrorCode == HRESULT.NTE_BAD_KEYSET)
                        {
                        CreateSignature(signature, AT_KEYEXCHANGE, flags);
                        }
                    else
                        {
                        throw;
                        }
                    }
                }
            else
                {
                throw new ArgumentOutOfRangeException(nameof(keyspec));
                }
            }
        #endregion
        #region M:CreateSignature(Stream,KeySpec)
        public void CreateSignature(Stream signature, KeySpec keyspec)
            {
            if (signature == null) { throw new ArgumentNullException(nameof(signature)); }
            CreateSignature(signature, keyspec, CRYPT_FLAGS.CRYPT_NONE);
            }
        #endregion
        #region M:CreateSignature(KeySpec):Byte[]
        public Byte[] CreateSignature(KeySpec keyspec) {
            using (var outputstream = new MemoryStream()) {
                CreateSignature(outputstream, keyspec, CRYPT_FLAGS.CRYPT_NONE);
                return outputstream.ToArray();
                }
            }
        #endregion
        
        public static void CreateSignature(IX509Certificate certificate, Byte[] digest, Stream signature, KeySpec keyspec) {
            if (certificate == null) { throw new ArgumentNullException(nameof(certificate)); }
            if (signature == null) { throw new ArgumentNullException(nameof(signature)); }
            if (digest == null) { throw new ArgumentNullException(nameof(digest)); }
            using (var context = new SCryptographicContext(certificate.SignatureAlgorithm, CryptographicContextFlags.CRYPT_SILENT|CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                using (var signing = new SCryptographicContext(context, certificate, CRYPT_ACQUIRE_FLAGS.CRYPT_ACQUIRE_NONE)) {
                    using (var engine = new CryptHashAlgorithm(signing, CryptographicObject.OidToAlgId(certificate.HashAlgorithm))) {
                        engine.EnsureCore();
                        var sz = sizeof(Int32);
                        engine.Validate(CryptGetHashParam(engine.handle, HP_HASHSIZE, out var c, ref sz, 0));
                        if (c != digest.Length) { throw new ArgumentOutOfRangeException(nameof(digest)); }
                        engine.Validate(CryptSetHashParam(engine.handle, HP_HASHVAL, digest, 0));
                        engine.CreateSignature(signature, (KeySpec)certificate.KeySpec);
                        }
                    }
                }
            }

        #region M:ToString:String
        public override String ToString()
            {
            return algorithm.ToString();
            }
        #endregion
        #region M:Validate(Boolean)
        protected virtual void Validate(Boolean status) {
            if (!status) {
                Exception e;
                var i = Marshal.GetLastWin32Error();
                if ((i >= 0xFFFF) || (i < 0))
                    {
                    e = new COMException($"{FormatMessage(i)} [HRESULT:{(HRESULT)i}]", i);
                    #if DEBUG
                    Debug.Print($"COMException:{e.Message}");
                    #endif
                    }
                else
                    {
                    e = new COMException($"{FormatMessage(i)} [{(Win32ErrorCode)i}]", i);
                    #if DEBUG
                    Debug.Print($"COMException:{e.Message}");
                    #endif
                    }
                e.Data["Algorithm"] = algorithm.ToString();
                throw e;
                }
            }
        #endregion
        #region M:Validate([Out]Exception,Boolean):Boolean
        protected virtual Boolean Validate(out Exception e, Boolean status) {
            e = null;
            if (!status) {
                var i = Marshal.GetLastWin32Error();
                if ((i >= 0xFFFF) || (i < 0))
                    {
                    e = new COMException($"{FormatMessage(i)} [HRESULT:{(HRESULT)i}]", i);
                    e.Data["Algorithm"] = algorithm.ToString();
                    #if DEBUG
                    Debug.Print($"COMException:{e.Message}");
                    #endif
                    }
                else
                    {
                    e = new COMException($"{FormatMessage(i)} [{(Win32ErrorCode)i}]", i);
                    e.Data["Algorithm"] = algorithm.ToString();
                    #if DEBUG
                    Debug.Print($"COMException:{e.Message}");
                    #endif
                    }
                return false;
                }
            return true;
            }
        #endregion
        #region M:FormatMessage(Int32):String
        private   const UInt32 FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        private   const UInt32 FORMAT_MESSAGE_FROM_SYSTEM     = 0x00001000;
        private   const UInt32 LANG_NEUTRAL                   = 0x00;
        private   const UInt32 SUBLANG_DEFAULT                = 0x01;
        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, SetLastError = true)] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, Int32 nSize, IntPtr[] arguments);
        protected internal static unsafe String FormatMessage(Int32 source) {
            void* r;
            if (FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER|FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, source,
                ((((UInt32)(SUBLANG_DEFAULT)) << 10) | (UInt32)(LANG_NEUTRAL)),
                &r, 0,
                #if NET40
                new IntPtr[0]
                #else
                Array.Empty<IntPtr>()
                #endif
                )) {
                try
                    {
                    #if CAPILITE
                    var o = Marshal.PtrToStringAnsi((IntPtr)r);
                    return (o != null)
                        ? o.Trim().TrimEnd('\n')
                        : null;
                    #else
                    return (new String((Char*)r)).Trim();
                    #endif
                    }
                finally
                    {
                    LocalFree((IntPtr)r);
                    }
                }
            return null;
            }
        #endregion
        #region M:GetHRForLastWin32Error:HRESULT
        protected static HRESULT GetHRForLastWin32Error()
            {
            return (HRESULT)(Marshal.GetHRForLastWin32Error());
            }
        #endregion
        #region M:GetExceptionForHR(HRESULT):Exception
        protected virtual Exception GetExceptionForHR(HRESULT hr)
            {
            var e = new COMException($"{FormatMessage((Int32)hr)} [HRESULT:{hr}]", (Int32)hr);
            #if DEBUG
            Debug.Print($"COMException:{e.Message}");
            #endif
            return e;
            }
        #endregion

        protected static Boolean Yield()
            {
            #if NET35
            return SwitchToThread();
            #else
            return Thread.Yield();
            #endif
            }

        private Boolean pinrequested;
        protected internal Boolean TryRequestPIN()
            {
            if (pinrequested) { return false; }
            //if (context.TryRequestPIN())
            //    {
            //    pinrequested = true;
            //    return true;
            //    }
            return false;
            }

        #if CAPILITE
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptCreateHash(IntPtr provider, ALG_ID algorithm, IntPtr key, UInt32 flags, out CryptHashAlgorithmHandle handle);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptHashData(CryptHashAlgorithmHandle handle, Byte* data, Int32 datasize, UInt32 flags);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptGetHashParam(CryptHashAlgorithmHandle handle, Int32 parameter, void* block, ref Int32 blocksize, UInt32 flags);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptSetHashParam(CryptHashAlgorithmHandle handle, Int32 parameter, [MarshalAs(UnmanagedType.LPArray)] Byte[] block, UInt32 flags);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptGetHashParam(CryptHashAlgorithmHandle handle, Int32 parameter, [MarshalAs(UnmanagedType.LPArray)] Byte[] block, ref Int32 blocksize, UInt32 flags);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptVerifySignature(CryptHashAlgorithmHandle handle, [MarshalAs(UnmanagedType.LPArray)] Byte[] signature, Int32 signaturesize, IntPtr key, IntPtr desc, UInt32 flags);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe Boolean CryptSignHash(CryptHashAlgorithmHandle handle, UInt32 keyspec, IntPtr description, CRYPT_FLAGS flags, [MarshalAs(UnmanagedType.LPArray)] Byte[] signature, ref Int32 length);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern unsafe void SetLastError(Int32 errorcode);
        #else
        #endif
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CryptDestroyHash(IntPtr handle);
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CryptCreateHash(IntPtr provider, ALG_ID algorithm, IntPtr key, UInt32 flags, out IntPtr handle);
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CryptHashData(IntPtr handle, [MarshalAs(UnmanagedType.LPArray)]Byte[] data, Int32 datasize, UInt32 flags);
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CryptHashData(IntPtr handle, IntPtr data, Int32 datasize, UInt32 flags);
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CryptSetHashParam(IntPtr handle, Int32 parameter, [MarshalAs(UnmanagedType.LPArray)] Byte[] block, UInt32 flags);
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CryptGetHashParam(IntPtr handle, Int32 parameter, IntPtr block, ref Int32 blocksize, UInt32 flags);
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CryptGetHashParam(IntPtr handle, Int32 parameter, [MarshalAs(UnmanagedType.LPArray)] Byte[] block, ref Int32 blocksize, UInt32 flags);
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CryptGetHashParam(IntPtr handle, Int32 parameter, out Int32 block, ref Int32 blocksize, UInt32 flags);
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)] private static extern Boolean CryptVerifySignature(IntPtr handle, [MarshalAs(UnmanagedType.LPArray)] Byte[] signature, Int32 signaturesize, IntPtr key, IntPtr desc, UInt32 flags);
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)] private static extern Boolean CryptSignHash(IntPtr handle, UInt32 keyspec, IntPtr description, CRYPT_FLAGS flags, [MarshalAs(UnmanagedType.LPArray)] Byte[] signature, ref Int32 length);
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)] private static extern void SetLastError(Int32 errorcode);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr LocalFree(IntPtr handle);

        private IntPtr handle;
        private SCryptographicContext context;
        private readonly ALG_ID algorithm;
        private const Int32 HP_ALGID                = 0x0001;  // Hash algorithm
        private const Int32 HP_HASHVAL              = 0x0002;  // Hash value
        private const Int32 HP_HASHSIZE             = 0x0004;  // Hash value size
        private const Int32 HP_HMAC_INFO            = 0x0005;  // information for creating an HMAC
        private const Int32 HP_TLS1PRF_LABEL        = 0x0006;  // label for TLS1 PRF
        private const Int32 HP_TLS1PRF_SEED         = 0x0007;  // seed for TLS1 PRF
        private const UInt32 AT_KEYEXCHANGE = 1;
        private const UInt32 AT_SIGNATURE   = 2;

        //private static readonly String TRACE_CRYPTHASHDATA     = $"CSP::{nameof(EntryPoint.CryptHashData)}(HCRYPTHASH,LPCBYTE,DWORD,DWORD):BOOL";
        //private static readonly String TRACE_CRYPTCREATEHASH   = $"CSP::{nameof(EntryPoint.CryptCreateHash)}(HCRYPTPROV,ALG_ID,HCRYPTKEY,DWORD,HCRYPTHASH*):BOOL";
        //private static readonly String TRACE_CRYPTGETHASHPARAM = $"CSP::{nameof(EntryPoint.CryptGetHashParam)}(HCRYPTHASH,DWORD,BYTE*,DWORD*,DWORD):BOOL";

        public virtual IntPtr Handle { get {
            EnsureCore();
            return handle;
            }}

        protected virtual ILogger Logger { get; }

        #region M:ICryptoTransform.TransformBlock(Byte[],Int32,Int32,Byte[],Int32):Int32
        /// <summary>Computes the hash value for the specified region of the input byte array and copies the specified region of the input byte array to the specified region of the output byte array.</summary>
        /// <param name="inputBuffer">The input to compute the hash code for.</param>
        /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
        /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
        /// <param name="outputBuffer">A copy of the part of the input array used to compute the hash code.</param>
        /// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
        /// <returns>The number of bytes written.</returns>
        /// <exception cref="ArgumentException"><paramref name="inputCount" /> uses an invalid value.
        /// -or-  
        /// <paramref name="inputBuffer"/> has an invalid length.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="inputBuffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="inputOffset" /> is out of range. This parameter requires a non-negative number.</exception>
        /// <exception cref="ObjectDisposedException">The object has already been disposed.</exception>
        Int32 ICryptoTransform.TransformBlock(Byte[] inputBuffer, Int32 inputOffset, Int32 inputCount, Byte[] outputBuffer, Int32 outputOffset)
            {
            return TransformBlock(inputBuffer,inputOffset,inputCount,outputBuffer,outputOffset);
            }
        #endregion
        #region M:ICryptoTransform.TransformFinalBlock(Byte[],Int32,Int32):Byte[]
        /// <summary>Computes the hash value for the specified region of the specified byte array.</summary>
        /// <param name="inputBuffer">The input to compute the hash code for.</param>
        /// <param name="inputOffset">The offset into the byte array from which to begin using data.</param>
        /// <param name="inputCount">The number of bytes in the byte array to use as data.</param>
        /// <returns>An array that is a copy of the part of the input that is hashed.</returns>
        /// <exception cref="ArgumentException"><paramref name="inputCount" /> uses an invalid value.
        /// -or-  
        /// <paramref name="inputBuffer"/> has an invalid offset length.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="inputBuffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="inputOffset"/> is out of range. This parameter requires a non-negative number.</exception>
        /// <exception cref="ObjectDisposedException">The object has already been disposed.</exception>
        Byte[] ICryptoTransform.TransformFinalBlock(Byte[] inputBuffer, Int32 inputOffset, Int32 inputCount)
            {
            return TransformFinalBlock(inputBuffer, inputOffset, inputCount);
            }
        #endregion

        Int32 ICryptoTransform.InputBlockSize { get; }
        Int32 ICryptoTransform.OutputBlockSize { get; }
        Boolean ICryptoTransform.CanTransformMultipleBlocks { get; }
        Boolean ICryptoTransform.CanReuseTransform { get; }
        Byte[] IHashAlgorithm.Hash { get; }

        public override Int32 HashSize { get {
            if (HashSizeValue < 0) {
                EnsureCore();
                var sz = sizeof(Int32);
                Validate(CryptGetHashParam(handle, HP_HASHSIZE, out var c, ref sz, 0));
                HashSizeValue = c;
                }
            return HashSizeValue;
            }}

        #region M:IHashAlgorithm.Initialize
        /// <summary>Initializes an implementation of the <see cref="IHashAlgorithm"/> class.</summary>
        void IHashAlgorithm.Initialize()
            {
            Initialize();
            }
        #endregion
        #region M:IHashAlgorithm.ComputeHash(Stream):Byte[]
        /// <summary>Computes the hash value for the specified <see cref="Stream" /> object.</summary>
        /// <param name="inputstream">The input to compute the hash code for.</param>
        /// <returns>The computed hash code.</returns>
        /// <exception cref="ObjectDisposedException">The object has already been disposed.</exception>
        Byte[] IHashAlgorithm.Compute(Stream inputstream)
            {
            return Compute(inputstream);
            }
        #endregion
        #region M:IHashOperation.HashCore(Byte[],Int32,Int32)
        void IHashOperation.HashCore(Byte[] array, Int32 startindex, Int32 size)
            {
            HashCore(array, startindex, size);
            }
        #endregion

        static CryptHashAlgorithm()
            {
            }
        }
    }