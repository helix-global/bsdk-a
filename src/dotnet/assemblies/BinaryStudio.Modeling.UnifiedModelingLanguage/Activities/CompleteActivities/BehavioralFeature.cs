namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface BehavioralFeature : Namespace, Feature
        {
        ParameterSet[] OwnedParameterSet { get; }
        }
    }