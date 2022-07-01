namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Signal : Classifier
        {
        Property[] OwnedAttribute { get; }
        }
    }