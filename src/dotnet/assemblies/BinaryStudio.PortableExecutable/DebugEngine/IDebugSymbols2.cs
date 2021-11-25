using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("3A707211-AFDD-4495-AD4F-56FECDF8163F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugSymbols2 : IDebugSymbols
        {
        void GetModuleVersionInformation(
            [In] UInt32 Index,
            [In] UInt64 Base,
            [In, MarshalAs(UnmanagedType.LPStr)] String Item,
            [Out] IntPtr Buffer,
            [In] UInt32 BufferSize,
            out UInt32 VerInfoSize);

        void GetModuleNameString(
            [In] UInt32 Which,
            [In] UInt32 Index,
            [In] UInt64 Base,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 NameSize);

        void GetConstantName(
            [In] UInt64 Module,
            [In] UInt32 TypeId,
            [In] UInt64 Value,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize);

        void GetFieldName(
            [In] UInt64 Module,
            [In] UInt32 TypeId,
            [In] UInt32 FieldIndex,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder NameBuffer,
            [In] UInt32 NameBufferSize,
            [Out] out UInt32 NameSize);

        UInt32 GetTypeOptions();

        void AddTypeOptions(
            [In] UInt32 Options);

        void RemoveTypeOptions(
            [In] UInt32 Options);

        void SetTypeOptions(
            [In] UInt32 Options);
        }
    }