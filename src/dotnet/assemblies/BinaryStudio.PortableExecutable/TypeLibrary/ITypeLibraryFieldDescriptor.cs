using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable
    {
    public interface ITypeLibraryFieldDescriptor : ITypeLibraryMemberDescriptor, IEquatable<ITypeLibraryFieldDescriptor>
        {
        Int32 Id { get; }
        ITypeLibraryTypeDescriptor FieldType { get; }
        TypeLibVarFlags Flags { get; }
        TypeLibraryFieldAttributes Attributes { get; }
        Boolean IsLiteral { get; }
        Object LiteralValue { get; }
        }
    }