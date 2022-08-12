using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    public abstract class ServiceObject : IDisposable
        {
        public virtual IntPtr Handle { get; }

        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing) {
            }
        #endregion

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }

        #region M:Finalize
        ~ServiceObject()
            {
            Dispose(false);
            }
        #endregion

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            #if NET35
            return $"{{{((IntPtr.Size == sizeof(Int64)) ? ((Int64)Handle).ToString("x16") : ((Int32)Handle).ToString("x8"))}}}";
            #else
            return $"{{{(Environment.Is64BitProcess ? ((Int64)Handle).ToString("x16") : ((Int32)Handle).ToString("x8"))}}}";
            #endif
            }

        #region M:CloseServiceHandle([ref]IntPtr)
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern Boolean CloseServiceHandle(IntPtr sc);
        protected static void CloseServiceHandle(ref IntPtr sc) {
            if (sc != IntPtr.Zero) {
                CloseServiceHandle(sc);
                sc = IntPtr.Zero;
                }
            }
        #endregion
        }
    }