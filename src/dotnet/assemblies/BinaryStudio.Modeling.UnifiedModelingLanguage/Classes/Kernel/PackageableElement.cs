﻿namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface PackageableElement : NamedElement
        {
        VisibilityKind Visibility { get; }
        }
    }