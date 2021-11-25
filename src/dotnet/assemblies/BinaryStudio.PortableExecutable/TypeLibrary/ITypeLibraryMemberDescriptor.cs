using System;
using System.Collections.Generic;

namespace BinaryStudio.PortableExecutable
    {
    public interface ITypeLibraryMemberDescriptor : IEquatable<ITypeLibraryMemberDescriptor>
        {
        String Name { get; }
        String HelpString { get; }
        TypeLibraryMemberTypes MemberType { get; }
        ITypeLibraryTypeDescriptor DeclaringType { get; }
        IList<TypeLibraryCustomAttribute> CustomAttributes { get; }
        }
    }