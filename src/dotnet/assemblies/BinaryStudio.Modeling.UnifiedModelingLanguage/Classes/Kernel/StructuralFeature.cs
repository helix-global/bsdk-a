using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface StructuralFeature : Feature, TypedElement, MultiplicityElement
        {
        Boolean IsReadOnly { get; }
        }
    }