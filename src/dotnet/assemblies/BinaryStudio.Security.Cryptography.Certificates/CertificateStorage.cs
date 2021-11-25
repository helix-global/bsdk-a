using BinaryStudio.PlatformComponents.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    internal abstract class CertificateStorage : IX509CertificateStorage, IDisposable
        {
        //protected static ICryptoAPI EntryPoint { get; }
        static CertificateStorage()
            {
            //EntryPoint = new CryptoAPILibrary();
            }

        public abstract X509StoreLocation Location { get; }
        public abstract String Name { get; }
        public abstract IntPtr Handle { get; }

        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing)
            {
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
        ~CertificateStorage()
            {
            Dispose(false);
            }
        #endregion
        #region M:Validate(Boolean)
        protected virtual void Validate(Boolean status) {
            if (!status) {
                Exception e;
                var i = Marshal.GetLastWin32Error();
                if ((i >= 0xFFFF) || (i < 0))
                    {
                    e = new COMException($"{FormatMessage(i)} [HRESULT:{(HRESULT)i}]");
                    }
                else
                    {
                    e = new COMException($"{FormatMessage(i)} [{(Win32ErrorCode)i}]");
                    }
                throw e;
                }
            }
        #endregion

        protected abstract void EnsureCore();
        public abstract unsafe X509Certificate Find(CERT_INFO* value, out Exception e);
        public abstract IEnumerable<X509Certificate> Certificates { get; }
        public abstract void Add(IX509Certificate o);
        public abstract void Add(IX509CertificateRevocationList o);
        public abstract void Remove(IX509Certificate o);
        public abstract void Remove(IX509CertificateRevocationList o);

        public abstract void Commit();

        IEnumerable<IX509Certificate> IX509CertificateStorage.Certificates { get {
            foreach (var certificate in Certificates) {
                yield return certificate;
                }
            }}

        #region M:FormatMessage(Int32):String
        protected internal static unsafe String FormatMessage(Int32 source) {
            void* r;
            if (FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER|FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, source,
                ((((UInt32)(SUBLANG_DEFAULT)) << 10) | (UInt32)(LANG_NEUTRAL)),
                &r, 0, new IntPtr[0])) {
                try
                    {
                    return (new String((Char*)r)).Trim();
                    }
                finally
                    {
                    LocalFree(r);
                    }
                }
            return null;
            }
        #endregion

        private   const UInt32 FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        private   const UInt32 FORMAT_MESSAGE_FROM_SYSTEM     = 0x00001000;
        private   const UInt32 LANG_NEUTRAL                   = 0x00;
        private   const UInt32 SUBLANG_DEFAULT                = 0x01;

        [DllImport("kernel32.dll", SetLastError = true)] internal static extern unsafe IntPtr LocalFree(void* handle);
        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, SetLastError = true)] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, Int32 nSize, IntPtr[] arguments);
        }
    }
