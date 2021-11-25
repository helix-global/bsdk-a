using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("B86FB3B1-80D4-475B-AEA3-CF06539CF63A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugControl7 : IDebugControl6
        {
        void GetDebuggeeType2(
            [In] UInt32 Flags,
            [Out] out UInt32 Class,
            [Out] out UInt32 Qualifier);
        }
    }