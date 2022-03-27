using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BinaryStudio.Security.SmartCard
    {
    public class SCardContext : IDisposable
        {
        public SCardScope Scope { get; }
        public SCardContext(SCardScope scope) {
            Scope = scope;
            var status = SCardEstablishContext(scope, IntPtr.Zero, IntPtr.Zero, out Context);
            if (status != SCARD_S_SUCCESS) { throw new Win32Exception(status); }
            }

        public IList<SCardReader> Readers { get {
            var values = IntPtr.Zero;
            var size = SCARD_AUTOALLOCATE;
            var status = SCardListReadersW(Context, IntPtr.Zero, ref values, ref size);
            if (status != SCARD_S_SUCCESS) { throw new Win32Exception(status); }
            var offset = 0;
            var r = new List<SCardReader>();
            while (size > offset + 1) {
                var i = new SCardReader(this, Marshal.PtrToStringUni(values + offset*2));
                r.Add(i);
                offset += i.Name.Length + 1;
                }
            SCardFreeMemory(Context, values);
            return r;
            }}

        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing) {
            if (Context != null) {
                Context.Dispose();
                Context = null;
                }
            if (disposing)
                {
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
        ~SCardContext()
            {
            Dispose(false);
            }
        #endregion

        [DllImport("winscard.dll")] private static extern Int32 SCardEstablishContext(SCardScope scope, IntPtr r1, IntPtr r2, out SCardContextHandle context);
        [DllImport("winscard.dll",CharSet = CharSet.Unicode, ExactSpelling = true)] private static extern Int32 SCardListReadersW(SCardContextHandle context, IntPtr groups, ref IntPtr readers, ref Int32 size);
        [DllImport("winscard.dll")] private static extern Int32 SCardFreeMemory(SCardContextHandle context, IntPtr ptr);
        internal SCardContextHandle Context;

        private const Int32 SCARD_S_SUCCESS = 0;
        private const Int32 SCARD_AUTOALLOCATE = -1;
        }
    }