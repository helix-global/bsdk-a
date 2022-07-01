using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Package : Namespace, PackageableElement
        {
        String URI { get; }
        Package[] NestedPackage { get; }
        Package NestingPackage { get; }
        Type[] OwnedType { get; }
        PackageMerge[] PackageMerge { get; }
        PackageableElement[] PackagedElement { get; }
        }
    }