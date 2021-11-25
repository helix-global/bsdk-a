using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [ComImport, ComConversionLoss, Guid("7DF74A86-B03F-407F-90AB-A20DADCEAD08"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugControl3 : IDebugControl2
        {
        UInt32 GetAssemblyOptions();

        void AddAssemblyOptions(
            [In] UInt32 Options);

        void RemoveAssemblyOptions(
            [In] UInt32 Options);

        void SetAssemblyOptions([In] DEBUG_ASMOPT Options);

        UInt32 GetExpressionSyntax();

        void SetExpressionSyntax(
            [In] UInt32 Flags);

        void SetExpressionSyntaxByName(
            [In, MarshalAs(UnmanagedType.LPStr)] String AbbrevName);

        UInt32 GetNumberExpressionSyntaxes();

        void GetExpressionSyntaxNames(
            [In] UInt32 Index,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder FullNameBuffer,
            [In] UInt32 FullNameBufferSize,
            [Out] out UInt32 FullNameSize,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder AbbrevNameBuffer,
            [In] UInt32 AbbrevNameBufferSize,
            [Out] out UInt32 AbbrevNameSize);

        UInt32 GetNumberEvents();

        void GetEventIndexDescription(
            [In] UInt32 Index,
            [In] UInt32 Which,
            [In, MarshalAs(UnmanagedType.LPStr)] String Buffer,
            [In] UInt32 BufferSize,
            [Out] out UInt32 DescSize);

        UInt32 GetCurrentEventIndex();

        UInt32 SetNextEventIndex(
            [In] UInt32 Relation,
            [In] UInt32 Value);
        }
    }