namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Classifier : Namespace
        {
        UseCase[] OwnedUseCase { get; }
        UseCase[] UseCase { get; }
        }
    }