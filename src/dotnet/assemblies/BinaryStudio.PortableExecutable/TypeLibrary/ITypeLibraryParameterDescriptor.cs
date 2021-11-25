using System;
using System.Runtime.InteropServices.ComTypes;

namespace BinaryStudio.PortableExecutable
    {
    public interface ITypeLibraryParameterDescriptor
        {
        ITypeLibraryTypeDescriptor ParameterType { get; }
        PARAMFLAG Flags { get; }
        Boolean IsIn       { get; }
        Boolean IsLcid     { get; }
        Boolean IsOptional { get; }
        Boolean IsOut      { get; }
        Boolean IsRetval   { get; }
        String Name { get; }
        }
    }