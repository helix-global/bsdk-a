using System;
using System.IO;
using BinaryStudio.PlatformComponents.Unix;
using BinaryStudio.PlatformComponents.Win32;
using Microsoft.Win32.SafeHandles;

namespace BinaryStudio.PlatformComponents
    {
    public abstract class SharedObject : IDisposable
        {
        public virtual String FileName { get; }

        public SafeHandleZeroOrMinusOneIsInvalid Handle { get {
            EnsureCore();
            return handle;
            }}

        protected abstract SafeHandleZeroOrMinusOneIsInvalid EnsureOverride();
        public abstract IntPtr Get(String methodname);

        protected SharedObject(String filename)
            {
            if (filename == null) { throw new ArgumentNullException(nameof(filename)); }
            #if NET35
            if (String.IsNullOrEmpty(filename)) { throw new ArgumentOutOfRangeException(nameof(filename)); }
            #else
            if (String.IsNullOrWhiteSpace(filename)) { throw new ArgumentOutOfRangeException(nameof(filename)); }
            #endif
            FileName = filename;
            }

        #region M:EnsureCore
        protected void EnsureCore() {
            if (handle == null) {
                handle = EnsureOverride();
                if ((handle == null) || (handle.IsInvalid)) {
                    throw new InvalidOperationException();
                    }
                }
            }
        #endregion
        #region M:Create(String):SharedObject
        public static SharedObject Create(String filename)
            {
            if (filename == null) { throw new ArgumentNullException(nameof(filename)); }
            #if NET35
            if (String.IsNullOrEmpty(filename)) { throw new ArgumentOutOfRangeException(nameof(filename)); }
            #else
            if (String.IsNullOrWhiteSpace(filename)) { throw new ArgumentOutOfRangeException(nameof(filename)); }
            #endif
            switch (Environment.OSVersion.Platform)
                {
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                    {
                    return new Win32SharedObject(filename);
                    }
                case PlatformID.Unix:
                    {
                    return new UnixSharedObject(filename);
                    }
                case PlatformID.Xbox:
                case PlatformID.MacOSX:
                default: throw new ArgumentOutOfRangeException();
                }
            }
        #endregion
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
        ~SharedObject()
            {
            Dispose(false);
            }
        #endregion

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return Path.GetFileName(FileName);
            }

        private SafeHandleZeroOrMinusOneIsInvalid handle;
        }
    }