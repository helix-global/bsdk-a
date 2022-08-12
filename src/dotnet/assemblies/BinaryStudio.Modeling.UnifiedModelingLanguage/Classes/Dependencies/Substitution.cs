namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Substitution : Realization
        {
        Classifier Contract { get; }
        Classifier SubstitutingClassifier { get; }
        }
    }