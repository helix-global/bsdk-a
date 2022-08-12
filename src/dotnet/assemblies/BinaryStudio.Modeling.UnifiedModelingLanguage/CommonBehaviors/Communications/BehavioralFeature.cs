namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface BehavioralFeature : Feature
        {
        CallConcurrencyKind Concurrency { get; }
        }
    }