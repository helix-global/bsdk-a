using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable
    {
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport, Guid("4E3A03ED-10A5-4F21-B907-60251FAB8C34")]
    internal interface IOpcodeStreamReader
        {
        IOpcode Read();
        }
    }