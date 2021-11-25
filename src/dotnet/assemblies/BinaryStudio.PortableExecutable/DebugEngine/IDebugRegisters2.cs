using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("1656AFA9-19C6-4E3A-97E7-5DC9160CF9C4"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugRegisters2 : IDebugRegisters
        {
        void GetDescriptionWide(
            [In] UInt32 Register,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize,
            [Out] out DEBUG_REGISTER_DESCRIPTION Desc);

        UInt32 GetIndexByNameWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Name);

        UInt32 GetNumberPseudoRegisters();

        void GetPseudoDescription(
            [In] UInt32 Register,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize,
            [Out] out UInt64 TypeModule,
            [Out] out UInt32 TypeId);

        void GetPseudoDescriptionWide(
            [In] UInt32 Register,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize,
            [Out] out UInt64 TypeModule,
            [Out] out UInt32 TypeId);

        UInt32 GetPseudoIndexByName(
            [In, MarshalAs(UnmanagedType.LPStr)] String Name);

        UInt32 GetPseudoIndexByNameWide(
            [In, MarshalAs(UnmanagedType.LPWStr)] String Name);

        void GetPseudoValues(
            [In] UInt32 Source,
            [In] UInt32 Count,
            [In] ref UInt32 Indices,
            [In] UInt32 Start,
            [Out] IntPtr Values);

        void SetPseudoValues(
            [In] UInt32 Source,
            [In] UInt32 Count,
            [In] ref UInt32 Indices,
            [In] UInt32 Start,
            [In] IntPtr Values);

        void GetValues2(
            [In] UInt32 Source,
            [In] UInt32 Count,
            [In] ref UInt32 Indices,
            [In] UInt32 Start,
            [Out] IntPtr Values);

        void SetValues2(
            [In] UInt32 Source,
            [In] UInt32 Count,
            [In] ref UInt32 Indices,
            [In] UInt32 Start = default(UInt32),
            [In] IntPtr Values = default(IntPtr));

        void OutputRegisters2(
            [In] UInt32 OutputControl,
            [In] UInt32 Source,
            [In] UInt32 Flags);

        UInt64 GetInstructionOffset2(
            [In] UInt32 Source);

        UInt64 GetStackOffset2(
            [In] UInt32 Source);

        UInt64 GetFrameOffset2(
            [In] UInt32 Source);
        }
    }