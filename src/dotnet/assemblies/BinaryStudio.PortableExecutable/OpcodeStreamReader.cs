using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable
    {
    internal abstract class OpcodeStreamReader : IDisposable
        {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)] private static extern Boolean SetDllDirectory(String pathname);
        [DllImport("intrcpx.dll", EntryPoint = "CreateOpcodeStreamReader")] private static extern Int32 CreateOpcodeStreamReader(IntPtr i, Int64 sz, out IOpcodeStreamReader scope);
        [DllImport("intrcpx.dll", EntryPoint = "CreateOpcodeStreamReader")] private static extern Int32 CreateOpcodeStreamReader([MarshalAs(UnmanagedType.LPArray)] Byte[] i, Int64 sz, out IOpcodeStreamReader scope);

        private IOpcodeStreamReader Source;
        public OpcodeStreamReader(IntPtr source)
            {
            //SetDllDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), String.Format("{0}", (IntPtr.Size == 4) ? "x86" : "x64")));
            //CreateOpcodeStreamReader(source, -1, out Source);
            }

        public OpcodeStreamReader(Byte[] source)
            {
            //SetDllDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), String.Format("{0}", (IntPtr.Size == 4) ? "x86" : "x64")));
            //CreateOpcodeStreamReader(source, source.LongLength, out Source);
            }

        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing) {
            if (Source != null) {
                Source = null;
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
        ~OpcodeStreamReader() {
            Dispose(false);
            }
        #endregion

        public virtual IOpcode Read() {
            return Source.Read();
            }
        }
    }