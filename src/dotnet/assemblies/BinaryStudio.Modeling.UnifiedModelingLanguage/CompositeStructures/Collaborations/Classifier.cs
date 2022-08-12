namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Classifier : Namespace
        {
        CollaborationUse[] CollaborationUse { get; }
        CollaborationUse Representation { get; }
        }
    }