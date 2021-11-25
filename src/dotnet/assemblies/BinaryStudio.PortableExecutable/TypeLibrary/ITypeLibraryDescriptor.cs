using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;

namespace BinaryStudio.PortableExecutable
    {
    public interface ITypeLibraryDescriptor
        {
        Version Version { get; }
        CultureInfo Culture { get; }
        String Name { get; }
        String HelpFile { get; }
        String HelpString { get; }
        Guid UniqueIdentifier { get; }
        Int32 HelpContext { get; }
        IList<ITypeLibraryTypeDescriptor> DefinedTypes { get; }
        LIBFLAGS Flags { get; }
        SYSKIND TargetOperatingSystemPlatform { get; }
        IList<TypeLibraryCustomAttribute> CustomAttributes { get; }
        IList<ITypeLibraryDescriptor> ImportedLibraries { get; }
        TypeLibraryIdentifier Identifier { get; }
        }
    }