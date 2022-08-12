using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PlatformComponents
    {
    //using LocalMemoryU = LocalMemory;
    //public class LocalMemory<T> : LocalMemory
    //    where T: struct
    //    {
    //    public static implicit operator IntPtr(LocalMemory<T> source)       { return source.source;        }
    //    public static unsafe implicit operator void*(LocalMemory<T> source) { return (void*)source.source; }
    //    public static unsafe implicit operator Byte*(LocalMemory<T> source) { return (Byte*)source.source; }
    //    internal LocalMemory(LocalMemoryU source)
    //        {
    //        if (source == null) { throw new ArgumentNullException(nameof(source)); }
    //        Size = source.Size;
    //        this.source = source.source;
    //        source.source = IntPtr.Zero;
    //        }
    //    }

    public class LocalMemory : IDisposable
        {
        protected internal IntPtr source;
        public Int64 Size { get;protected set; }
        public LocalMemory(IntPtr size) { Size = (Int64)size; source = LocalAlloc(size, LMEM_ZEROINIT); }
        public LocalMemory(Int32  size, Int32 flags = LMEM_ZEROINIT) { Size = size; source = LocalAlloc(new IntPtr(size), flags); }
        public LocalMemory(UInt32 size) { Size = size; source = LocalAlloc(new IntPtr(size), LMEM_ZEROINIT); }
        public LocalMemory(Int64  size) { Size = size; source = LocalAlloc(new IntPtr(size), LMEM_ZEROINIT); }
        protected LocalMemory()
            {
            }

        #region M:Dispose(Boolean)
        private void Dispose(Boolean disposing) {
            if (source != IntPtr.Zero) {
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

        internal LocalMemory(IntPtr source, Int64 size)
            {
            this.source = source;
            Size = size;
            }

        public static implicit operator IntPtr(LocalMemory source) { return source.source; }
        public static unsafe implicit operator void*(LocalMemory source) { return (void*)source.source; }
        public static unsafe implicit operator Byte*(LocalMemory source) { return (Byte*)source.source; }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] protected static extern IntPtr LocalAlloc([In] Int32 flags, [In] IntPtr size);
        [DllImport("kernel32.dll")] protected static extern IntPtr LocalFree(IntPtr ptr);

        public const Int32 LMEM_ZEROINIT = 0x0040;
        public const Int32 LMEM_FIXED    = 0x0000;

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() {
            var r = new StringBuilder();
            r.Append("{");
            #if NET35
            r.Append((IntPtr.Size == sizeof(Int64))
                ? ((Int64)source).ToString("X16")
                : ((Int32)source).ToString("X8"));
            #else
            r.Append(Environment.Is64BitProcess
                ? ((Int64)source).ToString("X16")
                : ((Int32)source).ToString("X8"));
            #endif
            r.Append("}");
            if (Size > 0) {
                r.Append(":{");
                r.AppendFormat("{0}", Size);
                r.Append("}");
                }
            return r.ToString();
            }

        private static IntPtr LocalAlloc([In] IntPtr size, Int32 flags)
            {
            return LocalAlloc(flags, size);
            }
        }
    }