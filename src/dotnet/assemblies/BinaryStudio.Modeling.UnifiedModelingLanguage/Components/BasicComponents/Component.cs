using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Component : NamedElement, Class
        {
        Boolean IsIndirectlyInstantiated { get; }
        Interface[] Provided { get; }
        ComponentRealization[] Realization { get; }
        Interface[] Required { get; }
        }
    }