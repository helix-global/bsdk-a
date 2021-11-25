using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable
    {
    public interface ITypeLibraryTypeDescriptor : ITypeLibraryMemberDescriptor, IEquatable<ITypeLibraryTypeDescriptor>
        {
        String LibraryQualifiedName { get; }
        TypeLibTypeFlags Flags { get; }
        ITypeLibraryTypeDescriptor UnderlyingType { get; }
        ITypeLibraryTypeDescriptor BaseType { get; }
        ITypeLibraryDescriptor Library { get; }
        Boolean IsAlias { get; }
        Boolean IsPrimitive { get; }
        Boolean IsPointer { get; }
        Boolean IsArray { get; }
        Boolean IsEnum { get; }
        Boolean IsClass { get; }
        Boolean IsUnion { get; }
        Boolean IsInterface { get; }
        Boolean IsStructure { get; }
        Boolean IsModule { get; }
        Boolean IsDispatch { get; }
        Guid? UniqueIdentifier { get; }
        ITypeLibraryFixedArrayTypeDescriptor FixedArrayTypeDescriptor { get; }
        IList<ITypeLibraryFieldDescriptor>    DeclaredFields        { get; }
        IList<ITypeLibraryMethodDescriptor>   DeclaredMethods       { get; }
        IList<ITypeLibraryPropertyDescriptor> DeclaredProperties    { get; }
        IList<ITypeLibraryTypeReference>      ImplementedInterfaces { get; }
        Version Version { get; }
        }
    }