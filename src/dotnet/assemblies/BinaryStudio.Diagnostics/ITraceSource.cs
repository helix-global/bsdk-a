using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.Diagnostics
    {
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport, Guid("96843321-9587-4A77-816B-8FA498F6C3BE")]
    public interface ITraceSource
        {
        unsafe void Trace(TraceSourceType type, UInt32 process, UInt32 thread, String source, Int64 iscope, Int64* oscope, ITraceParameterCollection args);
        }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport, Guid("FBF01F17-222A-4FBE-8A5B-D4BCD074019B")]
    public interface ITraceParameter
        {
        Int32 Size { get; }
        Byte[] Value { [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] get; }
        [return: MarshalAs(UnmanagedType.BStr)] String ToString();
        }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport, Guid("C2556245-118B-4330-9F43-0CD97F4B4EAB")]
    public interface ITraceParameterCollection
        {
        Int32 Count { get; }
        ITraceParameter this[Int32 i] { get; }
        }
    }