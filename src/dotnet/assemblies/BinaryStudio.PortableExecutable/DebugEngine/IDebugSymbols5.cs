using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("C65FA83E-1E69-475E-8E0E-B5D79E9CC17E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugSymbols5 : IDebugSymbols4
        {
        UInt32 GetCurrentScopeFrameIndexEx(
            [In] UInt32 Flags);

        void SetScopeFrameByIndexEx(
            [In] UInt32 Flags,
            [In] UInt32 Index);
        }
    }