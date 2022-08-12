namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface InformationItem : Classifier
        {
        Classifier[] Represented { get; }
        }
    }