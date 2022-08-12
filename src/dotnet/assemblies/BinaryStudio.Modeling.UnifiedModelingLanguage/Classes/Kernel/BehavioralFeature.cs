namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface BehavioralFeature : Feature, Namespace
        {
        Parameter[] OwnedParameter { get; }
        Type[] RaisedException { get; }
        }
    }