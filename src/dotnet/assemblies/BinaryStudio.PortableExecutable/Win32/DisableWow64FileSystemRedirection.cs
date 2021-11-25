using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BinaryStudio.PortableExecutable.Win32
    {
    public class DisableWow64FileSystemRedirection : IDisposable
        {
        private unsafe void* redirect = null;
        public unsafe DisableWow64FileSystemRedirection() {
            if (!Environment.Is64BitProcess) {
                fixed (void** r = &redirect) {
                    Wow64DisableWow64FsRedirection(r);
                    }
                }
            }

        protected virtual unsafe void Dispose(Boolean disposing) {
            if (!Environment.Is64BitProcess) {
                Wow64RevertWow64FsRedirection(redirect);
                }
            }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
            }

        ~DisableWow64FileSystemRedirection() {
            Dispose(false);
            }

        [DllImport("kernel32.dll", SetLastError = true)][SecurityCritical, SuppressUnmanagedCodeSecurity] private static extern unsafe Boolean Wow64DisableWow64FsRedirection(void** redirect);
        [DllImport("kernel32.dll", SetLastError = true)][SecurityCritical, SuppressUnmanagedCodeSecurity] private static extern unsafe Boolean Wow64RevertWow64FsRedirection(void* redirect);
        }
    }