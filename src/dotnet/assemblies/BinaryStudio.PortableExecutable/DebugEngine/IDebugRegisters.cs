using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("CE289126-9E84-45A7-937E-67BB18691493"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugRegisters
        {
        UInt32 GetNumberRegisters();

        void GetDescription(
            [In] UInt32 Register,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize,
            [Out] out DEBUG_REGISTER_DESCRIPTION Desc);

        UInt32 GetIndexByName(
            [In, MarshalAs(UnmanagedType.LPStr)] String Name);

        DEBUG_VALUE GetValue(
            [In] UInt32 Register);

        void SetValue(
            [In] UInt32 Register,
            [In] ref DEBUG_VALUE Value);

        void GetValues(
            [In] UInt32 Count,
            [In] ref UInt32 Indices,
            [In] UInt32 Start,
            [Out] IntPtr Values);

        void SetValues(
            [In] UInt32 Count,
            [In] ref UInt32 Indices,
            [In] UInt32 Start,
            [In] IntPtr Values);

        void OutputRegisters(
            [In] UInt32 OutputControl,
            [In] UInt32 Flags);

        UInt64 GetInstructionOffset();

        UInt64 GetStackOffset();

        UInt64 GetFrameOffset();
        }
    }