using System;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PlatformComponents.Win32;

namespace BinaryStudio.Security.SmartCard
    {
    public class SCardObject : IDisposable
        {
        internal LocalMemoryManager manager;

        #region M:Validate(HRESULT)
        protected virtual void Validate(HRESULT status) {
            if (status != HRESULT.SCARD_S_SUCCESS) {
                throw new HResultException((Int32)status);
                }
            }
        #endregion
        #region M:Validate(HRESULT)
        protected virtual Boolean Validate(out HResultException e, HRESULT status) {
            e = null;
            if (status == HRESULT.SCARD_S_SUCCESS) { return true; }
            e = new HResultException((Int32)status);
            return false;
            }
        #endregion

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
        /// <summary>
        /// Releases the unmanaged resources used by the instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual void Dispose(Boolean disposing) {
            if (disposing) {
                if (manager != null) {
                    manager.Dispose();
                    manager = null;
                    }
                }
            }
        #endregion
        #region M:Dispose
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Finalize
        /// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
        ~SCardObject()
            {
            Dispose(false);
            }
        #endregion
        }
    }