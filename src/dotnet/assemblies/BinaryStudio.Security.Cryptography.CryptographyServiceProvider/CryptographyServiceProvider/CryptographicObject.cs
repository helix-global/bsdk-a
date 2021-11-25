using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.Security.Cryptography.Win32;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    using CRYPT_INTEGER_BLOB = CRYPT_BLOB;
    using CERT_NAME_BLOB     = CRYPT_BLOB;

    #if CODE_ANALYSIS
    [SuppressMessage("Design", "CA1060:Move pinvokes to native methods class")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    #endif
    public abstract class CryptographicObject : IDisposable, IServiceProvider
        {
        #region M:Dispose<T>([Ref]T)
        protected void Dispose<T>(ref T o)
            where T: class, IDisposable
            {
            if (o != null) {
                o.Dispose();
                o = null;
                }
            }
        #endregion
        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing) {
            /*Debug.Print($"{GetType().Name}.Dispose({disposing})");*/
            if (disposing) {
                if (manager != null) {
                    manager.Dispose();
                    manager = null;
                    }
                }
            }
        #endregion
        #region M:Dispose
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Finalize
        ~CryptographicObject()
            {
            Dispose(false);
            }
        #endregion

        /// <summary>Gets the service object of the specified type.</summary>
        /// <param name="service">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="service" />.
        /// -or-
        /// <see langword="null" /> if there is no service object of type <paramref name="service" />.</returns>
        public virtual Object GetService(Type service) {
            if (service == null) { return null; }
            if (service == GetType()) { return this; }
            if (service.IsAssignableFrom(GetType())) { return this; }
            return null;
            }

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
                return false;
                }
            return true;
            }
        #endregion
        #region M:Validate(HRESULT)
        protected virtual void Validate(HRESULT hr) {
            if (hr != HRESULT.S_OK) {
                throw new COMException($"{FormatMessage((Int32)hr)} [HRESULT:{hr}]");
                }
            }
        #endregion
        #region M:GetExceptionForHR(Int32):Exception
        protected virtual Exception GetExceptionForHR(Int32 errorcode)
            {
            Exception e;
            if ((errorcode >= 0xFFFF) || (errorcode < 0))
                {
                e = new COMException($"{FormatMessage(errorcode)} [HRESULT:{(HRESULT)errorcode}]", errorcode);
                #if DEBUG
                Debug.Print($"COMException:{e.Message}");
                #endif
                }
            else
                {
                e = new COMException($"{FormatMessage(errorcode)} [{(Win32ErrorCode)errorcode}]", errorcode);
                #if DEBUG
                Debug.Print($"COMException:{e.Message}");
                #endif
                }
            return e;
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

        protected static UInt32 MAKELCID(UInt16 lgid, UInt16 srtid) { return ((UInt32)((((UInt32)((UInt16)(srtid))) << 16) | ((UInt32)((UInt16)(lgid))))); }
        protected static UInt16 MAKELANGID(UInt16 p, UInt16 s) { return (UInt16)((((UInt16) (s)) << 10) | (UInt16) (p)); } 

        #region M:FormatMessage(Int32):String
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
        #region M:DecodeNameString(ref CERT_NAME_BLOB):String
        internal static String DecodeNameString(ref CERT_NAME_BLOB source) {
            var r = CertNameToStr(X509_ASN_ENCODING, ref source, CERT_X500_NAME_STR, IntPtr.Zero, 0);
            if (r != 0) {
                using (var buffer = new LocalMemory(r << 1)) {
                    if (CertNameToStr(X509_ASN_ENCODING, ref source, CERT_X500_NAME_STR, buffer, r) > 0) {
                        return Marshal.PtrToStringUni(buffer);
                        }
                    }
                }
            return null;
            }
        #endregion
        #region M:DecodeSerialNumberString(ref CRYPT_INTEGER_BLOB):String
        internal static unsafe String DecodeSerialNumberString(ref CRYPT_INTEGER_BLOB source) {
            var c = source.Size;
            var r = new StringBuilder();
            var bytes = source.Data;
            for (var i = 0U; i < c; ++i) {
                r.AppendFormat("{0:X2}", bytes[c - i - 1]);
                }
            return r.ToString();
            }
        #endregion
        #region M:GetHRForLastWin32Error:HRESULT
        protected static HRESULT GetHRForLastWin32Error()
            {
            return (HRESULT)(Marshal.GetHRForLastWin32Error());
            }
        #endregion
        #region M:GetLastWin32Error:Int32
        protected static Int32 GetLastWin32Error()
            {
            #if CAPILITE
            return GetLastError();
            #else
            return Marshal.GetLastWin32Error();
            #endif
            }
        #endregion

        #if CAPILITE
        protected const String CAPI20 = "capi20";
        [DllImport(CAPI20, EntryPoint = "GetLastError")]   private static extern Int32 GetLastError();
        [DllImport(CAPI20, EntryPoint = "FormatMessageA")] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, Int32 nSize, IntPtr[] arguments);
        [DllImport(CAPI20)] internal static extern unsafe IntPtr LocalFree(void* handle);
        [DllImport(CAPI20, CharSet = CharSet.Auto)] private static extern UInt32 CertNameToStr([In] UInt32 dwCertEncodingType, [In] ref CERT_NAME_BLOB pName, [In] UInt32 dwStrType, [In] [Out] IntPtr psz, [In] UInt32 csz);
        [DllImport(CAPI20, CharSet = CharSet.None)] private static extern ALG_ID CertOIDToAlgId([MarshalAs(UnmanagedType.LPStr)] String objid);
        #else
        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, SetLastError = true)] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, Int32 nSize, IntPtr[] arguments);
        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, SetLastError = true)] protected static extern void SetLastError(Int32 errorcode);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr LocalFree(IntPtr handle);
        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern UInt32 CertNameToStr([In] UInt32 dwCertEncodingType, [In] ref CERT_NAME_BLOB pName, [In] UInt32 dwStrType, [In] [Out] IntPtr psz, [In] UInt32 csz);
        [DllImport("crypt32.dll",  BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern unsafe ALG_ID CertOIDToAlgId([MarshalAs(UnmanagedType.LPStr)] String objid);
        #endif

        private   const UInt32 FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        private   const UInt32 FORMAT_MESSAGE_FROM_SYSTEM     = 0x00001000;
        private   const UInt32 LANG_NEUTRAL                   = 0x00;
        private   const UInt32 SUBLANG_DEFAULT                = 0x01;
        protected const UInt32 X509_ASN_ENCODING           = 0x00000001;
        protected const UInt32 PKCS_7_ASN_ENCODING         = 0x00010000;
        private const UInt32 CERT_SIMPLE_NAME_STR        = 1;
        private const UInt32 CERT_OID_NAME_STR           = CERT_SIMPLE_NAME_STR + 1;
        private const UInt32 CERT_X500_NAME_STR          = CERT_OID_NAME_STR    + 1;

        internal LocalMemoryManager manager;
        public abstract IntPtr Handle { get; }
        protected internal abstract ILogger Logger { get; }

        protected virtual void EnsureCore() { }

        private void EnsureMemoryManager() {
            if (manager == null) {
                manager = new LocalMemoryManager();
                }
            }

        protected unsafe void* LocalAlloc(Int32 size)
            {
            EnsureMemoryManager();
            return manager.Alloc(size);
            }

        #region M:StringToSecureString(String):SecureString
        protected static unsafe SecureString StringToSecureString(String source) {
            if (String.IsNullOrEmpty(source)) { return null; }
            fixed (Char* r = source) {
                return new SecureString(r, source.Length);
                }
            }
        #endregion
        #region M:SecureStringToString(SecureString):String
        protected static String SecureStringToString(SecureString source) {
            if (source == null) { return null; }
            var i = Marshal.SecureStringToGlobalAllocAnsi(source);
            try
                {
                return Marshal.PtrToStringAnsi(i);
                }
            finally
                {
                Marshal.ZeroFreeGlobalAllocAnsi(i);
                }
            }
        #endregion

        public unsafe void* StringToMem(String value, Encoding encoding) {
            if (value == null) { return null; }
            var bytes = encoding.GetBytes(value);
            var c = bytes.Length;
            var r = (Byte*)LocalAlloc(c + 1);
            for (var i = 0; i < c; i++)
                {
                r[i] = bytes[i];
                }
            return r;
            }

        protected static Boolean Yield()
            {
            #if NET35
            return SwitchToThread();
            #else
            return Thread.Yield();
            #endif
            }

        protected static IDisposable ReadLock(ReaderWriterLockSlim o)            { return new ReadLockScope(o);            }
        protected static IDisposable WriteLock(ReaderWriterLockSlim o)           { return new WriteLockScope(o);           }
        protected static IDisposable UpgradeableReadLock(ReaderWriterLockSlim o) { return new UpgradeableReadLockScope(o); }

        private class ReadLockScope : IDisposable
            {
            private ReaderWriterLockSlim o;
            public ReadLockScope(ReaderWriterLockSlim o)
                {
                this.o = o;
                o.EnterReadLock();
                }

            public void Dispose()
                {
                o.ExitReadLock();
                o = null;
                }
            }

        private class UpgradeableReadLockScope : IDisposable
            {
            private ReaderWriterLockSlim o;
            public UpgradeableReadLockScope(ReaderWriterLockSlim o)
                {
                this.o = o;
                o.EnterUpgradeableReadLock();
                }

            public void Dispose()
                {
                o.ExitUpgradeableReadLock();
                o = null;
                }
            }

        private class WriteLockScope : IDisposable
            {
            private ReaderWriterLockSlim o;
            public WriteLockScope(ReaderWriterLockSlim o)
                {
                this.o = o;
                o.EnterWriteLock();
                }

            public void Dispose()
                {
                o.ExitWriteLock();
                o = null;
                }
            }

        public static ALG_ID OidToAlgId(Oid value)
            {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            switch (value.Value) {
                case ObjectIdentifiers.szOID_NIST_sha256: { return ALG_ID.CALG_SHA_256; }
                case ObjectIdentifiers.szOID_NIST_sha384: { return ALG_ID.CALG_SHA_384; }
                case ObjectIdentifiers.szOID_NIST_sha512: { return ALG_ID.CALG_SHA_512; }
                case ObjectIdentifiers.szOID_CP_GOST_R3411_12_256: { return ALG_ID.CALG_GR3411_2012_256; }
                case ObjectIdentifiers.szOID_CP_GOST_R3411_12_512: { return ALG_ID.CALG_GR3411_2012_512; }
                }
            return CertOIDToAlgId(value.Value);
            }

        protected static Boolean IsValid(SafeHandle value)
            {
            return !((value == null) || (value.IsInvalid) || (value.IsClosed));
            }

        static CryptographicObject()
            {
            }
        }
    }