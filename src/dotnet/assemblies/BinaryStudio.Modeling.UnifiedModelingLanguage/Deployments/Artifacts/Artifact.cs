using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Artifact : Classifier, NamedElement
        {
        String FileName { get; }
        Manifestation[] Manifestation { get; }
        Artifact[] NestedArtifact { get; }
        Property[] OwnedAttribute { get; }
        Operation[] OwnedOperation { get; }
        }
    }