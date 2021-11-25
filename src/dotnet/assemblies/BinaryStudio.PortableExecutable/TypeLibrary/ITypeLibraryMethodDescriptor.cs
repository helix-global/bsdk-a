using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable
    {
    using CALLCONV = System.Runtime.InteropServices.ComTypes.CALLCONV;
    public interface ITypeLibraryMethodDescriptor : ITypeLibraryMemberDescriptor, IEquatable<ITypeLibraryMethodDescriptor>
        {
        Int32 Id { get; }
        TypeLibFuncFlags Flags { get; }
        TypeLibraryMethodAttributes Attributes { get; }
        ITypeLibraryTypeDescriptor ReturnType { get; }
        IList<ITypeLibraryParameterDescriptor> Parameters { get; }
        CALLCONV CallingConvention { get; }
        }
    }