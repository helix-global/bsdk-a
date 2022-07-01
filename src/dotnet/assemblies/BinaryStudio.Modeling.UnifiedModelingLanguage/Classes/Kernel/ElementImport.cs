using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ElementImport : DirectedRelationship
        {
        String Alias { get; }
        PackageableElement ImportedElement { get; }
        Namespace ImportingNamespace { get; }
        VisibilityKind Visibility { get; }
        }
    }