using System;
using System.Collections.Generic;

namespace BinaryStudio.PortableExecutable
    {
    public interface ITypeLibraryPropertyDescriptor : ITypeLibraryMemberDescriptor, IEquatable<ITypeLibraryPropertyDescriptor>
        {
        Int32 Id { get; }
        Boolean CanRead { get; }
        Boolean CanWrite { get; }
        ITypeLibraryTypeDescriptor PropertyType { get; }
        ITypeLibraryMethodDescriptor GetMethod { get; }
        ITypeLibraryMethodDescriptor SetMethod { get; }
        IList<ITypeLibraryParameterDescriptor> Parameters { get; }
        }
    }