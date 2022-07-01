using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Generalization : DirectedRelationship
        {
        Classifier General { get; }
        Boolean IsSubstitutable { get; }
        Classifier Specific { get; }
        }
    }