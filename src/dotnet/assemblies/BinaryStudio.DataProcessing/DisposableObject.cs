#pragma warning disable 1591
using System;

namespace BinaryStudio.DataProcessing
    {
    public class DisposableObject : IDisposable
        {
        #region D:DisposableObject
        ~DisposableObject() {
            Dispose(false);
            }
        #endregion

        protected Boolean IsDisposed { get; private set; }
        public event EventHandler Disposing;

        #region M:DisposeManagedResources
        protected virtual void DisposeManagedResources() { }
        #endregion
        #region M:DisposeNativeResources
        protected virtual void DisposeNativeResources() { }
        #endregion

        #region M:Dispose
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Dispose(IDisposable)
        public static void Dispose(IDisposable source) {
            if (source != null)
                {
                source.Dispose();
                }
            }
        #endregion
        #region M:Dispose<T>(T)
        public static void Dispose<T>(T source) {
            if (source != null)
                {
                if (source is IDisposable)
                    {
                    Dispose((IDisposable)source);
                    }
                }
            }
        #endregion
        #region M:Dispose<T>(ref T)
        public static void Dispose<T>(ref T source)
            where T : class {
            if (source != null)
                {
                Dispose(source);
                source = null;
                }
            }
        #endregion
        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing) {
            if (!IsDisposed)
                {
                try
                    {
                    if (disposing)
                        {
                        if (Disposing != null)
                            {
                            Disposing(this, EventArgs.Empty);
                            Disposing = null;
                            }
                        DisposeManagedResources();
                        }
                    DisposeNativeResources();
                    }
                finally
                    {
                    IsDisposed = true;
                    }
                }
            }
        #endregion
        }
    }
