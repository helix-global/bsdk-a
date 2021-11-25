using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider.Internal
    {
    internal class HGlobalAnsi : IDisposable
        {
        private IntPtr value;
        public HGlobalAnsi(IntPtr source)
            {
            value = source;
            }

        public HGlobalAnsi(SecureString source)
            {
            value = Marshal.SecureStringToGlobalAllocAnsi(source);
            }

        #region M:Dispose(Boolean)
        private void Dispose(Boolean disposing) {
            if (value != IntPtr.Zero) {
                Marshal.ZeroFreeGlobalAllocAnsi(value);
                value = IntPtr.Zero;
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
        #region F:Finalize
        ~HGlobalAnsi()
            {
            Dispose(false);
            }
        #endregion

        public static implicit operator IntPtr(HGlobalAnsi source) { return source.value; }
        public static unsafe implicit operator void*(HGlobalAnsi source) { return (void*)source.value; }
        public static unsafe implicit operator Byte*(HGlobalAnsi source) { return (Byte*)source.value; }

        public unsafe Int32 Length { get {
            if (value == IntPtr.Zero) { return 0; }
            var I = (Byte*)value;
            if (I == null) { return 0; }
            for (var i = 0;; i++) {
                if (I[i] == 0) {
                    return i;
                    }
                }
            }}
        }
    }