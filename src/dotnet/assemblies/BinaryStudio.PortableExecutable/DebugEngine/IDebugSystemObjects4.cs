using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, Guid("489468E6-7D0F-4AF5-87AB-25207454D553"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugSystemObjects4 : IDebugSystemObjects3
        {
        void GetCurrentProcessExecutableNameWide(
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 ExeSize);

        void GetCurrentSystemServerNameWide(
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 NameSize);
        }
    }