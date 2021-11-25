using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("BC0D583F-126D-43A1-9CC4-A860AB1D537B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugControl6 : IDebugControl5
        {
        UInt32 GetExecutionStatusEx();
        void GetSynchronizationStatus(
            [Out] out UInt32 SendsAttempted,
            [Out] out UInt32 SecondsSinceLastResponse);
        }
    }