using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface NamedElement : Element
        {
        String Name { get; }
        Namespace Namespace { get; }
        String QualifiedName { get; }
        VisibilityKind Visibility { get; }
        }
    }