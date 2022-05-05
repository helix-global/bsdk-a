using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace BinaryStudio.IO
    {
    [DebuggerDisplay("{ToString(),nq}")]
    public class FileMappingMemory : IDisposable
        {
        private ViewOfFileHandle Handle { get; }
        public FileMappingMemory(FileMapping mapping)
            {
            Handle = MapViewOfFile(mapping.Mapping, FileMappingAccess.Read, 0,0, IntPtr.Zero);
            }

        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing) {
            if (disposing) {
                }
            }
        #endregion-
        #region M:Dispose
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Finalize
        ~FileMappingMemory()
            {
            Dispose(false);
            }
        #endregion

        public static unsafe explicit operator void*(FileMappingMemory source)
            {
            return (source.Handle != null)
                ? (void*)source.Handle
                : null;
            }

        public static unsafe explicit operator IntPtr(FileMappingMemory source)
            {
            return (source.Handle != null)
                ? (IntPtr)(void*)source.Handle
                : IntPtr.Zero;
            }

        [DllImport("kernel32.dll", SetLastError = true)][SecurityCritical, SuppressUnmanagedCodeSecurity] private static extern ViewOfFileHandle MapViewOfFile(FileMappingHandle filemappingobject, FileMappingAccess desiredaccess, UInt32 fileoffsethigh, UInt32 fileoffsetlow, IntPtr numberofbytestomap);

        public override String ToString()
            {
            return Handle.ToString();
            }
        }
    }