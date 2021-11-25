using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace BinaryStudio.Security.Cryptography.Win32
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Design", "CA1060:Move pinvokes to native methods class", Justification = "<Pending>")]
    #endif
    internal class LocalMemory : IDisposable
        {
        private IntPtr source;
        public Int64 Size { get; }
        public LocalMemory(IntPtr size) { Size = (Int64)size; source = LocalAlloc(size); }
        public LocalMemory(Int32  size) { Size = size; source = LocalAlloc(new IntPtr(size)); }
        public LocalMemory(UInt32 size) { Size = size; source = LocalAlloc(new IntPtr(size)); }
        public LocalMemory(Int64  size) { Size = size; source = LocalAlloc(new IntPtr(size)); }

        #region M:Dispose(Boolean)
        private void Dispose(Boolean disposing) {
            if (source != IntPtr.Zero) {
                //Marshal.FreeHGlobal(source);
                Debug.Print($"LocalFree({(Int64)source:X16}");
                LocalFree(source);
                source = IntPtr.Zero;
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
        ~LocalMemory()
            {
            Dispose(false);
            }
        #endregion

        public static implicit operator IntPtr(LocalMemory source) { return source.source; }
        public static unsafe implicit operator void*(LocalMemory source) { return (void*)source.source; }
        public static unsafe implicit operator Byte*(LocalMemory source) { return (Byte*)source.source; }

        //#if CAPILITE
        //[DllImport("capi20")] protected static extern IntPtr LocalAlloc([In] UInt32 flags, [In] IntPtr size);
        //[DllImport("capi20")] protected static extern IntPtr LocalFree(IntPtr ptr);
        //#else
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] protected static extern IntPtr LocalAlloc([In] UInt32 flags, [In] IntPtr size);
        [DllImport("kernel32.dll")] protected static extern IntPtr LocalFree(IntPtr ptr);
        //#endif

        internal const UInt32 LMEM_ZEROINIT = 0x0040;

        public override String ToString()
            {
            return $"${(Int64)source:X8}";
            }

        private static IntPtr LocalAlloc([In] IntPtr size)
            {
            //return Marshal.AllocHGlobal(size);
            var r = LocalAlloc(LMEM_ZEROINIT, size);
            //Debug.Print($"LocalAlloc({(Int64)size}):{(Int64)r:X16}");
            return r;
            }
        }
    }