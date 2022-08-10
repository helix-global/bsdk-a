using System;
using System.Collections.Generic;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Package : Namespace, PackageableElement
        {
        String URI { get; }
        /// <summary>
        /// References the packaged elements that are Packages.
        /// {subsets <see cref="PackagedElement"/>} (opposite <see cref="NestingPackage"/>)
        /// </summary>
        IList<Package> NestedPackage { get; }
        Package NestingPackage { get; }
        /// <summary>
        /// References the packaged elements that are Types.
        /// {subsets <see cref="PackagedElement"/>}
        /// </summary>
        IList<Type> OwnedType { get; }
        PackageMerge[] PackageMerge { get; }
        /// <summary>
        /// Specifies the packageable elements that are owned by this <see cref="Package"/>.
        /// {subsets <see cref="Namespace.OwnedMember"/>}
        /// </summary>
        IList<PackageableElement> PackagedElement { get; }
        }
    }