namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Interface : Classifier
        {
        Reception[] OwnedReception { get; }
        }
    }