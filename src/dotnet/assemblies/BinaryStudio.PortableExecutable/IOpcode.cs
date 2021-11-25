using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable
    {
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport, Guid("A4E76CEB-11D0-48B8-A845-BA6DC025296F")]
    internal interface IInstructionOperand
        {
        [return: MarshalAs(UnmanagedType.BStr)] String ToString();
        }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport, Guid("A4E76CEB-11D0-48B8-A845-BA6DC025296E")]
    internal interface IInstructionOperandCollection
        {
        Int32 Count { get; }
        IInstructionOperand this[Int32 index]{ get; }
        }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport, Guid("5DA30660-EF71-41DA-A53E-B356C5F944AC")]
    internal interface IOpcode
        {
        Int32 Size { get; }
        String Instruction { [return: MarshalAs(UnmanagedType.BStr)] get; }
        IInstructionOperandCollection Operands { get; }
        String Postfix { [return: MarshalAs(UnmanagedType.BStr)] get; }
        //[return: MarshalAs(UnmanagedType.BStr)] String ToStringInternal();
        }
    }