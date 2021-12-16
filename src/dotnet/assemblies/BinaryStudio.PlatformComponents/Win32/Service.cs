using System;

namespace BinaryStudio.PlatformComponents.Win32
    {
    public class Service : IDisposable
        {
        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing) {
            if (disposing) {
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
        ~Service()
            {
            Dispose(false);
            }
        #endregion
        }
    }